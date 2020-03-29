using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
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
    /// SUB ORGANIZATIONAL UNIT
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class DepartementSubController: BaseController<DepartementSub, IDepartementSubService, DepartementSubViewModel, DepartementSubModelForm, Guid>
    {
        private readonly IDepartementService departement;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly ConfigHelper _config;
        private readonly IUserHelper _userHelper;
        private readonly IUserProfileService userProfile;
        private readonly ITacticalResourceService _trcp;
        private readonly UserManager<ApplicationUser> _userManager;


        public DepartementSubController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IUserProfileService userProfile,
            IMapper mapper, 
            IDepartementSubService service,
            IUserHelper userHelper, 
            IDepartementService departement,
            IHostingEnvironment environment,
            ITacticalResourceService trcp,
            UserManager<ApplicationUser> userManager,
            ConfigHelper config,
            ExcelHelper excel) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.departement = departement;
            this.userProfile = userProfile;
            _environment = environment;
            _excel = excel;
            _config = config;
            _userHelper = userHelper;
            _userManager = userManager;
            _trcp = trcp;
        }

        public override IActionResult Create()
        {
            ViewBag.Departements = this.departement.GetAll().ToList();
            ViewBag.LineManagers = _userHelper.GetByRoleName("Head Of Service Line").ToList();
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.Departements = this.departement.GetAll().ToList();
            ViewBag.LineManagers = _userHelper.GetByRoleName("Head Of Service Line").ToList();
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var managerId = Service.GetById(id).LineManagerid;
            //ViewBag.currentManager = this.userManager.Users.Include(x => x.UserProfile).FirstOrDefault(x => x.Id == managerId);
            ViewBag.currentManager = this.userProfile.GetById(managerId);
            var departementId = Service.GetById(id).DepartmentId;
            ViewBag.currentDepartement = departement.GetAll().Single(x => x.Id == departementId);
            return base.Details(id);
        }

        protected override void CreateData(DepartementSub item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(DepartementSub item, DepartementSubModelForm model)
        {
            item.SubName = model.SubName;
            item.DsStatus = model.DsStatus;
            item.DepartmentId = model.DepartmentId;
            item.LineManagerid = model.LineManagerid;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
        }

        protected override void AfterCreateData(DepartementSub item)
        {
            _trcp.Add(new TacticalResource
            {
                Approved = 0,
                Forecast = 0,
                CountSrf = 0,
                DepartmentSubId = item.Id,
                OtherInfo = "HRMS"
            });

            _trcp.Add(new TacticalResource
            {
                Approved = 0,
                Forecast = 0,
                CountSrf = 0,
                DepartmentSubId = item.Id,
                OtherInfo = "NON HRMS"
            });
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
                                    worksheet.Cells[row, 3].Value != null)
                                {

                                    var OrganizationUnitName = worksheet.Cells[row, 1].Value;
                                    var HeadServiceLine = worksheet.Cells[row, 2].Value;
                                    var SubOrganizationUnitName = worksheet.Cells[row, 3].Value;

                                    Departement DepartmentFind = departement
                                      .GetAll()
                                      .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(OrganizationUnitName.ToString()));


                                    DepartementSub SubFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.SubName) == _excel.TruncateString(SubOrganizationUnitName.ToString()));

                                    ApplicationUser AppUser = await _userManager.FindByEmailAsync(HeadServiceLine.ToString());

                                    if (DepartmentFind != null && AppUser!=null)
                                    {
                                        var UserProfile = userProfile.GetByUserId(AppUser.Id);

                                        if (SubFind == null)
                                        {
                                            DepartementSub ds = new DepartementSub { Departement = DepartmentFind, LineManager = UserProfile, SubName = SubOrganizationUnitName.ToString() };
                                            Service.Add(ds);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {
                                            DepartementSub ds = Service.GetById(SubFind.Id);
                                            ds.Departement = DepartmentFind;
                                            ds.LineManager = UserProfile;
                                            ds.SubName = SubOrganizationUnitName.ToString();
                                            Service.Update(ds);
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
