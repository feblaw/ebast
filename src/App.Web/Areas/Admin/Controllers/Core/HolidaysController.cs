using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class HolidaysController : BaseController<Holidays, IHolidaysService, HolidaysViewModel, HolidaysFormModel, Guid>
    {
        private readonly IHolidaysService service;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public HolidaysController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ExcelHelper excel,
            IHostingEnvironment environment,
            IHolidaysService service,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.service = service;
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(Holidays item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(Holidays item, HolidaysFormModel model)
        {
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
        }

        public override IActionResult Create()
        {
            ViewBag.DayType = Enum.GetValues(typeof(DayType)).Cast<DayType>().Select(v => new SelectListItem
            {
                Text = Extension.GetEnumDescription(v),
                Value = ((int)v).ToString()
            }).ToList();
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.DayType = Enum.GetValues(typeof(DayType)).Cast<DayType>().Select(v => new SelectListItem
            {
                Text = Extension.GetEnumDescription(v),
                Value = ((int)v).ToString()
            }).ToList();
            return base.Edit(id);
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
            var uploads = System.IO.Path.Combine(_environment.WebRootPath, "temp");
            file = Request.Form.Files[0];
            using (var fileStream = new FileStream(System.IO.Path.Combine(uploads, file.FileName),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                await file.CopyToAsync(fileStream);
                try
                {
                    using (var Stream = new FileStream(System.IO.Path.Combine(uploads, file.FileName),
                          FileMode.OpenOrCreate,
                          FileAccess.ReadWrite,
                          FileShare.ReadWrite))
                    {
                        using (ExcelPackage package = new ExcelPackage(Stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null && worksheet.Cells[row, 3].Value != null)
                                {
                                    var DateDay = worksheet.Cells[row, 1].Value;
                                    var Type = worksheet.Cells[row, 2].Value;
                                    var Description = worksheet.Cells[row, 3].Value;
                                    service.Add(new Holidays() { DateDay = DateTime.Parse(DateDay.ToString()), DayType = (DayType)Enum.Parse(typeof(DayType), Type.ToString()), Description = Description.ToString() });
                                    TOTAL_INSERT++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }
    }
}
