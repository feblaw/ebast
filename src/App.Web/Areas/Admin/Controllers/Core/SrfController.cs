using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using App.Web.Models.ViewModels.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Authorize]
    [Area("Admin")]
    public class SrfController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {

        private readonly IVacancyListService _vacancy;
        private readonly ICandidateInfoService _candidate;
        private readonly IDepartementService _department;
        private readonly IServicePackService _ssow;
        private readonly IServicePackCategoryService _ssowCategory;
        private readonly IJobStageService _jobStage;
        private readonly IDepartementSubService _departmentSub;
        private readonly INetworkNumberService _network;
        private readonly IAccountNameService _account;
        private readonly NotifHelper _notif;
        private readonly HostConfiguration _hostConfiguration;
        private readonly MailingHelper _mailingHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICostCenterService _costCenter;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ConfigHelper _config;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly IUserProfileService _profileService;
        private readonly ISrfMigrationService _migrate;
        private readonly IPackageTypeService _packageType;


        public SrfController(IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            ISrfRequestService service,
            IVacancyListService vacancy,
            ICandidateInfoService candidate,
            IDepartementService department,
            IServicePackService ssow,
            ICostCenterService costCenter,
            ISrfMigrationService migrate,
            MailingHelper mailingHelper,
            IOptions<HostConfiguration> hostConfiguration,
            NotifHelper notif,
            IDepartementSubService departmentSub,
            IServicePackCategoryService ssowCategory,
            IJobStageService jobsTage,
            INetworkNumberService network,
            IAccountNameService account,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ConfigHelper config,
            IPackageTypeService packageType,
            IUserProfileService profileService,
            ISrfEscalationRequestService escalation,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _vacancy = vacancy;
            _candidate = candidate;
            _department = department;
            _ssow = ssow;
            _ssowCategory = ssowCategory;
            _jobStage = jobsTage;
            _departmentSub = departmentSub;
            _network = network;
            _account = account;
            _notif = notif;
            _hostConfiguration = hostConfiguration.Value;
            _mailingHelper = mailingHelper;
            _userHelper = userHelper;
            _costCenter = costCenter;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _escalation = escalation;
            _profileService = profileService;
            _migrate = migrate;
            _packageType = packageType;
        }

        public override IActionResult Index()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return base.Index();
        }

        public IActionResult Pending()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public IActionResult PendingEsc()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public override IActionResult Edit(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var Vacancy = _vacancy.GetById(_candidate.GetById(item.CandidateId).VacancyId);
                var Departement = _department.GetById(item.DepartmentId);
                var PackageType = _packageType.GetById(Vacancy.PackageTypeId);
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;

                ViewBag.PriceType = PackageType.Name;
                ViewBag.Vacancy = Vacancy;
                ViewBag.Candidate = _candidate.GetById(item.CandidateId);
                ViewBag.Departement = _department.GetById(item.DepartmentId);
                ViewBag.SSOW = _ssow.GetById(item.ServicePackId);
                ViewBag.SCategory = _ssowCategory.GetById(_ssow.GetById(item.ServicePackId).ServicePackCategoryId);
                ViewBag.JobsStage = _jobStage.GetById(Vacancy.JobStageId);
                ViewBag.ListDepartement = _department.GetAll().ToList();
                if (Departement.OperateOrNon == 1)
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                }
                else
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                }
                ViewBag.isOperation = Departement.OperateOrNon;
                ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
                ViewBag.ListAccount = _account.GetAll().ToList();
                ViewBag.ListSubDepartment = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(item.DepartmentId)).ToList();
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(item.DepartmentId)
                    && x.Status == Status.Active
                    ).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(item.DepartmentId) && x.IsClosed == false).ToList();
                ViewBag.Id = id;
                ViewBag.isOperation = Departement.OperateOrNon;
                ViewBag.txtOperaion = (Departement.OperateOrNon == 1) ? "Operational" : "Non Operational";
                ViewBag.FormDisable = 1;
                ViewBag.SrfNumber = Service.GenerateNumnber();
                ViewBag.NowYear = DateTime.Now.Year.ToString("yy");

                if (User.IsInRole("Line Manager"))
                {
                    if (item.Status == SrfStatus.Waiting)
                    {
                        if (item.ApproveStatusOne == SrfApproveStatus.Reject ||
                           item.ApproveStatusOne == SrfApproveStatus.Waiting ||
                           item.ApproveStatusThree == SrfApproveStatus.Reject ||
                           item.ApproveStatusFour == SrfApproveStatus.Reject ||
                           item.ApproveStatusSix == SrfApproveStatus.Reject)
                        {
                            ViewBag.FormDisable = 0;
                        }

                    }
                }
                else if (User.IsInRole("Administrator") || User.IsInRole("Sourcing"))
                {
                    ViewBag.FormDisable = 0;
                }

                if (item.ApproveStatusOne == SrfApproveStatus.Submitted &&  // Line Manager
                   (item.ApproveStatusThree == SrfApproveStatus.Approved || // Head Ops Op
                    item.ApproveStatusFour == SrfApproveStatus.Approved) && // Head Ops Non Op
                    item.ApproveStatusSix == SrfApproveStatus.Approved) // Service Coordinator
                {
                    ViewBag.SetExtended = "1";
                }
                else
                {
                    ViewBag.SetExtended = "0";
                }

                var esc = _escalation.GetAll().Where(x => x.SrfId.Equals(id)).FirstOrDefault();
                if (esc != null)
                {
                    ViewBag.IsEscalation = "1";
                }
                else
                {
                    ViewBag.IsEscalation = "0";
                }

                if (item.Status == SrfStatus.Terminate || item.Status == SrfStatus.Blacklist)
                {
                    ViewBag.SetExtended = "0";
                }

                ViewBag.PreofileId = PreofileId;
                ViewBag.LineManagerId = item.ApproveOneId;
                ViewBag.ServiceLineId = item.ApproveTwoId;
                ViewBag.HeadOpId = item.ApproveThreeId;
                ViewBag.HeadNonId = item.ApproveFourId;
                ViewBag.ServiceCoordId = item.ApproveSixId;

                return base.Edit(id);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }
        protected override void UpdateData(SrfRequest item, SrfRequestModelForm model)
        {
            var Dept = _department.GetById(model.DepartmentId);
            String type = "";
            if (item.IsExtended == true)
            {
                type = "( Extended )";
            }
            #region Approval
            if (User.IsInRole("Line Manager") && model.ApproveStatusOne == SrfApproveStatus.Submitted)
            {

                if (model.ApproveTwoId > 0)
                {
                    item.ApproveTwoId = model.ApproveTwoId;
                }

                if (model.ApproveThreeId > 0)
                {
                    item.ApproveThreeId = model.ApproveThreeId;
                }

                if (model.ApproveFourId > 0)
                {
                    item.ApproveFourId = model.ApproveFourId;
                }

                if (model.ApproveSixId > 0)
                {
                    item.ApproveSixId = model.ApproveSixId;
                }

                item.SrfBegin = model.SrfBegin;
                item.SrfEnd = model.SrfEnd;
                MultiApprove(true, item, model.Description, item.Id.ToString(), type, Dept.OperateOrNon);
            }
            #endregion
            else
            {
                int total_day = Extension.MonthDistance(model.SrfBegin, model.SrfEnd);
                item.SrfBegin = model.SrfBegin;
                item.SrfEnd = model.SrfEnd;
                item.DepartmentId = model.DepartmentId;
                item.DepartmentSubId = model.DepartmentSubId;
                item.CostCenterId = model.CostCenterId;
                item.AccountId = model.AccountId;
                item.NetworkId = model.NetworkId;
                item.Description = model.Description;
                item.ApproveStatusOne = model.ApproveStatusOne;
                item.ApproveSixId = model.ApproveSixId;
                item.SrfBegin = model.SrfBegin;
                item.SrfEnd = model.SrfEnd;
                item.DateApproveStatusOne = DateTime.Now;

                if (item.Type == SrfType.Extension)
                {
                    item.AnnualLeave = item.AnnualLeave + (int)total_day;
                }
                else
                {
                    item.AnnualLeave = (int)total_day;
                }

                if (Dept.OperateOrNon == 1)
                {
                    // Send To Head Of Service Line
                    item.ApproveFourId = null;
                    item.ApproveTwoId = model.ApproveTwoId;
                    item.ApproveThreeId = model.ApproveThreeId;
                }
                else
                {
                    // Send To Non Head Operation
                    item.ApproveTwoId = null;
                    item.ApproveThreeId = null;
                    if (model.ApproveFourId > 0)
                    {
                        item.ApproveFourId = model.ApproveFourId;
                    }
                }
            }


        }
        protected override void AfterUpdateData(SrfRequest before, SrfRequest after)
        {
            TempData["Submitted"] = "OK";
        }
        public override IActionResult Details(Guid id)
        {
            return RedirectToAction("Edit", new { Id = id });
        }
        public IActionResult Extends(Guid id)
        {
            var item = Service.GetById(id);
            if (item != null)
            {

                SrfRequest Srf = new SrfRequest
                {
                    Type = SrfType.Extension,
                    ApproveOneBy = item.ApproveOneBy,
                    CandidateId = item.CandidateId,
                    CreatedAt = DateTime.Now,
                    RequestBy = _userHelper.GetLoginUser(User).Name,
                    ServicePackId = item.ServicePackId,
                    ServiceLevel = item.ServiceLevel,
                    isWorkstation = item.isWorkstation,
                    isCommunication = item.isCommunication,
                    NetworkId = item.NetworkId,
                    IsManager = item.IsManager,
                    DepartmentId = item.DepartmentId,
                    DepartmentSubId = item.DepartmentSubId,
                    CostCenterId = item.CostCenterId,
                    IsHrms = item.IsHrms,
                    IsOps = true,
                    LineManagerId = item.LineManagerId,
                    Status = SrfStatus.Waiting,
                    AccountId = item.AccountId,
                    ApproveStatusOne = SrfApproveStatus.Waiting,  // Line Manager
                    ApproveStatusTwo = SrfApproveStatus.Waiting,  // Head Of Service Line
                    ApproveStatusThree = SrfApproveStatus.Waiting,// Head Of Operation
                    ApproveStatusFour = SrfApproveStatus.Waiting, // Head Of Non Operation
                    ApproveStatusFive = SrfApproveStatus.Waiting, // Head Of Sourcing
                    ApproveStatusSix = SrfApproveStatus.Waiting,  // Service Cordinator
                    ProjectManagerId = item.ProjectManagerId,
                    ExtendFrom = item.Id,
                    SrfBegin = null,
                    SrfEnd = null,
                    IsActive = false,
                    AnnualLeave = item.AnnualLeave
                };
                Service.Add(Srf);
                TempData["Extend"] = "OK";
                return RedirectToAction("Edit", new { Id = Srf.Id });
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Terminate(Guid id, int type)
        {
            var item = Service.GetById(id);
            String What = "";
            if (type == 3)
            {
                What = "Blacklist";
            }
            else
            {
                What = "Terminate";
            }
            if (item != null)
            {
                item.Status = SrfStatus.WaitingTerminate;
                Service.Update(item);

                // Send To Agency
                var Candidate = _candidate.GetById(item.CandidateId);
                var AgencyProfile = _profileService.GetById(Candidate.AgencyId);
                var AgencyUser = _userHelper.GetUser(AgencyProfile.ApplicationUserId);
                var LineManager = _userHelper.GetUserProfile(item.ApproveOneId.Value);
                if (AgencyUser != null)
                {
                    var callbackUrl = Url.Action("Index",
                      "Srf",
                       new { area = "Admin" },
                      _hostConfiguration.Protocol,
                      _hostConfiguration.Name);

                    var Data = new Dictionary<string, string>()
                        {
                            { "AgencyName", AgencyProfile.Name},
                            { "SubjectTitle", What},
                            { "NameResource", Candidate.Name},
                            { "ContractEnd", item.SrfEnd.Value.ToString("dd MMM yyyy") },
                            { "Note", item.Description },
                            { "LineManagerName", LineManager.Name },
                        };

                    _notif.Send(
                        User, // User From
                        "Request " + What + " Resource", // Subject
                        AgencyProfile.Name, // User target name
                        AgencyUser.Email, // User target email
                        callbackUrl, // Link CallBack
                        null, // Email content or descriptions
                        null, // Description
                        NotificationInboxStatus.Request, // Notif Status
                        Activities.SRF, // Activity Status,
                        null,
                        "Emails/Terminate",
                        Data
                    );

                    var GetAdmin = _userManager.GetUsersInRoleAsync("Administrator").Result;
                    if (GetAdmin != null)
                    {
                        foreach (var row in GetAdmin)
                        {
                            var ProfileAdmin = _profileService.GetByUserId(row.Id);
                            _notif.Send(
                                User, // User From
                                "Request " + What + " Resource", // Subject
                                ProfileAdmin.Name, // User target name
                                row.Email, // User target email
                                callbackUrl, // Link CallBack
                                null, // Email content or descriptions
                                null, // Description
                                NotificationInboxStatus.Request, // Notif Status
                                Activities.SRF, // Activity Status,
                                null,
                                "Emails/Terminate",
                                Data
                            );
                        }
                    }

                    TempData["Terminate"] = "OK";
                    return RedirectToAction("Edit", new { Id = id });
                }



            }
            return NotFound();
        }
        public IActionResult ApproveTerminate(string id, string notes, int submit, string date = null)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (User.IsInRole("Line Manager"))
            {
                item.Status = SrfStatus.WaitingTerminate;
                Service.Update(item);

                // Send To Agency
                var Candidate = _candidate.GetById(item.CandidateId);
                var AgencyProfile = _profileService.GetById(Candidate.AgencyId);
                var AgencyUser = _userHelper.GetUser(AgencyProfile.ApplicationUserId);
                var LineManager = _userHelper.GetUserProfile(item.ApproveOneId.Value);
                if (AgencyUser != null)
                {
                    var callbackUrl = Url.Action("Index",
                      "Srf",
                       new { area = "Admin" },
                      _hostConfiguration.Protocol,
                      _hostConfiguration.Name);

                    string What = submit == 3 ? "Terminate" : "Blacklist";

                    var Data = new Dictionary<string, string>()
                        {
                            { "AgencyName", AgencyProfile.Name},
                            { "SubjectTitle", What},
                            { "NameResource", Candidate.Name},
                            { "ContractEnd", item.SrfEnd.Value.ToString("dd MMM yyyy") },
                            { "Note", item.Description },
                            { "LineManagerName", LineManager.Name },
                        };

                    _notif.Send(
                        User, // User From
                        "Request " + What + " Resource", // Subject
                        AgencyProfile.Name, // User target name
                        AgencyUser.Email, // User target email
                        callbackUrl, // Link CallBack
                        null, // Email content or descriptions
                        null, // Description
                        NotificationInboxStatus.Request, // Notif Status
                        Activities.SRF, // Activity Status,
                        null,
                        "Emails/Terminate",
                        Data
                    );


                    TempData["Terminate"] = "OK";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                if (item != null)
                {
                    var Candidate = _candidate.GetById(item.CandidateId);
                    var UserProfile = _profileService.GetById(Candidate.AccountId);
                    item.TeriminateNote = notes;
                    item.TerimnatedBy = _userHelper.GetLoginUser(User).Name;
                    if (!string.IsNullOrWhiteSpace(date))
                    {
                        item.TerminatedDate = DateTime.Parse(date);
                    }
                    else
                    {
                        item.TerminatedDate = DateTime.Now;
                    }
                    if (submit == 3)
                    {
                        item.Status = SrfStatus.Terminate;
                        UserProfile.IsTerminate = true;
                        UserProfile.IsActive = false;
                        _profileService.Update(UserProfile);
                    }
                    else
                    {
                        item.Status = SrfStatus.Blacklist;
                        UserProfile.IsBlacklist = true;
                        UserProfile.IsActive = false;
                        _profileService.Update(UserProfile);
                    }
                    Service.Update(item);
                    TempData["Terminated"] = "OK";
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult ApproveMultipleFaux(string srfId, bool status, string notes)
        {
            if (!string.IsNullOrEmpty(srfId))
            {
                if (User.IsInRole("Service Coordinator"))
                {
                    ApproveGeneral(srfId, status, notes, Service.GenerateNumnber());
                }
                else
                {
                    ApproveGeneral(srfId, status, notes, null);
                }

                if (status == true)
                {
                    TempData["Approved"] = "OK";
                }
                else
                {
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ApproveMultiple(string data, bool status)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {

                        if (User.IsInRole("Service Coordinator"))
                        {
                            ApproveGeneral(id, status, null, Service.GenerateNumnber());
                        }
                        else
                        {
                            ApproveGeneral(id, status, null, null);
                        }

                    }
                    if (status == true)
                    {
                        TempData["Approved"] = "OK";
                    }
                    else
                    {
                        TempData["Rejected"] = "OK";
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private void SendNotif(string Id, int? UserId, string Subject, string Content, NotificationInboxStatus status, string Notes = null)
        {
            var Profile = _userHelper.GetUserProfile(UserId.Value);
            if (Profile != null)
            {
                var AppUser = _userHelper.GetUser(Profile.ApplicationUserId);
                var callbackUrl = Url.Action("Details",
                   "Srf",
                    new { area = "Admin", id = Guid.Parse(Id) },
                   _hostConfiguration.Protocol,
                   _hostConfiguration.Name);

                _notif.Send(
                    User, // User From
                    Subject, // Subject
                    Profile.Name, // User target name
                    AppUser.Email, // User target email
                    callbackUrl, // Link CallBack
                    Content, // Email content or descriptions
                    Notes, // Description
                    status, // Notif Status
                    Activities.SRF // Activity Status
                );

            }
        }
        [HttpPost]
        public IActionResult Approval(string id, bool status, string notes = null, string number = null)
        {
            ApproveGeneral(id, status, notes, number);
            return RedirectToAction("Index");
        }
        public IActionResult MultiReject(string data, string remarks)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {
                        ApproveGeneral(id, false, remarks, null);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Index");
        }
        private void ApproveGeneral(string id, bool status, string notes = null, string number = null)
        {
            var item = Service.GetById(Guid.Parse(id));
            var Dept = _department.GetById(item.DepartmentId);
            if (item != null)
            {
                MultiApprove(status, item, notes, id, item.IsExtended == true ? "( Extended )" : "", Dept.OperateOrNon, number);
            }

        }
        private void SendNotifReject(SrfRequest Srf, string Notes, string Roles)
        {
            var AppProfile = _userHelper.GetUserProfile(Srf.ApproveOneId.Value);
            if (AppProfile != null)
            {
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);

                var callbackUrl = Url.Action("Edit",
                    "Escalation",
                     new { area = "Admin", id = Srf.Id },
                    _hostConfiguration.Protocol,
                    _hostConfiguration.Name);

                _notif.Send(
                   User, // User From
                   "Service Request Form is rejected by " + Roles + " ", // Subject
                   AppProfile.Name, // User target name
                   AppUser.Email, // User target email
                   callbackUrl, // Link CallBack
                   "Service Request Form is rejected by " + Roles + "", // Email content or descriptions
                   Notes, // Description
                   NotificationInboxStatus.Reject, // Notif Status
                   Activities.SRF // Activity Status
               );
            }
        }
        private void UpdateUser(SrfRequest item, string notes, string id, string type, string number)
        {
            #region CreateUser
            var Candidate = _candidate.GetById(item.CandidateId);
            var AppUser = _userManager.FindByEmailAsync(Candidate.Email).Result;

            var defaultPassword = _config.GetConfig("user.default.password");
            if (AppUser == null)
            {
                // Email not exists
                var user = new ApplicationUser()
                {
                    UserName = Candidate.Email,
                    Email = Candidate.Email,
                    UserProfile = new UserProfile()
                    {
                        Name = Candidate.Name,
                        Email = Candidate.Email,
                        UserName = Candidate.Email,
                        IsActive = true,
                        Roles = "Contractor",
                        Address = Candidate.Address,
                        Birthdate = Candidate.DateOfBirth,
                        Description = Candidate.Description,
                        Gender = Candidate.Gender,
                        HomePhoneNumber = Candidate.HomePhoneNumber,
                        MobilePhoneNumber = Candidate.MobilePhoneNumber,
                        IdNumber = Candidate.IdNumber,
                        IsBlacklist = false,
                        IsTerminate = false

                    }
                };
                var result = _userManager.CreateAsync(user, defaultPassword).Result;
                if (result.Succeeded)
                {
                    List<string> Roles = new List<string> { "Contractor" };
                    result = _userManager.AddToRolesAsync(user, Roles).Result;
                    if (result.Succeeded)
                    {
                        var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                        //var callbackUrl = Url.Action("ConfirmEmail",
                        //    "Account",
                        // new { userId = user.Id, code = code, area = "" },
                        //_hostConfiguration.Protocol,
                        // _hostConfiguration.Name);

                        //var additionalData = new Dictionary<string, string>()
                        //        {
                        //            { "CallbackUrl", callbackUrl },
                        //            { "Name", Candidate.Name },
                        //            { "Email", Candidate.Email },
                        //            { "Password", defaultPassword }
                        //        };

                        //var subject = "You have been registered, please confirm your account, Leave to be taken cannot compensate";

                        //var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                        //    Candidate.Email,
                        //    subject,
                        //    "Emails/RegisterUser",
                        //    user,
                        //    additionalData,
                        //    null);

                        //// Send Email Confirmation To User
                        //var emailResult = _mailingHelper.SendEmail(email).Result;
                        var confirm = _userManager.ConfirmEmailAsync(user, code).Result;
                        if (confirm.Succeeded)
                        {
                            // Update Candidate
                            Candidate.IsCandidate = false;
                            Candidate.IsContractor = true;
                            Candidate.IsUser = true;
                            Candidate.Account = user.UserProfile;
                            _candidate.Update(Candidate);
                            Service.SetActive(item.Id, Candidate.Id, user.UserProfile.Id);
                        }

                    }
                }

            }
            else
            {
                if (item.Type == SrfType.New)
                {
                    // Current email registered
                    var UserProfile = _profileService.GetByUserId(AppUser.Id);
                    //var callbackUrl = Url.Action("Index",
                    //    "Home",
                    // new { area = "Admin" },
                    //_hostConfiguration.Protocol,
                    // _hostConfiguration.Name);

                    //var additionalData = new Dictionary<string, string>()
                    //            {
                    //                { "CallbackUrl", callbackUrl },
                    //                { "Name", Candidate.Name },
                    //                { "Email", Candidate.Email },
                    //                { "Password", defaultPassword }
                    //            };

                    //var subject = "You have been registered, please confirm your account, Leave to be taken cannot compensate";

                    //var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                    //    Candidate.Email,
                    //    subject,
                    //    "Emails/RegisterUser",
                    //    AppUser,
                    //    additionalData,
                    //    null);

                    //// Send Email Confirmation To User
                    //var emailResult = _mailingHelper.SendEmail(email).Result;

                    // Update User Profile
                    UserProfile up = _profileService.GetById(UserProfile.Id);
                    up.Name = Candidate.Name;
                    up.Email = Candidate.Email;
                    up.UserName = Candidate.Email;
                    up.IsActive = true;
                    up.Address = Candidate.Address;
                    up.Birthdate = Candidate.DateOfBirth;
                    up.Description = Candidate.Description;
                    up.Gender = Candidate.Gender;
                    up.HomePhoneNumber = Candidate.HomePhoneNumber;
                    up.MobilePhoneNumber = Candidate.MobilePhoneNumber;
                    up.IdNumber = Candidate.IdNumber;
                    up.IsBlacklist = false;
                    up.IsTerminate = false;
                    _profileService.Update(up);

                    // Update Candidate
                    Candidate.IsCandidate = false;
                    Candidate.IsContractor = true;
                    Candidate.IsUser = true;
                    Candidate.Account = UserProfile;
                    _candidate.Update(Candidate);
                    Service.SetActive(item.Id, Candidate.Id, UserProfile.Id);
                }
                else
                {
                    var UserProfile = _profileService.GetByUserId(AppUser.Id);
                    Service.SetActive(item.Id, Candidate.Id, UserProfile.Id);
                }
            }

            // Update General Srf
            if (!string.IsNullOrWhiteSpace(number))
            {
                item.Number = number;
            }
            item.NotesFirst = notes;
            item.Status = SrfStatus.Waiting;
            item.ApproveStatusSix = SrfApproveStatus.Approved;
            item.DateApproveStatusSix = DateTime.Now;
            Service.Update(item);

            // Send Notification To LM
            SendNotif(id, item.ApproveOneId, "Service Request Form is approved by " + _userHelper.GetLoginUser(User).Name + " " + type, "your Service Request Form is approved by " + _userHelper.GetLoginUser(User).Name + " " + type, NotificationInboxStatus.Approval, notes);
            #endregion
        }
        private void MultiApprove(bool status, SrfRequest item, string notes, string id, string type, int OperateOrNon, string Number = null)
        {
            #region MultiUser
            if (status == true)
            {
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                int SendApproval = 0;
                var Approver = UserApprover.LineManager;

                #region LineManagerApprove
                if (User.IsInRole("Line Manager") && item.ApproveOneId == PreofileId)
                {

                    if (item.Status == SrfStatus.Waiting)
                    {
                        if (item.ApproveStatusOne == SrfApproveStatus.Waiting ||
                            item.ApproveStatusOne == SrfApproveStatus.Reject ||
                            item.ApproveStatusTwo == SrfApproveStatus.Reject ||
                            item.ApproveStatusThree == SrfApproveStatus.Reject ||
                            item.ApproveStatusFour == SrfApproveStatus.Reject ||
                            item.ApproveStatusFive == SrfApproveStatus.Waiting ||
                            item.ApproveStatusSix == SrfApproveStatus.Reject)
                        {
                            if (OperateOrNon == 1)
                            {
                                // Send To Head Of Service Line
                                item.ApproveStatusOne = SrfApproveStatus.Approved;
                                item.ApproveFourId = null;
                                item.ApproveTwoId = item.ApproveTwoId;
                                item.ApproveThreeId = item.ApproveThreeId;
                                SendApproval = item.ApproveTwoId.Value;
                            }
                            else
                            {
                                // Send To Non Head Operation
                                item.ApproveStatusOne = SrfApproveStatus.Approved;
                                item.ApproveTwoId = null;
                                item.ApproveThreeId = null;
                                item.ApproveFourId = item.ApproveFourId;
                                SendApproval = item.ApproveFourId.Value;
                            }
                            item.ApproveStatusOne = SrfApproveStatus.Approved;
                            item.DateApproveStatusOne = DateTime.Now;

                            if (item.Status == SrfStatus.Waiting)
                            {
                                if (item.ApproveStatusOne == SrfApproveStatus.Waiting ||
                                    item.ApproveStatusOne == SrfApproveStatus.Reject ||
                                    item.ApproveStatusTwo == SrfApproveStatus.Reject ||
                                    item.ApproveStatusThree == SrfApproveStatus.Reject ||
                                    item.ApproveStatusFour == SrfApproveStatus.Reject ||
                                    item.ApproveStatusFive == SrfApproveStatus.Waiting ||
                                    item.ApproveStatusSix == SrfApproveStatus.Reject)
                                {
                                    if (item.Status == SrfStatus.Waiting)
                                    {
                                        int total_day = Extension.MonthDistance(item.SrfBegin.Value, item.SrfEnd.Value);
                                        item.DateApproveStatusOne = DateTime.Now;

                                        if (item.Type == SrfType.Extension)
                                        {
                                            item.AnnualLeave = item.AnnualLeave + (int)total_day;
                                        }
                                        else
                                        {
                                            item.AnnualLeave = (int)total_day;
                                        }


                                        if (item.ApproveStatusTwo == SrfApproveStatus.Reject)
                                        {
                                            item.ApproveStatusTwo = SrfApproveStatus.Waiting;
                                        }

                                        if (item.ApproveStatusThree == SrfApproveStatus.Reject)
                                        {
                                            item.ApproveStatusThree = SrfApproveStatus.Waiting;
                                        }

                                        if (item.ApproveStatusFour == SrfApproveStatus.Reject)
                                        {
                                            item.ApproveStatusFour = SrfApproveStatus.Waiting;
                                        }

                                        if (item.ApproveStatusSix == SrfApproveStatus.Reject)
                                        {
                                            item.ApproveStatusSix = SrfApproveStatus.Waiting;
                                        }

                                        #region EscalationApprove
                                        // Update Escalation IF Exists
                                        var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                                        if (Escalation != null)
                                        {
                                            Escalation.Status = StatusEscalation.Submitted;
                                            _escalation.Update(Escalation);
                                        }
                                        #endregion

                                    }
                                }
                            }

                        }
                    }
                }
                #endregion

                #region ServiceLine
                if (User.IsInRole("Head Of Service Line") && OperateOrNon == 1 && item.ApproveTwoId == PreofileId)
                {
                    item.NotesFirst = notes;
                    item.Status = SrfStatus.Waiting;
                    item.ApproveStatusTwo = SrfApproveStatus.Approved;
                    item.DateApproveStatusTwo = DateTime.Now;
                    SendApproval = item.ApproveThreeId.Value;

                    #region EscalationApprove
                    // Update Escalation IF Exists
                    var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                    if (Escalation != null)
                    {
                        Escalation.ApproveStatusOne = SrfApproveStatus.Approved;
                        _escalation.Update(Escalation);
                    }
                    #endregion

                    Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region HeadOfOperation
                if (User.IsInRole("Head Of Operation") && OperateOrNon == 1 && item.ApproveThreeId == PreofileId)
                {
                    item.NotesFirst = notes;
                    item.Status = SrfStatus.Waiting;
                    item.ApproveStatusThree = SrfApproveStatus.Approved;
                    item.DateApproveStatusThree = DateTime.Now;
                    SendApproval = item.ApproveSixId.Value;

                    #region EscalationApprove
                    // Update Escalation IF Exists
                    var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                    if (Escalation != null)
                    {
                        Escalation.ApproveStatusTwo = SrfApproveStatus.Approved;
                        _escalation.Update(Escalation);
                    }
                    #endregion
                    Approver = UserApprover.HeadOfOperation;
                }
                #endregion

                #region HeadOfNonOperation
                if (User.IsInRole("Head Of Non Operation") && OperateOrNon == 0 && item.ApproveFourId == PreofileId)
                {
                    item.NotesFirst = notes;
                    item.Status = SrfStatus.Waiting;
                    item.ApproveStatusFour = SrfApproveStatus.Approved;
                    item.DateApproveStatusFour = DateTime.Now;
                    SendApproval = item.ApproveSixId.Value;

                    #region EscalationApprove
                    // Update Escalation IF Exists
                    var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                    if (Escalation != null)
                    {
                        Escalation.ApproveStatusTwo = SrfApproveStatus.Approved;
                        _escalation.Update(Escalation);
                    }
                    #endregion
                    Approver = UserApprover.HeadOfNonOperation;
                }
                #endregion

                #region HeadOfSouircing
                if (User.IsInRole("Head Of Sourcing") && item.ApproveFiveId == PreofileId)
                {
                    var Esc = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                    if (Esc != null)
                    {
                        // Update General Srf
                        item.NotesFirst = notes;
                        item.Status = SrfStatus.Waiting;
                        item.ApproveStatusFive = SrfApproveStatus.Approved;
                        item.DateApproveStatusFive = DateTime.Now;
                        SendApproval = item.ApproveSixId.Value;

                        #region EscalationApprove
                        // Update Escalation IF Exists
                        Esc.ApproveStatusThree = SrfApproveStatus.Approved;
                        _escalation.Update(Esc);
                        #endregion
                        Approver = UserApprover.HeadOfSourcing;
                    }
                }
                #endregion

                #region ServiceCoordinator
                if (User.IsInRole("Service Coordinator") && item.ApproveSixId == PreofileId)
                {
                    UpdateUser(item, notes, id, type, Number);
                    #region EscalationApprove
                    // Update Escalation IF Exists
                    var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                    if (Escalation != null)
                    {
                        Escalation.ApproveStatusFour = SrfApproveStatus.Approved;
                        _escalation.Update(Escalation);
                    }
                    #endregion
                    Approver = UserApprover.ServiceCoordinator;
                }
                else
                {
                    if (SendApproval != 0)
                    {
                        // Send Email To Approver
                        var LoginUser = _userHelper.GetLoginUser(User);
                        var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                        var Notif = "";
                        if (Escalation != null)
                        {
                            Notif = "( Escalation )";
                        }

                        if (Approver == UserApprover.LineManager)
                        {
                            SendNotif(id, SendApproval, "New Service Request Form is Submmited " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Submmited " + type + " By " + LoginUser.Name, NotificationInboxStatus.Approval, notes);
                        }
                        else
                        {
                            SendNotif(id, SendApproval, "New Service Request Form is Approved  " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Approved " + type + " By " + LoginUser.Name, NotificationInboxStatus.Approval, notes);
                        }
                        Service.Update(item);
                    }
                }
                #endregion
            }
            else
            {
                // Send Notif Rejected
                item.Status = SrfStatus.Waiting;
                item.ApproveStatusOne = SrfApproveStatus.Waiting;
                item.ApproveStatusTwo = SrfApproveStatus.Waiting;
                item.ApproveStatusThree = SrfApproveStatus.Waiting;
                item.ApproveStatusFour = SrfApproveStatus.Waiting;
                item.ApproveStatusFive = SrfApproveStatus.Waiting;
                item.ApproveStatusSix = SrfApproveStatus.Waiting;
                Service.Update(item);

                // Send Email To Approver
                var LoginUser = _userHelper.GetLoginUser(User);
                var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
                var Notif = "";
                if (Escalation != null)
                {
                    Notif = "( Escalation )";
                }

                SendNotif(id, item.ApproveOneId, "New Service Request Form is Rejected " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Approved " + type + " By " + LoginUser.Name, NotificationInboxStatus.Reject, notes);
            }
            #endregion
        }

        public IActionResult TestNumber()
        {
            return Content(Service.GenerateNumnber() + "");
        }

    }
}
