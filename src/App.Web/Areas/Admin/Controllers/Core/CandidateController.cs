using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using FileIO = System.IO.File;
using System.IO;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "HR Agency,Line Manager,Sourcing,Administrator")]
    public class CandidateController : BaseController<CandidateInfo, ICandidateInfoService, CandidateInfoViewModel, CandidateInfoModelForm, Guid>
    {
        private readonly IVacancyListService _vacancy;
        private readonly IUserProfileService _user;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly FileHelper _fileHelper;
        private readonly ConfigHelper _config;
        private readonly IUserHelper _userHelper;
        private readonly IUserProfileService _profile;
        private readonly IUserService _userService;
        private readonly NotifHelper _notif;
        private readonly HostConfiguration _hostConfiguration;
        private readonly MailingHelper _mailingHelper;
        private readonly ICandidateInfoService _service;
        private readonly ISrfRequestService _srf;
        private readonly IDepartementService _department;
        private readonly INetworkNumberService _network;


        public CandidateController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IVacancyListService vacancy,
            ICandidateInfoService service,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            FileHelper fileHelper,
            ConfigHelper config,
            IUserProfileService user,
            IUserProfileService profile,
            ISrfRequestService srf,
            MailingHelper mailingHelper,
            IOptions<HostConfiguration> hostConfiguration,
            NotifHelper notif,
            INetworkNumberService network,
            IDepartementService department,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _vacancy = vacancy;
            _user = user;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileHelper = fileHelper;
            _config = config;
            _userHelper = userHelper;
            _profile = profile;
            _userService = userService;
            _notif = notif;
            _hostConfiguration = hostConfiguration.Value;
            _mailingHelper = mailingHelper;
            _service = service;
            _srf = srf;
            _department = department;
            _network = network;
        }

        [HttpGet]
        [Authorize(Roles = "HR Agency,Administrator")]
        public IActionResult Submit(Guid id)
        {
            var vacancy = _vacancy.GetById(id);
            if (vacancy == null)
            {
                return NotFound();
            }

            Dictionary<string, bool> IsEriccson = new Dictionary<string, bool>();
            IsEriccson.Add("No", false);
            IsEriccson.Add("Yes", true);
            var GenderOption = from Gender g in Enum.GetValues(typeof(Gender)) select new { Id = (int)g, Name = g.ToString() };
            var AgentOpt = from AgencyType g in Enum.GetValues(typeof(AgencyType)) select new { Id = (int)g, Name = g.ToString() };
            var MartialOpt = from Martial g in Enum.GetValues(typeof(Martial)) select new { Id = (int)g, Name = g.ToString() };

            ViewBag.VacancyId = id;
            ViewBag.Gender = GenderOption.ToList();
            ViewBag.Agent = AgentOpt.ToList();
            ViewBag.Martial = MartialOpt.ToList();
            ViewBag.IsEriccson = IsEriccson.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

            return View(new CandidateInfoModelForm());
        }

        [Authorize(Roles = "HR Agency,Administrator")]
        public async Task<IActionResult> Submit(CandidateInfoModelForm model)
        {

            if (ModelState.IsValid)
            {
                bool CheckEmail = false;
                bool CheckIdNumber = false;

                // Check Email
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var CheckEmailInSystem = await _userManager.FindByEmailAsync(model.Email);
                    if (CheckEmailInSystem == null)
                    {
                        var CheckEmailCandidate = _service.GetAll().Where(x => x.Email.Equals(model.Email) && x.VacancyId.Equals(model.VacancyId)).FirstOrDefault();
                        if (CheckEmailCandidate == null)
                        {
                            CheckEmail = true;
                        }
                    }
                    else
                    {
                        var AppUser = _userManager.FindByEmailAsync(model.Email).Result;
                        var UserProfile = _profile.GetByUserId(AppUser.Id);
                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            TempData["Error"] = "Email " + model.Email + " is blacklist";
                            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
                        }

                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            CheckEmail = true;
                        }
                       
                       
                    }
                }

                // Check Id Number
                if (!string.IsNullOrEmpty(model.IdNumber))
                {
                    var Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(model.IdNumber) && x.IsUser == true).FirstOrDefault();
                    if (Candidate != null)
                    {
                        // Candidate is User
                        var UserProfile = _profile.GetById(Candidate.AccountId);
                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            TempData["Error"] = "Id Number " + model.IdNumber + " is blacklist !!";
                            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
                        }

                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            CheckIdNumber = true;
                        }
                       
                    }
                    else
                    {
                        Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(model.IdNumber) && x.VacancyId.Equals(model.VacancyId)).FirstOrDefault();
                        if (Candidate != null)
                        {
                            TempData["Error"] = "Id Number " + model.IdNumber + " is already exist !!";
                            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
                        }
                        else
                        {
                            CheckIdNumber = true;
                        }
                    }
                }

                if (CheckEmail == true && CheckIdNumber == true)
                {
                    if (!string.IsNullOrWhiteSpace(model.Attachments))
                    {
                        string fileDestination = "uploads/candidate";
                        var AttachmentUpload = model.Attachments.Split('|');
                        var listSaveAttachment = new List<string>();
                        if (AttachmentUpload != null)
                        {
                            foreach (var file in AttachmentUpload)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    string FileName = Path.GetFileNameWithoutExtension(file).ToSlug();
                                    string MovedFiles = _fileHelper.FileMove(file, fileDestination, FileName, true);
                                    if (!string.IsNullOrEmpty(MovedFiles))
                                    {
                                        listSaveAttachment.Add(MovedFiles);
                                    }

                                }
                            }
                            if (listSaveAttachment != null)
                            {
                                model.Attachments = JsonConvert.SerializeObject(listSaveAttachment);
                            }
                        }
                    }

                    var item = Mapper.Map<CandidateInfo>(model);
                    item.RequestById = _userHelper.GetLoginUser(User).Id;
                    item.AgencyId = _userHelper.GetLoginUser(User).Id;
                    item.IsCandidate = true;
                    item.IsContractor = false;
                    item.IsUser = false;
                    item.HomeBaseId = null;
                    item.CreatedBy = _userHelper.GetUserId(User);
                    item.CreatedAt = DateTime.Now;
                    Service.Add(item);


                    // Send Notif To Line Manager
                    var Vacancy = _vacancy.GetById(model.VacancyId);
                    var LineManager = _user.GetById(Vacancy.ApproverOneId);
                    var LmUser = _userService.GetById(LineManager.ApplicationUserId);

                    var callbackUrl = Url.Action("Details",
                     "Vacancy",
                      new { area = "Admin", id = model.VacancyId },
                     _hostConfiguration.Protocol,
                     _hostConfiguration.Name);

                    _notif.Send(
                       User, // User From
                       "New CV is submitted by "+_userHelper.GetLoginUser(User).Name, // Subject
                       LineManager.Name, // User target name
                       LmUser.Email, // User target email
                       callbackUrl, // Link CallBack
                       "New CV is submitted by " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                       item.Description, // Description
                       NotificationInboxStatus.Request, // Notif Status
                       Activities.Vacant // Activity Status
                   );


                    TempData["Success"] = "Candidate with name " + model.Name + " have been added !!";
                    return RedirectToAction("Details", "Vacancy", new { id = model.VacancyId });
                }

            }

            TempData["Error"] = string.Join(",", ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors).Select(E => E.ErrorMessage).ToArray());
            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
        }

        [Authorize(Roles = "HR Agency,Administrator,Line Manager,Sourcing")]
        public override IActionResult Details(Guid id)
        {
            var info = Service.GetById(id);
            if (info == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(info.Attachments))
            {
                ViewBag.Files = JsonConvert.DeserializeObject<List<string>>(info.Attachments);
            }
            else
            {
                ViewBag.Files = null;
            }
            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            ViewBag.Vacancy = _vacancy.GetById(info.VacancyId);
            ViewBag.UserRole = UserRole.FirstOrDefault();
            return base.Details(id);
        }

        [Authorize(Roles = "HR Agency,Administrator")]
        public override IActionResult Edit(Guid id)
        {
            var info = Service.GetById(id);
            if (info == null)
            {
                return NotFound();
            }

            ViewBag.Vacancy = _vacancy.GetById(info.VacancyId);
            Dictionary<string, bool> IsEriccson = new Dictionary<string, bool>();
            IsEriccson.Add("No", false);
            IsEriccson.Add("Yes", true);
            var GenderOption = from Gender g in Enum.GetValues(typeof(Gender)) select new { Id = (int)g, Name = g.ToString() };
            var AgentOpt = from AgencyType g in Enum.GetValues(typeof(AgencyType)) select new { Id = (int)g, Name = g.ToString() };
            var MartialOpt = from Martial g in Enum.GetValues(typeof(Martial)) select new { Id = (int)g, Name = g.ToString() };

            ViewBag.VacancyId = info.VacancyId;
            ViewBag.Gender = GenderOption.ToList();
            ViewBag.Agent = AgentOpt.ToList();
            ViewBag.Martial = MartialOpt.ToList();
            ViewBag.IsEriccson = IsEriccson.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.Info = info;
            ViewBag.Id = id;

            var model = Mapper.Map<CandidateInfoModelForm>(info);
            if (!string.IsNullOrWhiteSpace(info.Attachments))
            {
                var attachments = JsonConvert.DeserializeObject<List<string>>(info.Attachments);
                model.Attachments = string.Join("|", attachments.ToArray()) + "|";
            }
            return View(model);
        }

        protected override void CreateData(CandidateInfo item)
        {
            item.RequestBy = _userHelper.GetUser(User).UserProfile;
        }

        protected override void UpdateData(CandidateInfo item, CandidateInfoModelForm model)
        {
            item.RequestBy = _userHelper.GetUser(User).UserProfile;
        }

        [Authorize(Roles = "HR Agency,Administrator")]
        public async Task<IActionResult> Update(Guid id, CandidateInfoModelForm model)
        {

            if (ModelState.IsValid)
            {
                bool CheckEmail = false;
                bool CheckIdNumber = false;

                // Check Email
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var CheckEmailInSystem = await _userManager.FindByEmailAsync(model.Email);
                    if (CheckEmailInSystem == null)
                    {
                        var CheckEmailCandidate = _service.GetAll().Where(x => x.Email.Equals(model.Email) && x.Id != id).FirstOrDefault();
                        if (CheckEmailCandidate == null)
                        {
                            CheckEmail = true;
                        }
                    }
                    else
                    {
                        var AppUser = _userManager.FindByEmailAsync(model.Email).Result;
                        var UserProfile = _profile.GetByUserId(AppUser.Id);
                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            TempData["Error"] = "Email " + model.Email + " is blacklist !!";
                            return RedirectToAction("Edit", "Candidate", new { id = id });
                        }
                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            CheckEmail = true;
                        }
                        
                    }
                }

                // Check Id Number
                if (!string.IsNullOrEmpty(model.IdNumber))
                {
                    var Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(model.IdNumber) && x.IsUser == true && x.Id != model.VacancyId).FirstOrDefault();
                    if (Candidate != null)
                    {
                        // Candidate is User
                        var UserProfile = _profile.GetById(Candidate.AccountId);
                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            TempData["Error"] = "Id Number " + model.IdNumber + " is blacklist !!";
                            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
                        }
                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            CheckIdNumber = true;
                        }
                       
                    }
                    else
                    {
                        Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(model.IdNumber) && x.VacancyId.Equals(model.VacancyId) && x.Id != id).FirstOrDefault();
                        if (Candidate != null)
                        {
                            TempData["Error"] = "Id Number " + model.IdNumber + " is already exist !!";
                            return RedirectToAction("Submit", "Candidate", new { id = model.VacancyId });
                        }
                        else
                        {
                            CheckIdNumber = true;
                        }
                    }
                }

                if (CheckEmail == true && CheckIdNumber == true)
                {

                    var item = Service.GetById(id);
                    item.Name = model.Name;
                    item.Email = model.Email;
                    item.IdNumber = model.IdNumber;
                    item.Nationality = model.Nationality;
                    item.DateOfBirth = model.DateOfBirth;
                    item.PlaceOfBirth = model.PlaceOfBirth;
                    item.Martial = model.Martial;
                    item.Address = model.Address;
                    item.Gender = model.Gender;
                    item.Description = model.Description;
                    item.NickName = model.NickName;
                    item.MobilePhoneNumber = model.MobilePhoneNumber;
                    item.HomePhoneNumber = model.HomePhoneNumber;
                    item.IsFormerEricsson = model.IsFormerEricsson;
                    item.LastEditedBy = _userHelper.GetUserId(User);
                    item.LastUpdateTime = DateTime.Now;

                    if (User.IsInRole("HR Agency"))
                    {
                        var GetVacancy = _vacancy.GetById(model.VacancyId);
                        if (item.ApproveOneStatus == ApproverStatus.Shortlist || item.ApproveTwoStatus == ApproverStatus.Shortlist)
                        {

                            var GetLM = _profile.GetById(GetVacancy.ApproverOneId);
                            var AppUser = _userService.GetById(GetLM.ApplicationUserId);

                            item.ApproveOneStatus = ApproverStatus.Shortlist;
                            item.ApproveTwoStatus = ApproverStatus.Shortlist;

                            // Send To LM Again
                            var callbackUrl = Url.Action("Details",
                            "Vacancy",
                             new { area = "Admin", id = model.VacancyId },
                            _hostConfiguration.Protocol,
                            _hostConfiguration.Name);

                            _notif.Send(
                                User, // User From
                                "CV Correction", // Subject
                                GetLM.Name, // User target name
                                AppUser.Email, // User target email
                                callbackUrl, // Link CallBack
                                "CV Correction", // Email content or descriptions
                                item.Description, // Description
                                NotificationInboxStatus.Request, // Notif Status
                                Activities.Vacant // Activity Status
                            );

                            GetVacancy.VacancyStatus = ApproverStatus.Shortlist;
                            _vacancy.Update(GetVacancy);

                        }

                    }
                    Service.Update(item);
                    TempData["Success"] = "Candidate with name " + model.Name + " have been updated !!";
                    return RedirectToAction("Details", "Vacancy", new { id = model.VacancyId });
                }

            }

            TempData["Error"] = string.Join(",", ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors).Select(E => E.ErrorMessage).ToArray());
            return RedirectToAction("Details", "Vacancy", new { id = model.VacancyId });

        }

        [HttpPost]
        [Authorize(Roles = "Line Manager,Sourcing")]
        public IActionResult Approval(string CandidateId, bool ApprovalStatus, string ApprovalNotes = null)
        {
            var Candidate = Service.GetById(Guid.Parse(CandidateId));

            var callbackUrl = Url.Action("Details",
               "Vacancy",
               new { area = "Admin", id = Candidate.VacancyId },
               _hostConfiguration.Protocol,
               _hostConfiguration.Name);


            if (ApprovalStatus == true)
            {
                if(User.IsInRole("Line Manager") && User.IsInRole("Sourcing"))
                {
                    Candidate.ApproveOneStatus = ApproverStatus.Selected;
                    Candidate.ApproveOneDate = DateTime.Now;
                    Candidate.ApproveOneNotes = ApprovalNotes;
                    Candidate.ApproveTwoStatus = ApproverStatus.Completed;
                    Candidate.ApproveTwoeDate = DateTime.Now;
                    Candidate.ApproveTwoNotes = ApprovalNotes;

                    // Update Another Candidate
                    var Temp = Service.GetAll().Where(x => x.VacancyId.Equals(Candidate.VacancyId) && x.Id != Candidate.Id).ToList();
                    foreach (var row in Temp)
                    {
                        var c = Service.GetById(row.Id);
                        c.ApproveTwoStatus = ApproverStatus.Shortlist;
                        Service.Update(c);
                    }

                }
                else if (User.IsInRole("Line Manager"))
                {
                    Candidate.ApproveOneStatus = ApproverStatus.Selected;
                    Candidate.ApproveOneDate = DateTime.Now;
                    Candidate.ApproveOneNotes = ApprovalNotes;
                    Service.Update(Candidate);

                    // Send Notif To Sourcing

                    var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                    var Sourcing = _user.GetById(Vacancy.ApproverTwoId);
                    var SrcUser = _userService.GetById(Sourcing.ApplicationUserId);

                    _notif.Send(
                       User, // User From
                       "Job vacancy CV is approved by "+_userHelper.GetLoginUser(User).Name, // Subject
                       Sourcing.Name, // User target name
                       SrcUser.Email, // User target email
                       callbackUrl, // Link CallBack
                       "New Job vacancy CV is approved by "+ _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                       ApprovalNotes, // Description
                       NotificationInboxStatus.Approval, // Notif Status
                       Activities.Vacant // Activity Status
                   );

                }

                else if (User.IsInRole("Sourcing"))
                {
                    Candidate.ApproveTwoStatus = ApproverStatus.Completed;
                    Candidate.ApproveTwoeDate = DateTime.Now;
                    Candidate.ApproveTwoNotes = ApprovalNotes;
                    Service.Update(Candidate);

                    // Update Another Candidate
                    var Temp = Service.GetAll().Where(x => x.VacancyId.Equals(Candidate.VacancyId) && x.Id != Candidate.Id).ToList();
                    foreach (var row in Temp)
                    {
                        var c = Service.GetById(row.Id);
                        c.ApproveTwoStatus = ApproverStatus.Shortlist;
                        Service.Update(c);
                    }

                }

                TempData["Approved"] = "OK";
            }
            else
            {
               
                // Send Correction To Agency
                var Up = _userHelper.GetUserProfile(Candidate.AgencyId.Value);
                var AppUser = _userHelper.GetUser(Up.ApplicationUserId);

                _notif.Send(
                    User, // User From
                    "Correction CV", // Subject
                    Up.Name, // User target name
                    Up.Email, // User target email
                    callbackUrl, // Link CallBack
                    "Rejected CV", // Email content or descriptions
                    ApprovalNotes, // Description
                    NotificationInboxStatus.Reject, // Notif Status
                    Activities.Vacant // Activity Status
                );

                Candidate.ApproveOneStatus = ApproverStatus.Shortlist;
                Candidate.ApproveTwoStatus = ApproverStatus.Shortlist;
                Candidate.ApproveOneDate = DateTime.Now;
                Candidate.ApproveOneNotes = ApprovalNotes;
                Service.Update(Candidate);
            }

            var Success = Service.GetAll().Where(x => x.ApproveTwoStatus == ApproverStatus.Completed && x.VacancyId.Equals(Candidate.VacancyId)).FirstOrDefault();
            if (Success != null)
            {
                var vc = _vacancy.GetById(Candidate.VacancyId);
                vc.VacancyStatus = ApproverStatus.Completed;
                _vacancy.Update(vc);

                var Dp = _department.GetById(vc.DepartmentId);
                var Network = _network.GetById(vc.NetworkId);

                // SEND TO SRF
                var LineManager = _user.GetById(vc.ApproverOneId);

                SrfRequest Srf = new SrfRequest
                {
                    Type = SrfType.New,
                    ApproveOneBy = vc.ApproverOne,
                    CandidateId = Candidate.Id,
                    CreatedAt = DateTime.Now,
                    RequestBy = _userHelper.GetLoginUser(User).Name,
                    ServicePackId = vc.ServicePackId,
                    ServiceLevel = vc.OtLevel,
                    isWorkstation = vc.isLaptop,
                    isCommunication = vc.isUsim,
                    NetworkId = vc.NetworkId,
                    IsManager = vc.isManager,
                    DepartmentId = vc.DepartmentId,
                    DepartmentSubId = vc.DepartmentSubId,
                    CostCenterId = vc.CostCodeId,
                    IsHrms = vc.isHrms,
                    IsOps = Dp.OperateOrNon == 1 ? true : false,
                    LineManagerId = LineManager.Id,
                    Status = SrfStatus.Waiting,
                    AccountId = Network.AccountNameId,
                    ApproveStatusOne = SrfApproveStatus.Waiting,  // Line Manager
                    ApproveStatusTwo = SrfApproveStatus.Waiting,  // Head Of Service Line
                    ApproveStatusThree = SrfApproveStatus.Waiting,// Head Of Operation
                    ApproveStatusFour = SrfApproveStatus.Waiting, // Head Of Non Operation
                    ApproveStatusFive = SrfApproveStatus.Waiting, // Head Of Sourcing
                    ApproveStatusSix = SrfApproveStatus.Waiting,  // Service Cordinator
                    ProjectManagerId = Network.ProjectManagerId,
                    SrfBegin = null,
                    SrfEnd = null,
                    IsActive = false,
                    IsLocked = false,
                    AnnualLeave = 0
                };
                _srf.Add(Srf);

                // Send Notif To Line Manager

                var LmUser = _userService.GetById(LineManager.ApplicationUserId);
                _notif.Send(
                   User, // User From
                   "Job vacancy CV is approved by "+ _userHelper.GetLoginUser(User).Name, // Subject
                   LineManager.Name, // User target name
                   LmUser.Email, // User target email
                   callbackUrl, // Link CallBack
                   "Job vacancy CV is approved by " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                   ApprovalNotes, // Description
                   NotificationInboxStatus.Request, // Notif Status
                   Activities.SRF // Activity Status
               );

                TempData["Approved"] = "OK";
            }
            else
            {
                var Selected = Service.GetAll().Where(x => x.ApproveOneStatus == ApproverStatus.Selected && x.VacancyId.Equals(Candidate.VacancyId)).Count();
                var vc = _vacancy.GetById(Candidate.VacancyId);
                if (Selected > 0)
                {
                    vc.VacancyStatus = ApproverStatus.Selected;
                }
                else
                {
                    TempData["Rejected"] = "OK";
                    vc.VacancyStatus = ApproverStatus.Rejected;
                }
                _vacancy.Update(vc);
            }


           
            return RedirectToAction("Details", "Vacancy", new { id = Candidate.VacancyId });
        }
    }
}