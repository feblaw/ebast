using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using App.Services.Identity;
using App.Helper;
using AutoMapper;
using App.Web.Models.ViewModels.Identity;
using App.Services.Core.Interfaces;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;
        private readonly IUserHelper _userHelper;
        private readonly IMapper _mapper;
        private readonly FileHelper _fileHelper;
        private readonly ConfigHelper _config;
        private readonly string _cropSuffix;
        private readonly string _userDir;
        private readonly ICandidateInfoService _candidate;
        private readonly IDepartementService _department;
        private readonly IDepartementSubService _departmentSub;
        private readonly IVacancyListService _vacancy;
        private readonly IServicePackService _ssow;
        private readonly ICityService _city;
        private readonly ISrfRequestService _srf;


        public ProfileController(UserManager<ApplicationUser> userManager,
            IUserProfileService userProfileService,
            IUserHelper userHelper,
            ICandidateInfoService candidate,
            IDepartementService department,
            IDepartementSubService departmentSub,
            IVacancyListService vacancy,
            IServicePackService ssow,
            ICityService city,
            ISrfRequestService srf,
            IMapper mapper,
            FileHelper fileHelper,
            ConfigHelper config)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
            _userHelper = userHelper;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _config = config;
            _cropSuffix = _config.GetConfig("crop.suffix");
            _userDir = _config.GetConfig("user.upload.directory");
            _candidate = candidate;
            _department = department;
            _departmentSub = departmentSub;
            _vacancy = vacancy;
            _ssow = ssow;
            _city = city;
            _srf = srf;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var user = _userHelper.GetUser(User);

            var roles = _userManager
               .GetRolesAsync(user)
               .Result
               .ToList();

            if (User.IsInRole("Contractor"))
            {
                var srf = _userHelper.GetCurrentSrfByLogin(User);
                var ContractorInfo = _candidate.GetById(srf.CandidateId);
                var Vacancy = _vacancy.GetById(ContractorInfo.VacancyId);
                ViewBag.Srf = srf;
                ViewBag.ContractorInfo = ContractorInfo;
                ViewBag.Departement = _department.GetById(srf.DepartmentId);
                ViewBag.DepartmentSub = _departmentSub.GetById(srf.DepartmentSubId);
                ViewBag.LineManager = _userHelper.GetUserProfile(srf.ApproveOneId.Value);
                ViewBag.ProjectManager = _userHelper.GetUserProfile(srf.ProjectManagerId);
                ViewBag.SSOW = _ssow.GetById(Vacancy.ServicePackId);
                if (ContractorInfo.HomeBaseId.HasValue)
                {
                    ViewBag.HomeBase = _city.GetById(ContractorInfo.HomeBaseId).Name;
                }
                else
                {
                    ViewBag.HomeBase = "-";
                }
            }
            else
            {
                ViewBag.Srf = null;
                ViewBag.ContractorInfo = null;
                ViewBag.Departement = null;
                ViewBag.DepartmentSub = null;
                ViewBag.LineManager = null;
            }


            ViewBag.ModelForm = _mapper.Map<UpdateProfileForm>(user);
            ViewBag.CurretRoles = roles;
            return View(_mapper.Map<ApplicationUserViewModel>(user));
        }

        [AllowAnonymous]
        public IActionResult CheckSrf()
        {
            var user = _userHelper.GetUser(User);
            var html = "";
            if (user != null)
            {
                var Profile = _userProfileService.GetByUserId(user.Id);
                if (Profile == null)
                {
                    html = "GAK PUNYA PROFILE";
                }
                else
                {
                    var CheckCandidate = _candidate.GetAll().Where(x => x.AccountId.Equals(Profile.Id)).FirstOrDefault();
                    if (CheckCandidate == null)
                    {
                        html = "GAK ADA DI CANDIDATE dengan PROFILE ID " + Profile.Id;
                    }
                    else
                    {
                        var SrfCheck = _srf.GetAll().Where(x => x.CandidateId.Equals(CheckCandidate.Id)).FirstOrDefault();
                        var Vacancy = _vacancy.GetById(CheckCandidate.VacancyId);
                        if (SrfCheck == null)
                        {
                            html = "GAK ADA DI SRF dengan Candidate ID " + CheckCandidate.Id + " DAN VACANCY ID " + Vacancy.Id;
                        }
                        else
                        {
                            html = "ADA DI SRF dengan ID " + SrfCheck.Id + " DAN CANDIATE ID " + CheckCandidate.Id + " USER PROFILE " + Profile.Id + " DAN VACANCY ID " + CheckCandidate.VacancyId;
                        }
                    }
                }
            }
            else
            {
                html = "USER TIDAK DITEMUKAN";
            }
            return Content(html);
        }

        public IActionResult UserNotInCandidate()
        {
            var Candidate = _candidate.GetAll().Select(x => x.AccountId).ToList();
            var UserProfile = _userProfileService.GetAll().Where(x => !Candidate.Contains(x.Id) && x.Roles.Trim().ToLower() == "Contractor".Trim().ToLower()).Select(x => x.Id).ToList();
            return Content(JsonConvert.SerializeObject(UserProfile));
        }

        public IActionResult UserNotHaveSrf()
        {
            var srf = _srf.GetAll().Select(x => x.CandidateId).ToList();
            var Candidate = _candidate.GetAll().Where(x => !srf.Contains(x.Id)).Select(x => x.AccountId).ToList();
            var UserProfile = _userProfileService.GetAll().Where(x => Candidate.Contains(x.Id) && x.Roles.Trim().ToLower() == "Contractor".Trim().ToLower()).Select(x => x.Id).ToList();
            return Content(JsonConvert.SerializeObject(UserProfile));
        }

        public IActionResult UpdateProfile()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileForm model)
        {
            var user = _userHelper.GetUser(User);
            if (!ModelState.IsValid)
            {
                ViewBag.ModelForm = model;
                ModelState.AddModelError("updateError", "");
                return View("Index", _mapper.Map<ApplicationUserViewModel>(user));
            }


            user.UserProfile.Name = model.Name;
            user.UserProfile.Birthplace = model.Birthplace;
            user.UserProfile.Birthdate = model.Birthdate;
            user.UserProfile.Address = model.Address;
            user.UserProfile.Photo = model.Photo;
            user.PhoneNumber = model.Phone;
            user.UserProfile.MobilePhoneNumber = model.MobilePhoneNumber;
            user.UserProfile.HomePhoneNumber = model.HomePhoneNumber;
            user.UserProfile.Description = model.Description;
            user.Email = model.Email;
            user.UserProfile.Email = model.Email;
            
            if(string.IsNullOrWhiteSpace(model.Username))
            {
                user.UserName = model.Email;
            }

            var result = await _userManager.UpdateAsync(user);

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
                        result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            TempData["Success"] = true;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddError(result.Errors, "updateError");
                        }
                    }
                }

                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Success"] = false;
                AddError(result.Errors, "updateError");
            }

            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordForm model)
        {
            var user = _userHelper.GetUser(User);

            if (!ModelState.IsValid)
            {
                TempData["PasswordSuccess"] = false;
                ModelState.AddModelError("passwordError", "");
                ViewBag.ModelForm = _mapper.Map<UpdateProfileForm>(user);
                return View("Index", _mapper.Map<ApplicationUserViewModel>(user));
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = true;
                return RedirectToAction("Index");
            }

            AddError(result.Errors, "passwordError");

            TempData["PasswordSuccess"] = false;
            ViewBag.ModelForm = _mapper.Map<UpdateProfileForm>(user);
            return View("Index", _mapper.Map<ApplicationUserViewModel>(user));
        }

        private void AddError(IEnumerable<IdentityError> errors, string key = "")
        {
            var errorString = "";
            var first = true;
            var last = errors.Last();
            foreach (var error in errors)
            {
                if (!first)
                {
                    errorString += "<li>";
                }

                errorString += $"{error.Description}";

                if (error.Code != last.Code)
                {
                    errorString += "</li>";
                }

                first = false;
            }
            ModelState.AddModelError(key, errorString);
        }
    }
}
