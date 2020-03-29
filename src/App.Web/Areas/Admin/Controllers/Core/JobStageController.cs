using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
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
    public class JobStageController : BaseController<JobStage, IJobStageService, JobStageViewModel, JobStageModelForm, Guid>
    {
        private readonly IJobStageService _jobStageService;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public JobStageController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IJobStageService service, 
            IUserHelper userHelper,
            IHostingEnvironment environment,
            ExcelHelper excel,
            IJobStageService jobStageService) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this._jobStageService = jobStageService;
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        protected override void CreateData(JobStage item)
        {
            item.Stage = item.Stage.ToUpper();
            item.Description = item.Description.ToUpper();
            item.Status= item.Status;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }
        protected override void UpdateData(JobStage item, JobStageModelForm model)
        {
            item.Stage = model.Stage.ToUpper();
            item.Description = model.Description.ToUpper();
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
                                   
                                    var StageName = worksheet.Cells[row, 1].Value;
                                    var Description = worksheet.Cells[row, 2].Value;
                                    JobStage JobStageFind = _jobStageService.GetAll()
                                        .Where(x => _excel.TruncateString(x.Stage) == _excel.TruncateString(StageName.ToString()))
                                        .FirstOrDefault();
                                    if (JobStageFind == null)
                                    {
                                        JobStage jb = new JobStage { Stage = StageName.ToString() , Description = Description.ToString() };
                                        _jobStageService.Add(jb);
                                        TOTAL_INSERT++;
                                    }
                                    else
                                    {
                                        JobStage jb = _jobStageService.GetById(JobStageFind.Id);
                                        jb.Description = Description.ToString().ToUpper();
                                        _jobStageService.Update(jb);
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
