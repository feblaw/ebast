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
using OfficeOpenXml;
using System.IO;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]

    public class NetworkNumberController : BaseController<NetworkNumber, INetworkNumberService, NetworkNumberViewModel, NetworkNumberModelForm, Guid>
    {
        private readonly IDepartementService departement;
        private readonly IProjectsService projects;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAccountNameService accountName;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly ConfigHelper _config;
        private readonly IUserHelper _userHelper;
        private readonly IUserProfileService _userProfile;

        public NetworkNumberController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            INetworkNumberService service, 
            IUserHelper userHelper, 
            IDepartementService departement, 
            IProjectsService projects, 
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment environment,
            ExcelHelper excel,
            ConfigHelper config,
            IUserProfileService userProfile,
            IAccountNameService accountName) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.departement = departement;
            this.projects = projects;
            this.userManager = userManager;
            this.accountName = accountName;
            _environment = environment;
            _excel = excel;
            _config = config;
            _userHelper = userHelper;
            _userProfile = userProfile;
        }

        public override IActionResult Create()
        {
            ViewBag.Departements = departement.GetAll().ToList();
            ViewBag.Projects = projects.GetAll().ToList();
            ViewBag.LineManagers = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.ProjectManagers = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.AccountNames = accountName.GetAll().ToList();

            Dictionary<string, bool> Status = new Dictionary<string, bool>();
            Status.Add("Active", false);
            Status.Add("Close", true);
            ViewBag.ListStatus = Status.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.Departements = departement.GetAll().ToList();
            ViewBag.Projects = projects.GetAll().ToList();
            ViewBag.LineManagers = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.ProjectManagers = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.AccountNames = accountName.GetAll().ToList();

            Dictionary<string, bool> Status = new Dictionary<string, bool>();
            Status.Add("Active", false);
            Status.Add("Close", true);
            ViewBag.ListStatus = Status.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var networkNumber = Service.GetById(id);
            ViewBag.Departements = departement.GetById(networkNumber.DepartmentId);
            ViewBag.Project = projects.GetById(networkNumber.ProjectId);
            ViewBag.LineManagers = _userHelper.GetUserProfile(networkNumber.LineManagerId);
            ViewBag.ProjectManagers = _userHelper.GetUserProfile(networkNumber.ProjectManagerId);
            ViewBag.AccountNames = accountName.GetById(networkNumber.AccountNameId);
            return base.Details(id);
        }

        protected override void CreateData(NetworkNumber item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(NetworkNumber item, NetworkNumberModelForm model)
        {
            item.Code = model.Code;
            item.ProjectId = model.ProjectId;
            item.DepartmentId = model.DepartmentId;
            item.LineManagerId = model.LineManagerId;
            item.ProjectManagerId = model.ProjectManagerId;
            item.AccountNameId = model.AccountNameId;
            item.Description = model.Description;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.IsClosed = model.IsClosed;
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
                                if (worksheet.Cells[row, 1].Value != null &&
                                    worksheet.Cells[row, 2].Value != null &&
                                    worksheet.Cells[row, 3].Value != null &&
                                    worksheet.Cells[row, 4].Value != null &&
                                    worksheet.Cells[row, 5].Value != null &&
                                    worksheet.Cells[row, 6].Value != null )
                                {
                                    var NetworkNumber = worksheet.Cells[row, 1].Value.ToString();
                                    var Project = worksheet.Cells[row, 2].Value.ToString();
                                    var Department = worksheet.Cells[row, 3].Value.ToString();
                                    var LineManager = worksheet.Cells[row, 4].Value.ToString();
                                    var ProjectManager = worksheet.Cells[row, 5].Value.ToString();
                                    var AccountName = worksheet.Cells[row, 6].Value.ToString();
                                    var Description = (worksheet.Cells[row, 7].Value!=null) ? worksheet.Cells[row, 7].Value.ToString() : "";

                                    NetworkNumber NetWorkFind = Service
                                       .GetAll()
                                       .FirstOrDefault(x => _excel.TruncateString(x.Code) == _excel.TruncateString(NetworkNumber));

                                    Projects ProjectFind = projects
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Project));

                                    Departement DepartmentFind = departement
                                      .GetAll()
                                      .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Department));

                                    UserProfile LineManagerUser = await FindUserByRole(LineManager, "Project Manager");
                                    UserProfile ProjectManagerUser = await FindUserByRole(ProjectManager, "Project Manager");

                                    AccountName AccountFind = accountName
                                     .GetAll()
                                     .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(AccountName.ToString()));


                                    if(ProjectFind==null)
                                    {
                                        Projects pj = new Projects { Name = Project };
                                        ProjectFind = projects.Add(pj);
                                    }

                                    if(LineManagerUser!=null && ProjectManagerUser!=null && DepartmentFind!=null && AccountName!=null)
                                    {
                                        if(NetWorkFind == null)
                                        {
                                           
                                            NetworkNumber nt = new NetworkNumber
                                            {
                                                Code = NetworkNumber,
                                                Project = ProjectFind,
                                                Departement = DepartmentFind,
                                                LineManager = LineManagerUser,
                                                ProjectManager = ProjectManagerUser,
                                                AccountName = AccountFind,
                                                Description = Description

                                            };
                                            Service.Add(nt);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {
                                            
                                            NetworkNumber nt = Service.GetById(NetWorkFind.Id);
                                            nt.Project = ProjectFind;
                                            nt.Departement = DepartmentFind;
                                            nt.LineManager = LineManagerUser;
                                            nt.ProjectManager = ProjectManagerUser;
                                            nt.AccountName = AccountFind;
                                            nt.Description = Description;
                                            Service.Update(nt);
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

        private async Task<UserProfile> FindUserByRole(string Email, string roleName)
        {
            ApplicationUser AppUser = await userManager.FindByEmailAsync(Email);
            List<string> RolesUser = userManager.GetRolesAsync(AppUser).Result.ToList();
            String CurrentRole = RolesUser.FirstOrDefault();
            UserProfile MyProfile = _userProfile.GetByUserId(AppUser.Id);
            if(AppUser==null)
            {
                var user = new ApplicationUser
                {
                    UserName = Email,
                    Email = Email,
                    UserProfile = new UserProfile
                    {
                        Name = Email,
                        IsActive = true,
                        Roles = roleName,
                        Email = Email
                    }
                };
                var defaultPassword = _config.GetConfig("user.default.password");
                var result = await userManager.CreateAsync(user, defaultPassword);
                if (result.Succeeded)
                {
                    string[] ListRoles = new string[] { roleName };
                    var resultRoles = userManager.AddToRolesAsync(user, ListRoles).Result;
                    if (resultRoles.Succeeded)
                    {
                        var code = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                        var confirm = userManager.ConfirmEmailAsync(user, code);
                        if (confirm.Result.Succeeded)
                        {
                            return user.UserProfile;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (_excel.TruncateString(CurrentRole) != _excel.TruncateString(roleName))
                {
                    string[] ListRoles = new string[] { roleName };
                    var currentRoles = userManager.GetRolesAsync(AppUser).Result;
                    var result = userManager
                        .RemoveFromRolesAsync(AppUser, currentRoles)
                        .Result;
                    if(result.Succeeded)
                    {
                        result = userManager
                           .AddToRolesAsync(AppUser, ListRoles)
                           .Result;
                        if(result.Succeeded)
                        {
                            UserProfile up = _userProfile.GetByUserId(AppUser.Id);
                            up.Roles = roleName;
                            _userProfile.Update(up);
                            return MyProfile;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return MyProfile;
                }
            }
        }


    }

}
