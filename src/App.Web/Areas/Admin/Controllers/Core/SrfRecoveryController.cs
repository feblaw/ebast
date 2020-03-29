using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Domain.Models.Enum;

namespace App.Web.Areas.Admin.Controllers.Core
{

    [Authorize]
    [Area("Admin")]
    public class SrfRecoveryController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, RecoveryForm, string>
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
        private readonly IUserHelper _userHelper;
        private readonly ICostCenterService _costCenter;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly IUserProfileService _profileService;
        private readonly ISrfMigrationService _migrate;
        private readonly IPackageTypeService _packageType;
        private readonly IActivityCodeService _activity;

        public SrfRecoveryController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISrfRequestService service,
            IVacancyListService vacancy,
            ICandidateInfoService candidate,
            IDepartementService department,
            IServicePackService ssow,
            IServicePackCategoryService ssowCategory,
            IJobStageService jobStage,
            IDepartementSubService departmentSub,
            INetworkNumberService network,
            IAccountNameService account,
            ICostCenterService costCenter,
            ISrfEscalationRequestService escalation,
            IUserProfileService profileService,
            ISrfMigrationService migrate,
            IActivityCodeService activity,
            IPackageTypeService packageType,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _vacancy = vacancy;
            _candidate = candidate;
            _department = department;
            _ssow = ssow;
            _ssowCategory = ssowCategory;
            _departmentSub = departmentSub;
            _network = network;
            _account = account;
            _userHelper = userHelper;
            _costCenter = costCenter;
            _escalation = escalation;
            _profileService = profileService;
            _migrate = migrate;
            _packageType = packageType;
            _jobStage = jobStage;
            _activity = activity;
        }

        public override IActionResult Edit(string id)
        {
            var UserProfile = _profileService.GetByUserId(id);
            Dictionary<string, int> ws = new Dictionary<string, int>();
            ws.Add("No", 0);
            ws.Add("Yes", 1);
            Dictionary<string, bool> com = new Dictionary<string, bool>();
            com.Add("No USIM", false);
            com.Add("USIM", true);
            Dictionary<string, bool> sign = new Dictionary<string, bool>();
            sign.Add("Non-HRMS", false);
            sign.Add("HRMS", true);
            ViewBag.Profile = UserProfile.Id;
            ViewBag.OrganizationUnit = _department.GetAll().ToList();
            ViewBag.AccountName = _account.GetAll().ToList();
            ViewBag.PackageType = _packageType.GetAll().ToList();
            ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
            ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });
            ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.Signum = sign.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.Jobstage = _jobStage.GetAll().Where(x => !string.IsNullOrEmpty(x.Description)).ToList();
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            ViewBag.Sourcing = _userHelper.GetByRoleName("Sourcing").ToList();
            ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
            ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
            ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
            ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
            ViewBag.ListAgency = _userHelper.GetByRoleName("HR Agency").ToList();
            ViewBag.CandidateName = UserProfile.Name;
            ViewBag.CanidateEmail = UserProfile.Email;

            var Model = new RecoveryForm();
            return View(Model);
        }
        public override IActionResult Edit(string id, RecoveryForm model)
        {
            if(ModelState.IsValid)
            {
                var User = _userHelper.GetUser(id);
                if(User!=null)
                {
                    // Insert Vacancy
                    var NewVacancy = new VacancyList();
                    NewVacancy.AccountNameId = model.AccountNameId;
                    NewVacancy.ApproverOneId = model.ApproverOneId;
                    NewVacancy.ApproverTwoId = model.SourcingId;
                    NewVacancy.CostCodeId = model.CostCodeId;
                    NewVacancy.CreatedAt = DateTime.Now;
                    NewVacancy.DepartmentId = model.DepartmentId;
                    NewVacancy.DepartmentSubId = model.DepartmentSubId;
                    NewVacancy.JobStageId = model.JobStageId;
                    NewVacancy.JoinDate = model.JoinDate;
                    NewVacancy.NetworkId = model.NetworkId;
                    NewVacancy.NoarmalRate = model.NoarmalRate;
                    NewVacancy.OtLevel = model.OtLevel;
                    NewVacancy.RequestBy = _userHelper.GetUserProfile(model.ApproverOneId);
                    NewVacancy.PackageTypeId = model.PackageTypeId;
                    NewVacancy.ServicePackCategoryId = model.ServicePackCategoryId;
                    NewVacancy.ServicePackId = model.ServicePackId;
                    //NewVacancy.Status = VacanStatusFive.Done;
                    NewVacancy.VacancyStatus = ApproverStatus.Completed;
                    NewVacancy.isHrms = model.isHrms;
                    NewVacancy.isLaptop = model.isLaptop == 1 ? true : false;
                    NewVacancy.isManager = model.isManager;
                    NewVacancy.isUsim = model.isUsim;
                    NewVacancy.StatusOne = SrfApproveStatus.Approved;
                    NewVacancy.StatusTwo = SrfApproveStatus.Approved;
                    NewVacancy.StatusThree = SrfApproveStatus.Approved;
                    NewVacancy.StatusFourth = SrfApproveStatus.Approved;
                    _vacancy.Add(NewVacancy);

                    if(NewVacancy.Id!=null)
                    {
                        // Insert Candidate
                        var UserProfile = _profileService.GetByUserId(id);
                        var GetCandidate = _candidate.GetAll().Where(x => x.AccountId == UserProfile.Id).FirstOrDefault();

                        if(GetCandidate==null)
                        {
                            var Candidate = new CandidateInfo();
                            Candidate.Account = UserProfile;
                            Candidate.Address = UserProfile.Address;
                            Candidate.AgencyId = model.AgencyId;
                            Candidate.AgencyType = AgencyType.Agency;
                            Candidate.ApproveOneDate = DateTime.Now;
                            Candidate.ApproveOneStatus = ApproverStatus.Selected;
                            Candidate.ApproveTwoeDate = DateTime.Now;
                            Candidate.CreatedAt = DateTime.Now;
                            Candidate.Email = User.Email;
                            Candidate.Gender = UserProfile.Gender.Value;
                            Candidate.HomePhoneNumber = UserProfile.HomePhoneNumber;
                            Candidate.IdNumber = UserProfile.IdNumber;
                            Candidate.IsCandidate = false;
                            Candidate.IsContractor = true;
                            Candidate.IsUser = true;
                            Candidate.Martial = Martial.M1;
                            Candidate.MobilePhoneNumber = UserProfile.MobilePhoneNumber;
                            Candidate.Name = UserProfile.Name;
                            Candidate.Nationality = "Indonesia";
                            Candidate.NickName = UserProfile.Name;
                            Candidate.PlaceOfBirth = UserProfile.Birthplace;
                            Candidate.DateOfBirth = UserProfile.Birthdate.Value;
                            Candidate.RequestBy = _userHelper.GetUserProfile(model.AgencyId);
                            Candidate.ApproveTwoStatus = ApproverStatus.Completed;
                            Candidate.Vacancy = NewVacancy;
                            GetCandidate = _candidate.Add(Candidate);
                        }

                        if(GetCandidate != null)
                        {
                            var Department = _department.GetById(model.DepartmentId);
                            var Network = _network.GetById(model.NetworkId);
                            // Insert SRF
                            SrfRequest NewSrf = new SrfRequest();
                            NewSrf.CreatedAt = DateTime.Now;
                            NewSrf.Number = "0000";
                            NewSrf.Type =  SrfType.New;
                            NewSrf.ApproveStatusOne = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusTwo = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusThree = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusFour = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusFive = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusSix = SrfApproveStatus.Approved;
                            NewSrf.RequestBy = _userHelper.GetUserProfile(model.ApproverOneId).Name;
                            NewSrf.SrfBegin = model.SrfBegin;
                            NewSrf.SrfEnd = model.SrfEnd;
                            NewSrf.ServiceLevel = model.OtLevel;
                            NewSrf.isWorkstation = model.isLaptop == 1 ? true  : false;
                            NewSrf.isCommunication = model.isUsim;
                            NewSrf.IsHrms = model.isHrms;
                            NewSrf.IsOps = Department.OperateOrNon == 1 ? true : false;
                            NewSrf.IsManager = false;
                            NewSrf.RateType = Domain.Models.Enum.RateType.Normal;
                            NewSrf.IsExtended = false;
                            NewSrf.IsLocked = false;
                            NewSrf.Status = Domain.Models.Enum.SrfStatus.Done;
                            NewSrf.SpectValue = 0;
                            NewSrf.IsActive = false;
                            NewSrf.ServicePackId = model.ServicePackId;
                            NewSrf.NetworkId = model.NetworkId;
                            NewSrf.CostCenterId = model.CostCodeId;
                            NewSrf.LineManagerId = model.ApproverOneId;
                            NewSrf.ActivityCode = _activity.GetAll().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                            NewSrf.DepartmentId = model.DepartmentId;
                            NewSrf.DepartmentSubId = model.DepartmentSubId;
                            NewSrf.ProjectManagerId = Network.ProjectManagerId;
                            NewSrf.ApproveOneId = model.ApproverOneId;
                            NewSrf.ApproveTwoId = model.ApproveTwoId;
                            NewSrf.ApproveThreeId = model.ApproveThreeId;
                            NewSrf.ApproveFourId = model.ApproveFourId;
                            NewSrf.ApproveSixId = model.ApproveSixId;
                            NewSrf.Candidate = GetCandidate;
                            NewSrf.AccountId = model.AccountNameId;
                            NewSrf.IsLocked = false;
                            NewSrf.IsActive = true;
                            var AnnualLeave = MonthDistance(model.SrfBegin, model.SrfEnd);
                            NewSrf.AnnualLeave = AnnualLeave;
                            Service.Add(NewSrf);
                            if(NewSrf.Id!=null)
                            {
                                TempData["Success"] = true;
                            }
                        } 
                    }  
                }
            }
            return RedirectToAction("Index");
        }

        private int MonthDistance(DateTime first, DateTime last)
        {
            return Math.Abs((first.Month - last.Month) + 12 * (first.Year - last.Year));
        }
    }
}
