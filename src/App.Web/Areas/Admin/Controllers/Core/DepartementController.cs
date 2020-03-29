using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    /// <summary>
    /// Organizational Unit
    /// </summary>
    /// 
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class DepartementController : BaseController<Departement, IDepartementService, DepartementViewModel, DepartementModelForm, Guid>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserProfileService userProfile;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly ConfigHelper _config;
        private readonly IUserHelper _userHelper;
        private readonly ITacticalResourceService _trcp;

        public DepartementController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IDepartementService service, 
            IUserHelper userHelper,
            IHostingEnvironment environment,
            ITacticalResourceService trcp,
            ExcelHelper excel,
            ConfigHelper config,
            UserManager<ApplicationUser> userManager,
            IUserProfileService userProfile) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {

            this.userManager = userManager;
            this.userProfile = userProfile;
            _environment = environment;
            _excel = excel;
            _config = config;
            _userHelper = userHelper;
            _trcp = trcp;
        }

      

        public override IActionResult Edit(Guid id)
        {
            var managerId = Service.GetById(id).HeadId;
            ViewBag.CurrentUser = this.userProfile.GetById(managerId);
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var managerId = Service.GetById(id).HeadId;
            ViewBag.CurrentUser = this.userProfile.GetById(managerId);
            return base.Details(id);
        }

        protected override void CreateData(Departement item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }


        protected override void UpdateData(Departement item, DepartementModelForm model)
        {
            item.Name = model.Name;
            item.OperateOrNon = model.OperateOrNon;
            item.Description = model.Description;
            item.HeadId = model.HeadId;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void AfterCreateData(Departement item)
        {
            _trcp.Add(new TacticalResource
            {
                Approved = 0,
                Forecast = 0,
                CountSrf = 0,
                DepartmentId = item.Id,
                OtherInfo = "HRMS"
            });

            _trcp.Add(new TacticalResource
            {
                Approved = 0,
                Forecast = 0,
                CountSrf = 0,
                DepartmentId = item.Id,
                OtherInfo = "NON HRMS"
            });
        }

        private UserProfile MyProfile(string id)
        {
            return userProfile.GetByUserId(id);
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
                                if (worksheet.Cells[row, 1].Value != null &&
                                    worksheet.Cells[row, 2].Value != null &&
                                    worksheet.Cells[row, 3].Value != null )
                                {
                                    var OrganizationName = worksheet.Cells[row, 1].Value;
                                    var Status = worksheet.Cells[row, 2].Value;
                                    var HeadOfOrganization = worksheet.Cells[row, 3].Value;
                                    var Description = (worksheet.Cells[row, 4].Value!=null) ? worksheet.Cells[row, 4].Value : "No Description";

                                    Departement FindDept = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(OrganizationName.ToString()));

                                    ApplicationUser AppUser = await userManager.FindByEmailAsync(HeadOfOrganization.ToString());
                                    int intStatus = 0;
                                    if(_excel.TruncateString(Status.ToString()) == _excel.TruncateString("Operational"))
                                    {
                                        intStatus = 1;
                                    }
                                    if(AppUser!=null)
                                    {
                                        if (FindDept == null)
                                        {
                                            Departement dp = new Departement
                                            {
                                                Name = OrganizationName.ToString(),
                                                OperateOrNon = intStatus,
                                                Head = MyProfile(AppUser.Id),
                                                Description = Description.ToString()
                                            };
                                            Service.Add(dp);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {
                                            Departement dp = Service.GetById(FindDept.Id);
                                            dp.Name = OrganizationName.ToString();
                                            dp.OperateOrNon = intStatus;
                                            dp.Head = MyProfile(AppUser.Id);
                                            dp.Description = Description.ToString();
                                            Service.Update(dp);
                                            TOTAL_UPDATE++;
                                        }
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
