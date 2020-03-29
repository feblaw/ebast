using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class SubOpsController : BaseController<SubOps, ISubOpsService, SubOpsViewModel, SubOpsModelForm, Guid>
    {
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public SubOpsController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISubOpsService service,
            IHostingEnvironment environment,
            ExcelHelper excel,
            IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(SubOps item)
        {
            item.Code = item.Code.ToUpper();
            item.Status = true;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(SubOps item, SubOpsModelForm model)
        {
            item.Code = model.Code.ToUpper();
            item.Description = model.Description;
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

                                    var SubsCode = worksheet.Cells[row, 1].Value;
                                    var Description = worksheet.Cells[row, 2].Value;
                                    SubOps SubOpsFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Code) == _excel.TruncateString(SubsCode.ToString()));
                                    if (SubOpsFind == null)
                                    {
                                        SubOps sb = new SubOps { Code = SubsCode.ToString(), Description = Description.ToString() };
                                        Service.Add(sb);
                                        TOTAL_INSERT++;
                                    }
                                    else
                                    {
  
                                        SubOps sb = Service.GetById(SubOpsFind.Id);
                                        sb.Description = Description.ToString();
                                        Service.Update(sb);
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
