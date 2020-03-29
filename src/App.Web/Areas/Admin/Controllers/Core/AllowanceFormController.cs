using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
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
    public class AllowanceFormController : BaseController<AllowanceForm, IAllowanceFormService, AllowanceFormViewModel, AllowanceFormModelForm, Guid>
    {
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;
        public AllowanceFormController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IAllowanceFormService service, 
            IUserHelper userHelper, 
            IHostingEnvironment environment, 
            ExcelHelper excel) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(AllowanceForm item)
        {
            item.Name = item.Name.ToUpper();
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(AllowanceForm item, AllowanceFormModelForm model)
        {
            item.Name = model.Name;
            item.Value = model.Value;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
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
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                                {

                                    var Name = worksheet.Cells[row, 1].Value;
                                    var Value = worksheet.Cells[row, 2].Value;
                                    AllowanceForm AllowanceFormFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Name.ToString()));
                                    if (AllowanceFormFind == null)
                                    {
                                        AllowanceForm af = new AllowanceForm() { Name = Name.ToString(), Value = decimal.Parse(Value.ToString()) };
                                        Service.Add(af);
                                        TOTAL_INSERT++;
                                    }
                                    else
                                    {

                                        AllowanceForm af = Service.GetById(AllowanceFormFind.Id);
                                        af.Value = decimal.Parse(Value.ToString());
                                        Service.Update(af);
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
