using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Identity;
using App.Services.Identity;
using App.Web.Models.ViewModels.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using App.Helper;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using ImageSharp.Drawing.Paths;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FileIO = System.IO.File;
using OfficeOpenXml;
using System.Text;
using App.Services.Core.Interfaces;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Web.Models.ViewModels.Core.Business;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Identity
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class UserManagementController : BaseController<ApplicationUser, IUserService, ApplicationUserViewModel, ApplicationUserForm, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly FileHelper _fileHelper;
        private readonly ConfigHelper _config;
        private readonly HostConfiguration _hostConfiguration;
        private const string _userDir = "uploads/user/";
        private const string _cropSuffix = "_crop";
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly NotifHelper _notifHelper;
        private readonly IASPService _asp;
 

        public UserManagementController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IUserService service,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IASPService asp,
            SignInManager<ApplicationUser> signInManager,
            FileHelper fileHelper,
            ConfigHelper config,
            ExcelHelper excel,
            IUserHelper userHelper,
            MailingHelper mail,
            IOptions<HostConfiguration> hostConfiguration,
            NotifHelper notifHelper,
            IHostingEnvironment environment) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _asp = asp;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileHelper = fileHelper;
            _config = config;
            _hostConfiguration = hostConfiguration.Value;
            _environment = environment;
            _signInManager = signInManager;
            _excel = excel;
            _notifHelper = notifHelper;
        }

       
        public override IActionResult Index()
        {
            ViewBag.Roles = JsonConvert.SerializeObject(_roleManager.Roles.ToList()
                .Select(x=> new { label = x.Name , value= x.Name })
                .Where(x=>x.value!="Contractor")
                .OrderBy(x=>x.value));
            //var NetworkNumber = _network.GetAll().Where(x => x.DepartmentId.Equals(Id) && x.IsClosed == false).ToList();

            ViewBag.ASP = JsonConvert.SerializeObject(_asp.GetAll().ToList()
                .Select(x => new { label = x.Name, value = x.Name })
                .Where(x => x.value != "Contractor")
                .OrderBy(x => x.value));
            return base.Index();
        }

        public override IActionResult Details(string id)
        {
            var user = UserService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = Mapper.Map<ApplicationUserViewModel>(user);

            model.Roles = _userManager
                .GetRolesAsync(user)
                .Result
                .ToList();

            if (user.UserProfile.IsActive == true)
            {
                ViewBag.Status = "Active";
            }
            else
            {
                ViewBag.Status = "Close";
            }
            

            return View(model);
        }

        public override IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.Company = _asp.GetAll().ToList();

            var model = new ApplicationUserForm()
            {
                Roles = new List<string>()
            };
            var model2 = new ApplicationUserForm()
            {
                Company = new List<string>()
            };

            return View(model);
        }

        [HttpPost]
        public override IActionResult Create(ApplicationUserForm model)
        {
            if (ModelState.IsValid)
            {
                //var defaultPassword = _config.GetConfig("user.default.password");
                var user = Mapper.Map<ApplicationUser>(model);

                user.UserProfile = new UserProfile();
                user.UserProfile.Name = model.Name;
                user.UserProfile.Email = model.Email;
                user.UserProfile.UserName = model.Username;
                user.UserProfile.IsBlacklist = false;
                user.UserProfile.IsTerminate = false;
                user.UserProfile.ASPId = model.ASPId;
                
   

                if (model.Status.Equals("1"))
                {
                    user.UserProfile.IsActive = true;
                }
                if (model.Status.Equals("0"))
                {
                    user.UserProfile.IsActive = false;
                }

               
                if (model.Roles.Count() > 1)
                {
                    var Roles = String.Join(", ", model.Roles.ToArray());
                    user.UserProfile.Roles = Roles;
                }
                else
                {
                    user.UserProfile.Roles = model.Roles.FirstOrDefault();
                }
              


                var result = _userManager.CreateAsync(user, model.Password).Result;

                if (result.Succeeded)
                {
                    result = _userManager.AddToRolesAsync(user, model.Roles).Result;

                    if (result.Succeeded)
                    {

                        var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                        var confirm =  _userManager.ConfirmEmailAsync(user, code);
                        if(confirm.Result.Succeeded)
                        {
                            TempData["Success"] = "Success";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddError(confirm.Result.Errors);
                        }

                    }
                    else
                    {
                        AddError(result.Errors);
                    }
                }
                else
                {
                    AddError(result.Errors);
                }
            }

            ViewBag.Roles = _roleManager.Roles.ToList();

            return View(model);
        }

        public override IActionResult Edit(string id)
        {
            try
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                ViewBag.Company = _asp.GetAll().ToList();

                var user = UserService.GetById(id);

                if (user == null)
                {
                    return NotFound();
                }

                var model = Mapper.Map<ApplicationUserForm>(user);

                model.Roles = _userManager
                    .GetRolesAsync(user)
                    .Result
                    .ToList();



                model.ASPId = user.UserProfile.ASPId;

                if (user.UserProfile.IsActive == true)
                {
                    model.Status = "1";
                }
                else
                {
                    model.Status = "0";
                }


                return View(model);
            }
            catch(Exception e)
            {
                return Content("" + e.ToString());
            }
        }

        [HttpPost]
        public override IActionResult Edit(string id, ApplicationUserForm model)
        {
            if (ModelState.IsValid)
            {
                var user = UserService.GetById(id);
                user.Email = model.Email;

                if (user == null)
                {
                    return NotFound();
                }

                if (model.Status.Equals("1"))
                {
                    user.UserProfile.IsActive = true;
                }
                if (model.Status.Equals("0"))
                {
                    user.UserProfile.IsActive = false;
                }

                if (model.Roles.Count() > 1)
                {
                    var Roles = String.Join(", ", model.Roles.ToArray());
                    user.UserProfile.Roles = Roles;
                }
                else
                {
                    user.UserProfile.Roles = model.Roles.FirstOrDefault();
                }

                user.UserProfile.Email = model.Email;
                user.UserProfile.UserName = model.Username;
                user.UserProfile.ASPId = model.ASPId;
                user.Update(model);

                var result = _userManager.UpdateAsync(user).Result;

                if (result.Succeeded)
                {
                    var currentRoles = _userManager
                        .GetRolesAsync(user)
                        .Result;

                    result = _userManager
                        .RemoveFromRolesAsync(user, currentRoles)
                        .Result;

                    if (result.Succeeded)
                    {
                        result = _userManager
                            .AddToRolesAsync(user, model.Roles)
                            .Result;

                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrWhiteSpace(model.Photo) && (model.Photo != user.UserProfile.Photo))
                            {
                                var attachment = model.Photo.ConvertToAttachment();
                                if (attachment != null)
                                {
                                    var newPath = $"{_userDir}/{user.Id}";
                                    attachment.CropedPath = _fileHelper.CreateCropped(attachment);
                                    attachment.Path = _fileHelper.FileMove(attachment.Path,
                                        newPath,
                                        user.Id);
                                    attachment.CropedPath = _fileHelper.FileMove(attachment.CropedPath,
                                        newPath,
                                        user.Id + _cropSuffix);
                                    user.UserProfile.Photo = attachment.ConvertToString();
                                    result = _userManager.UpdateAsync(user).Result;

                                    if (result.Succeeded)
                                    {
                                        TempData["Success"] = "Success";
                                        return RedirectToAction("Details", new { id });
                                    }
                                    else
                                    {
                                        AddError(result.Errors);
                                    }
                                }
                            }
                            

                            if(!string.IsNullOrWhiteSpace(model.Password))
                            {
                                var Code =  _userManager.GeneratePasswordResetTokenAsync(user).Result;
                                var resetPassword = _userManager.ResetPasswordAsync(user, Code, model.Password).Result;
                            }

                            TempData["Success"] = "Success";
                            //return RedirectToAction("Details", new { id });
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddError(result.Errors);
                        }
                    }
                    else
                    {
                        AddError(result.Errors);
                    }
                }
                else
                {
                    AddError(result.Errors);
                }
            }

            ViewBag.Roles = _roleManager.Roles.ToList();

            return View(model);
        }


        [HttpGet]
        public override IActionResult Import()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
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
                                // Checking If Exists
                                if (worksheet.Cells[row, 1].Value != null && 
                                    worksheet.Cells[row, 2].Value != null &&
                                    worksheet.Cells[row, 3].Value != null &&
                                    worksheet.Cells[row, 4].Value != null 
                                 )
                                {
                                    var Name = worksheet.Cells[row, 1].Value;
                                    var Email = worksheet.Cells[row, 2].Value;
                                    var Roles = worksheet.Cells[row, 3].Value;
                                    var Status = worksheet.Cells[row, 4].Value;

                                    bool sStatus = false;
                                    if (_excel.TruncateString(Status.ToString()) == _excel.TruncateString("Active"))
                                    {
                                        sStatus = true;
                                    }

                                    if(Roles!=null)
                                    {
                                        string[] UserRoles = Roles.ToString().Split(',');
                                        ApplicationUser AppUser = await _userManager.FindByEmailAsync(Email.ToString());

                                        if (AppUser == null)
                                        {
                                            var user = new ApplicationUser
                                            {
                                                UserName = Email.ToString(),
                                                Email = Email.ToString(),
                                                UserProfile = new UserProfile
                                                {
                                                    Name = Name.ToString(),
                                                    IsActive = sStatus,
                                                    Roles = Roles.ToString(),
                                                    Email = Email.ToString(),
                                                    UserName = Email.ToString(),
                                                    IsBlacklist = false,
                                                    IsTerminate = false,
                                        }
                                            };
                                            var defaultPassword = _config.GetConfig("user.default.password");
                                            var result = await _userManager.CreateAsync(user, defaultPassword);
                                            if (result.Succeeded)
                                            {

                                                var resultRoles = _userManager.AddToRolesAsync(user, UserRoles).Result;
                                                if (resultRoles.Succeeded)
                                                {
                                                    var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                                                    var confirm = _userManager.ConfirmEmailAsync(user, code);
                                                    if (confirm.Result.Succeeded)
                                                    {
                                                        TOTAL_INSERT++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var UpdateUser = UserService.GetById(AppUser.Id);
                                            UpdateUser.UserProfile.IsActive = sStatus;
                                            UpdateUser.UserProfile.Roles = Roles.ToString();
                                            UpdateUser.UserProfile.Name = Name.ToString();
                                            UserService.Update(UpdateUser);

                                            var result = _userManager.UpdateAsync(UpdateUser).Result;

                                            if (result.Succeeded)
                                            {
                                                var currentRoles = _userManager
                                                    .GetRolesAsync(UpdateUser)
                                                    .Result;

                                                result = _userManager
                                                     .RemoveFromRolesAsync(UpdateUser, currentRoles)
                                                     .Result;

                                                if (result.Succeeded)
                                                {
                                                    result = _userManager
                                                      .AddToRolesAsync(UpdateUser, UserRoles)
                                                      .Result;
                                                    TOTAL_UPDATE++;
                                                }

                                            }


                                        }

                                    }
                                    
                                    
                                }

                            }
                        }
                        
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            TempData["Messages"] = "Total Inserted = "+TOTAL_INSERT+" , Total Updated = "+TOTAL_UPDATE;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }

    }

}
