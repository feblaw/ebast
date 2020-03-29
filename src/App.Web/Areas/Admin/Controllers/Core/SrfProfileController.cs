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
using Newtonsoft.Json;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Authorize]
    [Area("Admin")]
    public class SrfProfileController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
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

        public SrfProfileController(
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
            IUserHelper userHelper,
            ICostCenterService costCenter,
            ISrfEscalationRequestService escalation,
            IUserProfileService profileService,
            ISrfMigrationService migrate,
            IActivityCodeService activity,
            IPackageTypeService packageType) : 
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


        public override IActionResult Edit(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var Vacancy = _vacancy.GetById(_candidate.GetById(item.CandidateId).VacancyId);
                var Departement = _department.GetById(item.DepartmentId);
                var PackageType = _packageType.GetById(Vacancy.PackageTypeId);
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
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(item.DepartmentId)).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(item.DepartmentId) && x.IsClosed == false).ToList();
                ViewBag.Id = id;
                ViewBag.isOperation = Departement.OperateOrNon;
                ViewBag.txtOperaion = (Departement.OperateOrNon == 1) ? "Operational" : "Non Operational";
                ViewBag.FormDisable = 1;
                ViewBag.SrfNumber = Service.GenerateNumnber();
                ViewBag.NowYear = DateTime.Now.Year.ToString("yy");

                // Aditional
                Dictionary<string, int> ws = new Dictionary<string, int>();
                ws.Add("No", 0);
                ws.Add("Yes", 1);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                Dictionary<string, bool> sign = new Dictionary<string, bool>();
                sign.Add("Non-HRMS", false);
                sign.Add("HRMS", true);

                Dictionary<string, bool> manager = new Dictionary<string, bool>();
                manager.Add("Manager", true);
                manager.Add("Non Manager", false);

                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
                ViewBag.ServicePack = _ssow.GetAll().Where(x => x.Type == (PackageTypes)Enum.Parse(typeof(PackageTypes), PackageType.Name)).ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });
                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Signum = sign.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.IsManager = manager.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Jobstage = _jobStage.GetAll().Where(x => !string.IsNullOrEmpty(x.Description)).ToList();
                ViewBag.ListAgency = _userHelper.GetByRoleName("HR Agency").ToList();
                ViewBag.ListActivity = _activity.GetAll().ToList();
                ViewBag.ListExtended = Service.GetAll().Where(x => x.CandidateId.Equals(item.CandidateId) && x.SrfEnd >= item.SrfBegin).ToList();
                ViewBag.ListLineManager = _userHelper.GetByRoleName("Line Manager").ToList();
                ViewBag.ListHeadOfSourcing = _userHelper.GetByRoleName("Head Of Sourcing").ToList();
                ViewBag.ListSrfStatus = new List<SrfApproveStatus>(new SrfApproveStatus[] { SrfApproveStatus.Waiting, SrfApproveStatus.Approved }).Select(x => new { Id = x, Name = x.ToString() });
                ViewBag.ListTypeSRF = new List<SrfType>(new SrfType[] { SrfType.New, SrfType.Extension }).Select(x => new { Id = x, Name = x.ToString() });
                ViewBag.LisStatusSrf = new List<SrfStatus>(new SrfStatus[] { SrfStatus.Done,SrfStatus.Terminate,SrfStatus.Blacklist }).Select(x => new { Id = x, Name = x.ToString() });

                var Model = Mapper.Map<SrfRequestModelForm>(item);
                Model.FormVacancy = Mapper.Map<VacancyListFormModel>(Vacancy);
                if(item.Status == SrfStatus.Waiting)
                {
                    Model.Status = SrfStatus.Done;
                }

                var CandidateInfo = _candidate.GetById(item.CandidateId);
                if(CandidateInfo!=null)
                {
                    Model.AgencyId = CandidateInfo.AgencyId.Value;
                }

                var Esc = _escalation.GetAll().Where(x => x.SrfId.Equals(id)).FirstOrDefault();
                if(Esc==null)
                {
                    ViewBag.IsEscalation = false;
                }
                else
                {
                    ViewBag.IsEscalation = true;
                }
                    
                return View(Model);


            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        public override IActionResult Edit(Guid id, SrfRequestModelForm model)
        {
            if (ModelState.IsValid)
            {

                var item = Service.GetById(id);

                if (item == null)
                {
                    return NotFound();
                }

                var before = item;

                UpdateData(item, model);

                var result = Service.Update(item);

                AfterUpdateData(before, item);

                return RedirectToAction("Index", "ContractorData", new { area = "Admin" });
            }

            return View(model);
        }


        protected override void UpdateData(SrfRequest item, SrfRequestModelForm model)
        {
            var Dept = _department.GetById(model.DepartmentId);
            var Network = _network.GetById(model.NetworkId);
            int total_day = Extension.MonthDistance(model.SrfBegin, model.SrfEnd);
            item.SrfBegin = model.SrfBegin;
            item.SrfEnd = model.SrfEnd;
            item.DepartmentId = model.DepartmentId;
            item.DepartmentSubId = model.DepartmentSubId;
            item.CostCenterId = model.CostCenterId;
            item.AccountId = model.AccountId;
            item.NetworkId = model.NetworkId;
            item.Description = model.Description;
            item.ApproveSixId = model.ApproveSixId;
            item.ServicePackId = model.FormVacancy.ServicePackId;
            item.ServiceLevel = model.FormVacancy.OtLevel;
            item.IsHrms = model.FormVacancy.isHrms;
            item.IsManager = model.FormVacancy.isManager;
            item.Number = model.Number;
            item.Type = model.Type;
            item.ActivityId = model.ActivityId;
            item.ApproveStatusOne = model.ApproveStatusOne;
            item.ApproveStatusTwo = model.ApproveStatusTwo;
            item.ApproveStatusThree = model.ApproveStatusThree;
            item.ApproveStatusFour = model.ApproveStatusFour;
            item.ApproveStatusFive = model.ApproveStatusFive;
            item.ApproveStatusSix = model.ApproveStatusSix;
            item.isWorkstation = model.FormVacancy.isLaptop == 1 ? true : false;
            item.isCommunication = model.FormVacancy.isUsim;
            item.ProjectManagerId = Network.ProjectManagerId;
            item.ApproveOneId = model.ApproveOneId;
            item.Status = model.Status;

            if(model.ApproveTwoId!=0)
            {
                item.ApproveTwoId = model.ApproveTwoId;
            }

            if (model.ApproveThreeId!=0)
            {
                item.ApproveThreeId = model.ApproveThreeId;
            }

            if (model.ApproveFourId !=0)
            {
                item.ApproveFourId = model.ApproveFourId;
            }

            if (model.ApproveFiveId !=0)
            {
                item.ApproveFiveId = model.ApproveFiveId;
            }

            if (model.ApproveSixId!=0)
            {
                item.ApproveSixId = model.ApproveSixId;
            }


            var Esc = _escalation.GetAll().Where(x => x.SrfId.Equals(x.Id)).FirstOrDefault();
            if(Esc!=null)
            {
                Esc.ApproveStatusOne = model.ApproveStatusTwo;
                if(Dept.OperateOrNon == 1)
                {
                    Esc.ApproveStatusTwo = model.ApproveStatusTwo;
                }
                else
                {
                    Esc.ApproveStatusTwo = model.ApproveStatusThree;
                }
                Esc.ApproveStatusThree = model.ApproveStatusFive;
                Esc.ApproveStatusFour = model.ApproveStatusSix;
                _escalation.Update(Esc);
             
            }

            if (model.ExtendFrom!=null)
            {
                item.ExtendFrom = model.ExtendFrom;
                item.IsExtended = true;
            }
            else
            {
                item.ExtendFrom = null;
                item.IsExtended = false;
            }

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
                item.ApproveFourId = model.ApproveFourId;
            }

           
                // Update Vacancy
            var Candidate = _candidate.GetById(item.CandidateId);
            if (Candidate != null)
            {
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                var UppdateVacancy = Mapper.Map(model.FormVacancy, Vacancy);
                UppdateVacancy.AccountNameId = item.AccountId.Value;
                UppdateVacancy.CostCodeId = item.CostCenterId;
                UppdateVacancy.DepartmentId = item.DepartmentId;
                UppdateVacancy.DepartmentSubId = item.DepartmentSubId;
                UppdateVacancy.NetworkId = item.NetworkId;
                UppdateVacancy.ApproverOneId = item.ApproveOneId;
                UppdateVacancy.ServicePackId = model.FormVacancy.ServicePackId;
                UppdateVacancy.isLaptop = model.FormVacancy.isLaptop == 1 ? true : false;
                UppdateVacancy.isUsim = model.FormVacancy.isUsim;
                UppdateVacancy.isHrms = model.FormVacancy.isHrms;
                UppdateVacancy.isManager = model.FormVacancy.isManager;
                UppdateVacancy.OtLevel = model.FormVacancy.OtLevel;
                _vacancy.Update(UppdateVacancy);

               
               

                // Set SRF Status

                var UserProfile = _profileService.GetById(Candidate.AccountId);
                if (item.Status == SrfStatus.Done)
                {
                    UserProfile.IsBlacklist = false;
                    UserProfile.IsTerminate = false;
                    UserProfile.IsActive = true;
                    _profileService.Update(UserProfile);
                }

                if (item.Status == SrfStatus.Terminate)
                {
                    UserProfile.IsBlacklist = false;
                    UserProfile.IsTerminate = true;
                    UserProfile.IsActive = false;
                    _profileService.Update(UserProfile);
                }

                if (item.Status == SrfStatus.Blacklist)
                {
                    UserProfile.IsBlacklist = true;
                    UserProfile.IsTerminate = false;
                    UserProfile.IsActive = false;
                    _profileService.Update(UserProfile);
                }

                Candidate.AgencyId = model.AgencyId;
                _candidate.Update(Candidate);


                TempData["Success"] = "OK";
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

        public IActionResult Terimanted(string id,string TerminatedDate)
        {
            if(!string.IsNullOrWhiteSpace(TerminatedDate))
            {
                var srf = Service.GetById(Guid.Parse(id));
                if(srf!=null)
                {
                    var DateTerminated = DateTime.Parse(TerminatedDate);
                    if(DateTime.Now == DateTerminated)
                    {
                        // Update SRF To Terminate
                        srf.TeriminateNote = "Terminate Resource";
                        srf.TerimnatedBy = _userHelper.GetLoginUser(User).Name;
                        srf.TerminatedDate = DateTerminated;
                        Service.Update(srf);

                        // Update User To Terminate
                        var Candidate = _candidate.GetById(srf.CandidateId);
                        if(Candidate!=null && Candidate.AccountId.HasValue)
                        {
                            var UserProfile = _profileService.GetById(Candidate.AccountId);
                            UserProfile.IsActive = false;
                            UserProfile.IsTerminate = true;
                            UserProfile.IsBlacklist = false;
                            _profileService.Update(UserProfile);
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
