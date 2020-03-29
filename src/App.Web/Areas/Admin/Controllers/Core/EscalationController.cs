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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class EscalationController : BaseController<SrfEscalationRequest, ISrfEscalationRequestService, EscalationViewModel, EscalationModelForm, Guid>
    {
        private readonly IVacancyListService _vacancy;
        private readonly ICandidateInfoService _candidate;
        private readonly IDepartementService _department;
        private readonly IPackageTypeService _packageType;
        private readonly IServicePackService _ssow;
        private readonly IServicePackCategoryService _ssowCategory;
        private readonly IJobStageService _jobStage;
        private readonly IDepartementSubService _departmentSub;
        private readonly IUserProfileService _profileService;
        private readonly INetworkNumberService _network;
        private readonly IAccountNameService _account;
        private readonly IHostingEnvironment _env;
        private readonly ISrfRequestService _srf;
        private readonly NotifHelper _notif;
        private readonly HostConfiguration _hostConfiguration;
        private readonly MailingHelper _mailingHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICostCenterService _costCenter;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly FileHelper _fileHelper;
        private readonly ConfigHelper _config;

        public EscalationController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ISrfRequestService srf,
            ISrfEscalationRequestService service,
            IVacancyListService vacancy,
            ICandidateInfoService candidate,
            IDepartementService department,
            IHostingEnvironment env,
            IServicePackService ssow,
            IPackageTypeService packageType,
            ICostCenterService costCenter,
            MailingHelper mailingHelper,
            IOptions<HostConfiguration> hostConfiguration,
            NotifHelper notif,
            IDepartementSubService departmentSub,
            IServicePackCategoryService ssowCategory,
            IJobStageService jobsTage,
            IUserProfileService profileService,
            INetworkNumberService network,
            IAccountNameService account,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            FileHelper fileHelper,
            ConfigHelper config,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _srf = srf;
            _vacancy = vacancy;
            _candidate = candidate;
            _department = department;
            _packageType = packageType;
            _ssow = ssow;
            _ssowCategory = ssowCategory;
            _jobStage = jobsTage;
            _departmentSub = departmentSub;
            _profileService = profileService;
            _network = network;
            _account = account;
            _env = env;
            _notif = notif;
            _hostConfiguration = hostConfiguration.Value;
            _mailingHelper = mailingHelper;
            _userHelper = userHelper;
            _costCenter = costCenter;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileHelper = fileHelper;
            _config = config;
        }

      
        [HttpGet]
        public IActionResult Add(Guid id)
        {
            try
            {
                var srf = _srf.GetById(id);
                var Candidate = _candidate.GetById(srf.CandidateId);
                var CurrentSSO = _ssow.GetById(srf.ServicePackId);
                var Departement = _department.GetById(srf.DepartmentId);
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                
                ViewBag.Candidate = Candidate;
                ViewBag.Vacancy = Vacancy;
                ViewBag.Departement = Departement;
                ViewBag.SSOW = CurrentSSO;
                ViewBag.SCategory = _ssowCategory.GetById(CurrentSSO.ServicePackCategoryId);
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
                ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
                ViewBag.ListHeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.ListHeadNonOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                ViewBag.ListHeadSourcing = _userHelper.GetByRoleName("Head Of Sourcing").ToList();
                ViewBag.ListAccount = _account.GetAll().ToList();
                ViewBag.ListSubDepartment = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId) && x.IsClosed == false).ToList();
                //ViewBag.ListSSOW = _ssow.GetAll().Where(x => x.Type.Equals(CurrentSSO.Type)).ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });


                Dictionary<string, bool> ws = new Dictionary<string, bool>();
                ws.Add("No", false);
                ws.Add("Yes", true);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

                var ProfileId = _userHelper.GetLoginUser(User).Id;
                ViewBag.ProfileId = ProfileId;

                if (srf != null)
                {
                    var Model = new EscalationModelForm();
                    Model.Srf = Mapper.Map<SrfRequestModelForm>(srf);
                    Model.SrfId = id;
                    return View(Model);
                }
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }
            return NoContent();

        }

        [HttpPost]
        public IActionResult Add(EscalationModelForm model)
        {
            if(ModelState.IsValid)
            {
                var srf = _srf.GetById(model.SrfId);
                var Dept = _department.GetById(srf.DepartmentId);

                if (srf!=null)
                {
                   
                    var item = Mapper.Map<SrfEscalationRequest>(model);
                    int ProfileId = 0;
                    if (Dept.OperateOrNon == 1)
                    {
                        ProfileId = model.Srf.ApproveTwoId;
                    }
                    else
                    {
                        ProfileId = model.Srf.ApproveFourId;
                    }
                    item.OtLevel = model.OtLevel;
                    item.IsWorkstation = model.IsWorkstation;
                    item.IsCommnunication = model.IsCommnunication;
                    item.SparateValue = model.SparateValue;
                    item.Note = model.Note;
                    item.ServicePackId = model.ServicePackId;
                    item.Status = StatusEscalation.Waiting;

                    //nitip
                    item.CustomField1 = model.PackageTypeId?.ToString();
                    item.CustomField2 = model.ServicePackCategoryId?.ToString();

                    if (!string.IsNullOrWhiteSpace(model.Files))
                    {
                        string fileDestination = "uploads/escalation";
                        var AttachmentUpload = model.Files.Split('|');
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
                                item.Files = JsonConvert.SerializeObject(listSaveAttachment);
                            }
                        }
                    }


                    Service.Add(item);


                    // Update Srf General
                    srf.SrfBegin = model.Srf.SrfBegin;
                    srf.SrfEnd = model.Srf.SrfEnd;
                    srf.AnnualLeave = Extension.MonthDistance(model.Srf.SrfBegin, model.Srf.SrfEnd);
                    srf.DepartmentId = model.Srf.DepartmentId;
                    srf.DepartmentSubId = model.Srf.DepartmentSubId;
                    srf.CostCenterId = model.Srf.CostCenterId;
                    srf.AccountId = model.Srf.AccountId;
                    srf.NetworkId = model.Srf.NetworkId;

                    if(model.Srf.ApproveTwoId > 0)
                    {
                        srf.ApproveTwoId = model.Srf.ApproveTwoId;
                    }

                    if (model.Srf.ApproveThreeId > 0)
                    {
                        srf.ApproveThreeId = model.Srf.ApproveThreeId;
                    }

                    if (model.Srf.ApproveFourId > 0)
                    {
                        srf.ApproveFourId = model.Srf.ApproveFourId;
                    }

                    srf.ApproveFiveId = model.Srf.ApproveFiveId;
                    srf.ApproveSixId = model.Srf.ApproveSixId;
                    srf.SpectValue = model.SparateValue;
                    if(model.SparateValue > 0)
                    {
                        srf.RateType = RateType.SpecialRate;
                    }
                    else
                    {
                        srf.RateType = RateType.Normal;
                    }
                    
                    _srf.Update(srf);

                    // Update Vacancy
                    var Candidate = _candidate.GetById(srf.CandidateId);
                    if (Candidate != null)
                    {
                        var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                        if (Vacancy != null)
                        {
                            Vacancy.AccountNameId = srf.AccountId.Value;
                            Vacancy.CostCodeId = srf.CostCenterId;
                            Vacancy.DepartmentId = srf.DepartmentId;
                            Vacancy.DepartmentSubId = srf.DepartmentSubId;
                            Vacancy.NetworkId = srf.NetworkId;
                            Vacancy.ApproverOneId = srf.ApproveOneId;
                            Vacancy.ServicePackId = srf.ServicePackId;
                            Vacancy.isLaptop = srf.isWorkstation;
                            Vacancy.isUsim = srf.isCommunication;
                            Vacancy.isHrms = srf.IsHrms;
                            Vacancy.OtLevel = model.OtLevel;
                            _vacancy.Update(Vacancy);
                        }
                    }

                    if (model.Status == StatusEscalation.Submitted)
                    {
                        // Do Approve
                        MultiApprove(true, srf, model.Note, srf.Id.ToString(), srf.Type == SrfType.Extension ? "( Extended )" : "", Dept.OperateOrNon);
                    }

                    TempData["Saved"] = "OK";
                    return RedirectToAction("Index", "Srf");
                }
            }
            TempData["Error"] = string.Join(",", ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors).Select(E => E.ErrorMessage).ToArray());
            return RedirectToAction("Add", "Escalation", new { id = model.SrfId });
        }

        [HttpGet]
        public override IActionResult Edit(Guid id)
        {
            try
            {
                var srf = _srf.GetById(id);
                var item = Service.GetAll().Where(x => x.SrfId.Equals(id)).FirstOrDefault();
                var Candidate = _candidate.GetById(srf.CandidateId);
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                var CurrentSSO = _ssow.GetById(srf.ServicePackId);
                var Departement = _department.GetById(srf.DepartmentId);
                ViewBag.Candidate = Candidate;
                ViewBag.Vacancy = Vacancy;
                ViewBag.Departement = Departement;
                ViewBag.SSOW = CurrentSSO;
                ViewBag.SCategory = _ssowCategory.GetById(CurrentSSO.ServicePackCategoryId);
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
                ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
                ViewBag.ListHeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.ListHeadNonOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                ViewBag.ListHeadSourcing = _userHelper.GetByRoleName("Head Of Sourcing").ToList();
                ViewBag.ListAccount = _account.GetAll().ToList();
                ViewBag.ListSubDepartment = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                //ViewBag.ListSSOW = _ssow.GetAll().Where(x => x.Type.Equals(CurrentSSO.Type)).ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });
             

                Dictionary<string, bool> ws = new Dictionary<string, bool>();
                ws.Add("No", false);
                ws.Add("Yes", true);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.EscalationId = item.Id;
                ViewBag.SrfId = item.SrfId;
                ViewBag.ServiceCode = _ssow.GetById(item.ServicePackId).Code;
                ViewBag.SrfNumber = _srf.GenerateNumnber();

                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                ViewBag.PreofileId = PreofileId;
                ViewBag.LineManagerId = srf.ApproveOneId;
                ViewBag.ServiceLineId = srf.ApproveTwoId;
                ViewBag.HeadOpId = srf.ApproveThreeId;
                ViewBag.HeadNonId = srf.ApproveFourId;
                ViewBag.HeadOfSourceId = srf.ApproveFiveId;
                ViewBag.ServiceCoordId = srf.ApproveSixId;

                if (User.IsInRole("Line Manager") || User.IsInRole("Administrator") || User.IsInRole("Sourcing"))
                {
                    ViewBag.FormDisable = 0;
                }
                else if (User.IsInRole("Line Manager") && item.Status == StatusEscalation.Submitted)
                {
                    ViewBag.FormDisable = 1;
                }
                else if (User.IsInRole("Head Of Service Line") || User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation") || User.IsInRole("Service Coordinator") || User.IsInRole("Head Of Sourcing"))
                {
                    ViewBag.FormDisable = 1;
                }
                else
                {
                    ViewBag.FormDisable = 1;
                }

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

                if (srf != null)
                {
                    var Model = Mapper.Map<EscalationModelForm>(item);
                    Model.Srf = Mapper.Map<SrfRequestModelForm>(srf);
                    Model.SrfId = id;
                    string typeName = CurrentSSO.Type.ToString();
                    Guid packageTypeId, servicePackCategoryId;
                    if (Guid.TryParse(item.CustomField1, out packageTypeId))
                        Model.PackageTypeId = packageTypeId;
                    if (Guid.TryParse(item.CustomField2, out servicePackCategoryId))
                        Model.ServicePackCategoryId = servicePackCategoryId;
                    if (!string.IsNullOrEmpty(item.Files))
                    {
                        var files = JsonConvert.DeserializeObject<List<string>>(item.Files)
                            ?? new List<string>();
                        ViewBag.Files = files;
                        Model.Files = string.Join("|", files.ToArray()) + "|";
                    }
                    else
                    {
                        Model.Files = null;
                    }
                    return View(Model);
                }

            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Update(EscalationModelForm model)
        {
            if (ModelState.IsValid)
            {
                var srf = _srf.GetById(model.SrfId);
                if (srf != null)
                {
                    var item = Service.GetAll().Where(x => x.SrfId.Equals(model.SrfId)).FirstOrDefault();
                    var Dept = _department.GetById(srf.DepartmentId);
                    int ProfileId = 0;
                    if (Dept.OperateOrNon == 1)
                    {
                        ProfileId = model.Srf.ApproveTwoId;
                    }
                    else
                    {
                        ProfileId = model.Srf.ApproveFourId;
                    }
                    item.OtLevel = model.OtLevel;
                    item.IsWorkstation = model.IsWorkstation;
                    item.IsCommnunication = model.IsCommnunication;
                    item.SparateValue = model.SparateValue;
                    item.Note = model.Note;
                    item.ServicePackId = model.ServicePackId;
                    item.Status = StatusEscalation.Waiting;

                    //nitip
                    item.CustomField1 = model.PackageTypeId?.ToString();
                    item.CustomField2 = model.ServicePackCategoryId?.ToString();

                    if (!string.IsNullOrWhiteSpace(model.Files))
                    {
                        var split = model.Files.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        item.Files = JsonConvert.SerializeObject(split);
                    }

                    Service.Update(item);

                    // Update Srf General
                    srf.SrfBegin = model.Srf.SrfBegin;
                    srf.SrfEnd = model.Srf.SrfEnd;
                    srf.AnnualLeave = Extension.MonthDistance(model.Srf.SrfBegin, model.Srf.SrfEnd);
                    srf.DepartmentId = model.Srf.DepartmentId;
                    srf.DepartmentSubId = model.Srf.DepartmentSubId;
                    srf.CostCenterId = model.Srf.CostCenterId;
                    srf.AccountId = model.Srf.AccountId;
                    srf.NetworkId = model.Srf.NetworkId;

                    if (model.Srf.ApproveTwoId > 0)
                    {
                        srf.ApproveTwoId = model.Srf.ApproveTwoId;
                    }

                    if (model.Srf.ApproveThreeId > 0)
                    {
                        srf.ApproveThreeId = model.Srf.ApproveThreeId;
                    }

                    if (model.Srf.ApproveFourId > 0)
                    {
                        srf.ApproveFourId = model.Srf.ApproveFourId;
                    }

                    srf.ApproveFiveId = model.Srf.ApproveFiveId;
                    srf.ApproveSixId = model.Srf.ApproveSixId;
                    srf.SpectValue = model.SparateValue;
                    if (model.SparateValue > 0)
                    {
                        srf.RateType = RateType.SpecialRate;
                    }
                    else
                    {
                        srf.RateType = RateType.Normal;
                    }
                    
                    _srf.Update(srf);

                    // Update Vacancy
                    var Candidate = _candidate.GetById(srf.CandidateId);
                    if (Candidate != null)
                    {
                        var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                        if (Vacancy != null)
                        {
                            Vacancy.AccountNameId = srf.AccountId.Value;
                            Vacancy.CostCodeId = srf.CostCenterId;
                            Vacancy.DepartmentId = srf.DepartmentId;
                            Vacancy.DepartmentSubId = srf.DepartmentSubId;
                            Vacancy.NetworkId = srf.NetworkId;
                            Vacancy.ApproverOneId = srf.ApproveOneId;
                            Vacancy.ServicePackId = srf.ServicePackId;
                            Vacancy.isLaptop = srf.isWorkstation;
                            Vacancy.isUsim = srf.isCommunication;
                            Vacancy.isHrms = srf.IsHrms;
                            Vacancy.OtLevel = model.OtLevel;
                            _vacancy.Update(Vacancy);
                        }
                    }

                    if (model.Status == StatusEscalation.Submitted)
                    {
                        // Do Approve
                        MultiApprove(true, srf, model.Note, srf.Id.ToString(), srf.Type == SrfType.Extension ? "( Extended )" : "", Dept.OperateOrNon);
                    }

                    TempData["Saved"] = "OK";
                    return RedirectToAction("Index", "Srf");
                }

            }
            TempData["Error"] = string.Join(",", ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors).Select(E => E.ErrorMessage).ToArray());
            return RedirectToAction("Edit", "Escalation", new { id = model.SrfId });
        }


        [HttpPost]
        public IActionResult Approval(string Id, bool ApprovalStatus, string ApprovalNotes = null,string Number = null)
        {
            var item = Service.GetById(Guid.Parse(Id));
            var Srf = _srf.GetById(item.SrfId);
            if (item!=null && Srf!=null)
            {
                var Dept = _department.GetById(Srf.DepartmentId);
                
            }
           if(ApprovalStatus==true)
           {
                TempData["Approved"] = "OK";
           }
           else
           {
                TempData["Rejected"] = "OK";
           }
           return RedirectToAction("Index", "Srf");
        }

        private void SendNotifReject(SrfRequest Srf,string Notes,string Roles)
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
                   "Service Request Form is rejected by "+Roles+" (Escalation)", // Subject
                   AppProfile.Name, // User target name
                   AppUser.Email, // User target email
                   callbackUrl, // Link CallBack
                   "Service Request Form is rejected by " + Roles + " (Escalation)", // Email content or descriptions
                   Notes, // Description
                   NotificationInboxStatus.Reject, // Notif Status
                   Activities.SRF // Activity Status
               );
            }
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

        private void MultiApprove(bool status, SrfRequest item, string notes, string id, string type, int OperateOrNon)
        {
            var Escalation = Service.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
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
                                        TempData["Success"] = "OK";

                                        #region EscalationApprove
                                        // Update Escalation IF Exists
                                        Escalation.Status = StatusEscalation.Submitted;
                                        Service.Update(Escalation);
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
                    item.ApproveStatusOne = SrfApproveStatus.Approved;
                    Service.Update(Escalation);
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
                   
                    if (Escalation != null)
                    {
                        Escalation.ApproveStatusTwo = SrfApproveStatus.Approved;
                        Service.Update(Escalation);
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
                    Escalation.ApproveStatusTwo = SrfApproveStatus.Approved;
                    Service.Update(Escalation);
                    #endregion
                    Approver = UserApprover.HeadOfNonOperation;
                }
                #endregion

                #region HeadOfSouircing
                if (User.IsInRole("Head Of Sourcing") && item.ApproveFiveId == PreofileId)
                {
                    var Esc = Service.GetAll().Where(x => x.SrfId.Equals(item.Id)).FirstOrDefault();
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
                        Service.Update(Esc);
                        #endregion
                        Approver = UserApprover.HeadOfSourcing;
                    }
                }
                #endregion

                #region ServiceCoordinator
                if (User.IsInRole("Service Coordinator") && item.ApproveSixId == PreofileId)
                {
                    UpdateUser(item, notes, id, type);
                    #region EscalationApprove
                    // Update Escalation IF Exists
                    Escalation.ApproveStatusFour = SrfApproveStatus.Approved;
                    Service.Update(Escalation);
                    #endregion
                    Approver = UserApprover.ServiceCoordinator;
                }
                else
                {
                    if (SendApproval != 0)
                    {
                        // Send Email To Approver
                        var LoginUser = _userHelper.GetLoginUser(User);
                        var Notif = "( Escalation )";
                        
                        if (Approver == UserApprover.LineManager)
                        {
                            SendNotif(id, SendApproval, "New Service Request Form is Submmited " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Submmited " + type + " By " + LoginUser.Name, NotificationInboxStatus.Approval, notes);
                        }
                        else
                        {
                            SendNotif(id, SendApproval, "New Service Request Form is Approved  " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Approved " + type + " By " + LoginUser.Name, NotificationInboxStatus.Approval, notes);
                        }
                        _srf.Update(item);
                    }
                }
                #endregion
            }
            else
            {
                Escalation.ApproveStatusOne = SrfApproveStatus.Waiting;
                Escalation.ApproveStatusTwo = SrfApproveStatus.Waiting;
                Escalation.ApproveStatusThree = SrfApproveStatus.Waiting;
                Escalation.ApproveStatusFour = SrfApproveStatus.Waiting;
                Escalation.Status = StatusEscalation.Waiting;
                Service.Update(Escalation);
                // Send Email To Approver
                var LoginUser = _userHelper.GetLoginUser(User);
                var Notif = "( Escalation )";
                SendNotif(id, item.ApproveOneId, "New Service Request Form is Rejected " + Notif + " " + type + " By " + LoginUser.Name, "New Service Request Form is Approved " + type + " By " + LoginUser.Name, NotificationInboxStatus.Reject, notes);
            }
            #endregion
        }
        private void UpdateUser(SrfRequest item, string notes, string id, string type)
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
                        var callbackUrl = Url.Action("ConfirmEmail",
                            "Account",
                         new { userId = user.Id, code = code, area = "" },
                        _hostConfiguration.Protocol,
                         _hostConfiguration.Name);

                        var additionalData = new Dictionary<string, string>()
                                {
                                    { "CallbackUrl", callbackUrl },
                                    { "Name", Candidate.Name },
                                    { "Email", Candidate.Email },
                                    { "Password", defaultPassword }
                                };

                        var subject = "You have been registered, please confirm your account, Leave to be taken cannot compensate";

                        var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                            Candidate.Email,
                            subject,
                            "Emails/RegisterUser",
                            user,
                            additionalData,
                            null);

                        // Send Email Confirmation To User
                        var emailResult = _mailingHelper.SendEmail(email).Result;

                        // Update Candidate
                        Candidate.IsCandidate = false;
                        Candidate.IsContractor = true;
                        Candidate.IsUser = true;
                        Candidate.Account = user.UserProfile;
                        _candidate.Update(Candidate);
                        _srf.SetActive(item.Id, Candidate.Id, user.UserProfile.Id);
                    }
                }

            }
            else
            {
                if (item.Type == SrfType.New)
                {
                    // Current email registered
                    var UserProfile = _profileService.GetByUserId(AppUser.Id);
                    var callbackUrl = Url.Action("Index",
                        "Home",
                     new { area = "Admin" },
                    _hostConfiguration.Protocol,
                     _hostConfiguration.Name);

                    var additionalData = new Dictionary<string, string>()
                                {
                                    { "CallbackUrl", callbackUrl },
                                    { "Name", Candidate.Name },
                                    { "Email", Candidate.Email },
                                    { "Password", defaultPassword }
                                };

                    var subject = "You have been registered, please confirm your account, Leave to be taken cannot compensate";

                    var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                        Candidate.Email,
                        subject,
                        "Emails/RegisterUser",
                        AppUser,
                        additionalData,
                        null);

                    // Send Email Confirmation To User
                    var emailResult = _mailingHelper.SendEmail(email).Result;

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
                    _srf.SetActive(item.Id, Candidate.Id, UserProfile.Id);
                }
                else
                {
                    var UserProfile = _profileService.GetByUserId(AppUser.Id);
                    _srf.SetActive(item.Id, Candidate.Id, UserProfile.Id);
                }
            }

            if(User.IsInRole("Service Coordinator"))
            {
                var Number = _srf.GenerateNumnber();
                item.Number = Number;
            }

            // Update General Srf
            item.NotesFirst = notes;
            item.Status = SrfStatus.Waiting;
            item.ApproveStatusSix = SrfApproveStatus.Approved;
            item.DateApproveStatusSix = DateTime.Now;
            _srf.Update(item);

            // Send Notification To LM
            SendNotif(id, item.ApproveOneId, "Service Request Form is approved (Escalation) by " + _userHelper.GetLoginUser(User).Name + " " + type, "your Service Request Form is approved (Escalation) by " + _userHelper.GetLoginUser(User).Name + " " + type, NotificationInboxStatus.Approval, notes);
            #endregion
        }

    }
}
