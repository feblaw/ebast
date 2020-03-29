using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class TimeSheetController : BaseController<TimeSheetType, ITimeSheetTypeService, TimeSheetTypeViewModel, TimeSheetTypeModelForm, Guid>
    {
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public TimeSheetController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment environment,
            ExcelHelper excel,
            ITimeSheetTypeService service, 
            IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(TimeSheetType item)
        {
            item.Type = item.Type.ToUpper();
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
            base.CreateData(item);
        }

        protected override void UpdateData(TimeSheetType item, TimeSheetTypeModelForm model)
        {
            item.Type = model.Type.ToUpper();
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
            base.UpdateData(item, model);
        }
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
            int TOTAL_UPDATE = 0;

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
                                if (worksheet.Cells[row, 1].Value != null)
                                {

                                    var Type = worksheet.Cells[row, 1].Value;
                                    TimeSheetType TimeSheetTypeFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Type) == _excel.TruncateString(Type.ToString()));
                                    if (TimeSheetTypeFind == null)
                                    {
                                        TimeSheetType ac = new TimeSheetType { Type = Type.ToString() };
                                        Service.Add(ac);
                                        TOTAL_INSERT++;
                                    }
                                    else
                                    {

                                        TimeSheetType ac = Service.GetById(TimeSheetTypeFind.Id);
                                        ac.Type = Type.ToString();
                                        Service.Update(ac);
                                        TOTAL_UPDATE++;
                                    }
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

            TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT + " , Total Updated = " + TOTAL_UPDATE;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }
    }
}
