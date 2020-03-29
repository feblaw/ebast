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
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Identity;
using App.Domain.Models.Identity;
using Newtonsoft.Json;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class ContractorDataController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {

        private readonly IUserHelper _userHelper;
        private readonly ICandidateInfoService _contractor;
        private readonly IUserProfileService _userProfile;
        private readonly IServicePackService _servicePack;
        private readonly IServicePackCategoryService _servicePackCategory;
        private readonly ICityService _city;
        private readonly IVacancyListService _vacancy;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPackageTypeService _packageType;
        private readonly ConfigHelper _config;

        public ContractorDataController
            (IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            ConfigHelper config,
            IMapper mapper, 
            ISrfRequestService service,
            ICandidateInfoService contractor,
            IUserProfileService userProfile,
            IServicePackService servicePack,
            IVacancyListService vacancy,
            IServicePackCategoryService servicePackCategory,
            UserManager<ApplicationUser> userManager,
            ICityService city,
            IPackageTypeService packageType,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userHelper = userHelper;
            _contractor = contractor;
            _userProfile = userProfile;
            _servicePack = servicePack;
            _servicePackCategory = servicePackCategory;
            _city = city;
            _userManager = userManager;
            _vacancy = vacancy;
            _packageType = packageType;
            _config = config;
        }

        [HttpGet]
        public override IActionResult Edit(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var Contractor = _contractor.GetById(item.CandidateId);
                var Vacancy = _vacancy.GetById(Contractor.VacancyId);
                var PackageType = _packageType.GetById(Vacancy.PackageTypeId);
                var Ssow = _servicePack.GetById(item.ServicePackId);
                var UserProfile = _userProfile.GetById(Contractor.AccountId);
                if (item == null)
                {
                    return NotFound();
                }
                else
                {
                    var GenderOption = from Gender g in Enum.GetValues(typeof(Gender)) select new { Id = (int)g, Name = g.ToString() };
                    var MartialOpt = from Martial g in Enum.GetValues(typeof(Martial)) select new { Id = (int)g, Name = g.ToString() };
                    var PricelistType = from PackageTypes g in Enum.GetValues(typeof(PackageTypes)) select new { Id = (int)g, Name = g.ToString() };
                    var User = _userManager.FindByIdAsync(UserProfile.ApplicationUserId).Result;

                    // Dropdown List
                    ViewBag.ServiceWorkPackage = _servicePackCategory.GetAll().ToList();
                    ViewBag.HomeBase = _city.GetAll().ToList();
                    ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
                    ViewBag.ProjectManager = _userHelper.GetByRoleName("Project Manager").ToList();
                    ViewBag.Gender = GenderOption.ToList();
                    ViewBag.Martial = MartialOpt.ToList();
                    ViewBag.PricelistType = PricelistType.ToList();
                    ViewBag.ContractorId = item.CandidateId;
                    ViewBag.UserProfile = UserProfile;


                    // Set Data
                    var PackageTy = (PackageTypes)Enum.Parse(typeof(PackageTypes), PackageType.Name);
                    ContractorModelForm model = new ContractorModelForm();
                    model.AhID = UserProfile.AhId;
                    model.SrfNumber = item.Number;
                    model.ContrctorName = Contractor.Name;
                    model.PricelistType = PackageTy;
                    model.ServicePackId = item.ServicePackId;
                    model.ServicePackCategoryId = Ssow.ServicePackCategoryId;
                    model.SrfBegin = item.SrfBegin.Value;
                    model.SrfEnd = item.SrfEnd.Value;

                    if (Contractor.HomeBaseId.HasValue)
                    {
                        model.HomeBaseId = Contractor.HomeBaseId.Value;
                    }

                    model.LineManagerId = item.ApproveOneId.Value;
                    model.ProjectManagerId = item.ProjectManagerId;
                    model.HomePhoneNumber = Contractor.HomePhoneNumber;
                    model.MobilePhoneNumber = Contractor.MobilePhoneNumber;
                    model.IdNumber = Contractor.IdNumber;
                    model.Email = User.Email;
                    model.Nationality = Contractor.Nationality;
                    model.DateOfBirth = Contractor.DateOfBirth;
                    model.PlaceOfBirth = Contractor.PlaceOfBirth;
                    model.Address = Contractor.Address;
                    model.Gender = Contractor.Gender;
                    model.Martial = Contractor.Martial.Value;
                    model.ApplicationUserId = User.Id;
                    model.Username = User.UserName;
                    model.Notes = UserProfile.Description;
                    model.SrfId = id;
                    return View(model);
                }
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }
        }

        public override IActionResult Details(Guid id)
        {
            var item = Service.GetById(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
           
                var Contractor = _contractor.GetById(item.CandidateId);
                var Vacancy = _vacancy.GetById(Contractor.VacancyId);
                var PackageType = _packageType.GetById(Vacancy.PackageTypeId);
                var Ssow = _servicePack.GetById(item.ServicePackId);
                var UserProfile = _userProfile.GetById(Contractor.AccountId);
                var PackageTy = (PackageTypes)Enum.Parse(typeof(PackageTypes), PackageType.Name);
                var User = _userManager.FindByIdAsync(UserProfile.ApplicationUserId).Result;
                ViewBag.ContractorId = item.CandidateId;
                ViewBag.ServiceCategory = _servicePackCategory.GetById(Ssow.ServicePackCategoryId).Name;
                ViewBag.ServicePack = Ssow.Name;
               
                ViewBag.ProjectManager = _userProfile.GetById(item.ProjectManagerId).Name;
                ViewBag.LineManager = _userProfile.GetById(item.ApproveOneId).Name;


                // Set Data
                ContractorModelForm model = new ContractorModelForm();
                model.AhID = UserProfile.AhId;
                model.SrfNumber = "e-EID/KI-"+(item.CreatedAt.Value).ToString("yy") +":SRF: "+item.Number+" UEN";
                model.ContrctorName = Contractor.Name;
                model.PricelistType = PackageTy;
                model.ServicePackId = item.ServicePackId;
                model.ServicePackCategoryId = Ssow.ServicePackCategoryId;
                model.SrfBegin = item.SrfBegin.Value;
                model.SrfEnd = item.SrfEnd.Value;

                if (Contractor.HomeBaseId.HasValue)
                {
                    model.HomeBaseId = Contractor.HomeBaseId.Value;
                    ViewBag.HomeBase = _city.GetById(Contractor.HomeBaseId).Name;
                }
                else
                {
                    ViewBag.HomeBase = null;
                }

                model.LineManagerId = item.ApproveOneId.Value;
                model.ProjectManagerId = item.ProjectManagerId;
                model.HomePhoneNumber = Contractor.HomePhoneNumber;
                model.MobilePhoneNumber = Contractor.MobilePhoneNumber;
                model.IdNumber = Contractor.IdNumber;
                model.Email = User.Email;
                model.Nationality = Contractor.Nationality;
                model.DateOfBirth = Contractor.DateOfBirth;
                model.PlaceOfBirth = Contractor.PlaceOfBirth;
                model.Address = Contractor.Address;
                model.Gender = Contractor.Gender;
                model.Martial = Contractor.Martial.Value;
                model.ApplicationUserId = User.Id;
                model.Username = User.UserName;
                model.Notes = UserProfile.Description;
                model.SrfId = id;
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult Update(ContractorModelForm model)
        {
            var Srf = Service.GetById(model.SrfId);
            if (ModelState.IsValid && Srf!=null)
            {
                var Candidate = _contractor.GetById(Srf.CandidateId);
                var UserProfile = _userProfile.GetById(Candidate.AccountId.Value);

                if (model.IdNumber.Length != 16)
                {
                    TempData["Error"] =  "Invalid ID Number / (KTP) "+model.IdNumber+" must be 16 digits and numeric";
                    return RedirectToAction("Edit", new { id = Srf.Id });
                }
                //else
                //{
                //    if(UserProfile!=null && User.IsInRole("Administrator"))
                //    {
                //        var CheckNumber = _userProfile.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber == model.IdNumber && x.Id != Candidate.AccountId.Value).FirstOrDefault();
                //        if (CheckNumber != null)
                //        {
                //            TempData["Error"] = "Invalid ID Number / (KTP) " + model.IdNumber + " has exists with other resource !!";
                //            return RedirectToAction("Edit", new { id = Srf.Id });
                //        }
                      
                //    }
                //}

                UserProfile.AhId = model.AhID;
                UserProfile.Name = model.ContrctorName;
                UserProfile.MobilePhoneNumber = model.MobilePhoneNumber;
                UserProfile.HomePhoneNumber = model.HomePhoneNumber;
                UserProfile.IdNumber = model.IdNumber;
                UserProfile.Email = model.Email;
                UserProfile.Birthplace = model.PlaceOfBirth;
                UserProfile.Birthdate = model.DateOfBirth;
                UserProfile.Address = model.Address;
                UserProfile.UserName = model.Username;
                UserProfile.Description = model.Notes;
                _userProfile.Update(UserProfile);

                Candidate.Name = model.ContrctorName;
                Candidate.HomeBaseId = model.HomeBaseId;
                Candidate.HomePhoneNumber = model.HomePhoneNumber;
                Candidate.MobilePhoneNumber = model.MobilePhoneNumber;
                Candidate.IdNumber = model.IdNumber;
                Candidate.Email = model.Email;
                Candidate.Nationality = model.Nationality;
                Candidate.PlaceOfBirth = model.PlaceOfBirth;
                Candidate.DateOfBirth = model.DateOfBirth;
                Candidate.Address = model.Address;
                Candidate.Gender = model.Gender;
                Candidate.Martial = model.Martial;
                Candidate.Email = model.Email;
                Candidate.Description = model.Notes;
                _contractor.Update(Candidate);

                // Update Vacancy Status
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                Vacancy.ServicePackId = model.ServicePackId;
                Vacancy.ApproverOneId = model.LineManagerId;
                if (model.PricelistType == PackageTypes.A)
                {
                    Vacancy.PackageType = _packageType.GetAll().Where(x => x.Name.Trim().ToLower().Equals("A".Trim().ToLower())).FirstOrDefault();
                }
                else if (model.PricelistType == PackageTypes.B)
                {
                    Vacancy.PackageType = _packageType.GetAll().Where(x => x.Name.Trim().ToLower().Equals("B".Trim().ToLower())).FirstOrDefault();
                }
                else
                {
                    Vacancy.PackageType = _packageType.GetAll().Where(x => x.Name.Trim().ToLower().Equals("FSO".Trim().ToLower())).FirstOrDefault();
                }
                _vacancy.Update(Vacancy);

                // Update SRF
                Srf.Number = model.SrfNumber;
                Srf.ServicePackId = model.ServicePackId;
                Srf.SrfBegin = model.SrfBegin;
                Srf.SrfEnd = model.SrfEnd;
                Srf.LineManagerId = model.LineManagerId;
                Srf.ProjectManagerId = model.ProjectManagerId;

                var NewAnnual = (Srf.SrfEnd.Value.Month - Srf.SrfBegin.Value.Month) <= 0 ? 0 : (Srf.SrfEnd.Value.Month - Srf.SrfBegin.Value.Month);
                if (Srf.Type == SrfType.Extension)
                {
                    Srf.AnnualLeave = Srf.AnnualLeave + NewAnnual;
                }
                else
                {
                    Srf.AnnualLeave = NewAnnual;
                }

                Service.Update(Srf);

                // Update ASP NET USERS
                var CurrentUser = _userManager.FindByIdAsync(model.ApplicationUserId).Result;
                CurrentUser.Email = model.Email;
                CurrentUser.UserName = model.Username;
                var Result = _userManager.UpdateAsync(CurrentUser).Result;
                
                if(!Result.Succeeded)
                {
                    TempData["Error"] = JsonConvert.SerializeObject(Result.Errors);
                    return RedirectToAction("Edit", new { id = Srf.Id });
                }

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var Code = _userManager.GeneratePasswordResetTokenAsync(CurrentUser).Result;
                    var resetPassword = _userManager.ResetPasswordAsync(CurrentUser, Code, model.Password).Result;

                    if (!resetPassword.Succeeded)
                    {
                        TempData["Error"] = JsonConvert.SerializeObject(resetPassword.Errors);
                        return RedirectToAction("Edit", new { id = Srf.Id });
                    }
                }

                TempData["Success"] = "OK";
                return RedirectToAction("Details", new { id = Srf.Id });

            }

            return RedirectToAction("Edit", new { id = Srf.Id });
        }

        [HttpPost]
        public IActionResult ApproveTerminate(string id, string notes, int submit)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item != null)
            {
                var Candidate = _contractor.GetById(item.CandidateId);
                var UserProfile = _userProfile.GetById(Candidate.AccountId);
                item.TeriminateNote = notes;
                item.TerimnatedBy = _userHelper.GetLoginUser(User).Name;
                item.TerminatedDate = DateTime.Now;
                if (submit == 3)
                {
                    item.Status = SrfStatus.Terminate;
                    UserProfile.IsTerminate = true;
                    _userProfile.Update(UserProfile);
                }
                else
                {
                    item.Status = SrfStatus.Blacklist;
                    UserProfile.IsBlacklist = true;
                    _userProfile.Update(UserProfile);
                }
                Service.Update(item);
                TempData["Terminated"] = "OK";
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult ResetAccount(Guid id)
        {
            var Srf = Service.GetById(id);
            var Candidate = _contractor.GetById(Srf.CandidateId);
            if(Candidate.IsUser == true)
            {
                var Profile = _userProfile.GetById(Candidate.AccountId);
                var AppUser = _userManager.FindByIdAsync(Profile.ApplicationUserId).Result;
                if(AppUser!=null)
                {
                    var PassWord = _config.GetConfig("user.default.password");
                    if (!string.IsNullOrWhiteSpace(PassWord))
                    {
                        var Code = _userManager.GeneratePasswordResetTokenAsync(AppUser).Result;
                        var resetPassword = _userManager.ResetPasswordAsync(AppUser, Code, PassWord).Result;
                        TempData["Success"] = "OK";
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Terimanted(string id, string TerminatedDate)
        {
            if (!string.IsNullOrWhiteSpace(TerminatedDate))
            {
                var srf = Service.GetById(Guid.Parse(id));
                if (srf != null)
                {
                    var DateTerminated = DateTime.Parse(TerminatedDate);
                    if (DateTime.Now.Date == DateTerminated.Date)
                    {
                        // Update SRF To Terminate
                        srf.TeriminateNote = "Terminate Resource";
                        srf.TerimnatedBy = _userHelper.GetLoginUser(User).Name;
                        srf.TerminatedDate = DateTerminated;
                        Service.Update(srf);

                        // Update User To Terminate
                        var Candidate = _contractor.GetById(srf.CandidateId);
                        if (Candidate != null && Candidate.AccountId.HasValue)
                        {
                            var UserProfile = _userProfile.GetById(Candidate.AccountId);
                            UserProfile.IsActive = false;
                            UserProfile.IsTerminate = true;
                            UserProfile.IsBlacklist = false;
                            _userProfile.Update(UserProfile);
                        }

                    }
                    else
                    {
                        srf.TerminatedDate = DateTerminated;
                        Service.Update(srf);
                    }
                }
            }
            return RedirectToAction("Index");
        }


    }
}
