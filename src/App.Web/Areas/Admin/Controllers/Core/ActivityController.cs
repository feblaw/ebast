using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ActivityController : BaseController<ActivityCode, IActivityCodeService, ActivityCodeViewModel, ActivityCodeModelForm, Guid>
    {
        private readonly IHostingEnvironment environment;
        private readonly ExcelHelper excel;
        private readonly IUserHelper _userHelper;

        public ActivityController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IActivityCodeService service, IUserHelper userHelper, IHostingEnvironment environment, ExcelHelper excel) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.environment = environment;
            this.excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(ActivityCode item)
        {
            item.Status = Status.Active;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
            base.CreateData(item);
        }

        protected override void UpdateData(ActivityCode item, ActivityCodeModelForm model)
        {
            item.Code = model.Code;
            item.Description = model.Description;
            item.Status = model.Status;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
            base.UpdateData(item, model);
        }
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
            int TOTAL_UPDATE = 0;

            var uploads = System.IO.Path.Combine(this.environment.WebRootPath, "temp");
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

                                    var ActivityCode = worksheet.Cells[row, 1].Value;
                                    var Description = worksheet.Cells[row, 2].Value;
                                    ActivityCode ActivityCodeFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => this.excel.TruncateString(x.Code) == this.excel.TruncateString(ActivityCode.ToString()));
                                    if (ActivityCodeFind == null)
                                    {
                                        ActivityCode ac = new ActivityCode { Code = ActivityCode.ToString(), Description = Description.ToString(), Status = Status.Active};
                                        Service.Add(ac);
                                        TOTAL_INSERT++;
                                    }
                                    else
                                    {

                                        ActivityCode ac = Service.GetById(ActivityCodeFind.Id);
                                        ac.Description = Description.ToString();
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
