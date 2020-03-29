using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
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
using System.Linq.Expressions;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using App.Domain.Models.Enum;
using App.Web.Models.ViewModels.Core.Business;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using App.Domain.DTO.Identity;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{

    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/srf")]
    [Authorize]
    public class SrfController : BaseApiController<SrfRequest, ISrfRequestService, SrfRequestDto, Guid>
    {

        private readonly IUserHelper _userHelper;
        private readonly IDepartementSubService _departmentSub;
        private readonly ICostCenterService _costCenter;
        private readonly INetworkNumberService _networkNumber;
        private readonly IDepartementService _department;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly IHostingEnvironment _env;
        private readonly ICandidateInfoService _candidate;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfile;
     

        public SrfController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            ISrfRequestService service,
            IDepartementSubService department,
            ICostCenterService costCenter,
            INetworkNumberService networkNumber,
            IDepartementService departmentOp,
            ISrfEscalationRequestService escalation,
            ICandidateInfoService candidate,
            IHostingEnvironment env,
            IUserProfileService userProfile,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userHelper = userHelper;
            _departmentSub = department;
            _costCenter = costCenter;
            _networkNumber = networkNumber;
            _department = departmentOp;
            _escalation = escalation;
            _env = env;
            _candidate = candidate;
            _userManager = userManager;
            _userProfile = userProfile;
        }


        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;


            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            var response = Service.GetDataTablesResponse<SrfRequestDto>(request,
                 Mapper,
                 $"IsLocked.Equals(false) && (ApproveOneId.toString().Equals(\"{PreofileId}\") || ApproveTwoId.toString().Equals(\"{PreofileId}\") || ApproveThreeId.toString().Equals(\"{PreofileId}\") || ApproveFourId.toString().Equals(\"{PreofileId}\") || ApproveFiveId.toString().Equals(\"{PreofileId}\") || ApproveSixId.toString().Equals(\"{PreofileId}\"))", Includes);
            
            if (User.IsInRole("HR Agency"))
            {
                response = Service.GetDataTablesResponse<SrfRequestDto>(request,
                Mapper,
                $"IsLocked.Equals(false) && Candidate.AgencyId.toString().Equals(\"{PreofileId}\")", Includes);
            }

            if (User.IsInRole("Sourcing"))
            {
                response = Service.GetDataTablesResponse<SrfRequestDto>(request,
                Mapper,
                $"IsLocked.Equals(false) && Candidate.Vacancy.ApproverTwoId.toString().Equals(\"{PreofileId}\")", Includes);
            }

            if(User.IsInRole("Administrator"))
            {
                response = Service.GetDataTablesResponse<SrfRequestDto>(request,
                Mapper, $"IsLocked.Equals(false)", Includes);
            }

            //#endregion
            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("ContractorHistory/{id}")]
        public IActionResult ContractorHistory(string id, IDataTablesRequest request)
        {
            Includes = new Expression<Func<SrfRequest, object>>[10];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;

            var data = Service.GetAllQ(Includes).Where(x => x.CandidateId == Guid.Parse(id));
            var response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
            return new DataTablesJsonResult(response, true);
        }


        [Route("GetDuration")]
        [HttpGet]
        public IActionResult GetDuration(string id = null,string start = null, string end = null)
        {
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(start))
            {
                DateTime lValue = DateTime.Parse(start);
                DateTime rValue = DateTime.Parse(end);
                TimeSpan difference = rValue - lValue;
                int val;
                int.TryParse(difference.TotalDays.ToString(), out val);
                int month, day, monthday;
                monthday = DateTime.DaysInMonth(lValue.Year, lValue.Month);
                string duration;

                int Annual = Extension.MonthDistance(lValue, rValue);
                if(!string.IsNullOrEmpty(id))
                {
                    var _lastSrf = Service.GetById(Guid.Parse(id));
                    if(_lastSrf!=null)
                    {
                        Annual += _lastSrf.AnnualLeave;
                    }
                }

                if (val >= monthday)
                {
                    month = val / monthday;
                    day = val % monthday;
                    if (day == 0)
                    {
                        duration = month.ToString() + " Month ";
                    }
                    else
                    {
                        duration = month.ToString() + " Month " + day.ToString() + " Days";
                    }
                }
                else
                {
                    int temp = val <= 0 ? 0 : val;
                    duration = temp + " Days";
                }
                //var val = Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
                return Json(new { srfduration = duration , srfannual = Annual });
            }
            return Json(new { result = false });
        }

        [Route("GetReconcile")]
        [HttpPost]
        public IActionResult GetReconcile(IDataTablesRequest request, string month = null, string year = null)
        {
            Includes = new Expression<Func<SrfRequest, object>>[18];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.ApproveOneBy;
            Includes[2] = pack => pack.Candidate.Agency;
            Includes[3] = pack => pack.Departement;
            Includes[4] = pack => pack.DepartementSub;
            Includes[5] = pack => pack.Account;
            Includes[6] = pack => pack.NetworkNumber;
            Includes[7] = pack => pack.ProjectManager;
            Includes[8] = pack => pack.CostCenter;
            Includes[9] = pack => pack.Candidate.Vacancy;
            Includes[10] = pack => pack.Candidate.Vacancy.JobStage;
            Includes[11] = pack => pack.ServicePack;
            Includes[12] = pack => pack.ServicePack.ServicePackCategory;
            Includes[13] = pack => pack.NetworkNumber.Project;
            Includes[14] = pack => pack.Escalation;
            Includes[15] = pack => pack.Escalation.ServicePack;
            Includes[16] = pack => pack.Candidate.Vacancy.PackageType;
            Includes[17] = pack => pack.Candidate.Account;

            var profileId = _userHelper.GetUser(User).UserProfile.Id;
            var data = Service.GetAllQ(Includes).Where(x => x.ApproveStatusSix == SrfApproveStatus.Approved
                && x.Candidate.IsUser == true
                && x.Candidate.AccountId.HasValue
                && x.Candidate.Account.IsBlacklist == false
                && x.Candidate.Account.IsTerminate == false);

            int m, y;
            if (!string.IsNullOrWhiteSpace(month) && !string.IsNullOrWhiteSpace(year)
                && int.TryParse(month, out m) && int.TryParse(year, out y))
            {
                var lastDayMonth = DateTime.DaysInMonth(y, m);
                var firstDate = new DateTime(y, m, 1);
                var lastDate = new DateTime(y, m, lastDayMonth);
                data = data.Where(x => (firstDate >= x.SrfBegin && firstDate <= x.SrfEnd)
                    || (lastDate >= x.SrfBegin && lastDate <= x.SrfEnd));
            }

            #region FilterByApprover

            if (User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
                data = data.Where(x => x.Departement.HeadId == profileId);
            else if (User.IsInRole("Head Of Service Line"))
                data = data.Where(x => x.DepartementSub.LineManagerid == profileId);
            else if (User.IsInRole("Head Of Sourcing"))
                data = data.Where(x => x.ApproveFiveId == profileId);
            else if (User.IsInRole("Customer Operation Manager"))
                data = data.Where(x => x.Account.Com == profileId);
            else if (User.IsInRole("Line Manager"))
                data = data.Where(x => x.ApproveOneId == profileId);
            else if (User.IsInRole("Service Coordinator"))
                data = data.Where(x => x.ApproveSixId == profileId);
            else if (User.IsInRole("HR Agency"))
                data = data.Where(x => x.Candidate.AgencyId == profileId);
            else if (User.IsInRole("Sourcing"))
                data = data.Where(x => x.Candidate.Vacancy.ApproverTwoId == profileId);

            #endregion

            var response = Service.GetDataTablesResponseByQuery<ReconcileDto>(request, Mapper, data);
            return new DataTablesJsonResult(response, true);
        }

        [Route("GetContractorData")]
        [HttpPost]
        public IActionResult GetContractorData(IDataTablesRequest request)
        {
            Includes = new Expression<Func<SrfRequest, object>>[12];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.ServicePack;
            Includes[2] = pack => pack.ServicePack.ServicePackCategory;
            Includes[3] = pack => pack.Departement;
            Includes[4] = pack => pack.DepartementSub;
            Includes[5] = pack => pack.ApproveOneBy;
            Includes[6] = pack => pack.NetworkNumber;
            Includes[7] = pack => pack.NetworkNumber.ProjectManager;
            Includes[8] = pack => pack.Candidate.Agency;
            Includes[9] = pack => pack.Candidate.Vacancy;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;
            Includes[11] = pack => pack.Candidate.Account;
            //Includes[11] = pack => pack.Candidate.Account.AhId;



            var profileId = _userHelper.GetUser(User).UserProfile.Id;
            var data = Service.GetAllQ(Includes).Where(x => x.IsActive == true
                && x.IsLocked == false
                && x.Candidate.IsUser == true
                && x.Candidate.AccountId.HasValue
                && x.Candidate.Account.IsBlacklist == false
                && x.Candidate.Account.IsTerminate == false);

            #region FilterByApprover

            if (User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
                data = data.Where(x => x.Departement.HeadId == profileId);
            else if (User.IsInRole("Head Of Service Line"))
                data = data.Where(x => x.DepartementSub.LineManagerid == profileId);
            else if (User.IsInRole("Head Of Sourcing"))
                data = data.Where(x => x.ApproveFiveId == profileId);
            else if (User.IsInRole("Customer Operation Manager"))
                data = data.Where(x => x.Account.Com == profileId);
            else if (User.IsInRole("Line Manager"))
                data = data.Where(x => x.ApproveOneId == profileId);
            else if (User.IsInRole("Service Coordinator"))
                data = data.Where(x => x.ApproveSixId == profileId);
            else if (User.IsInRole("HR Agency"))
                data = data.Where(x => x.Candidate.AgencyId == profileId);
            else if (User.IsInRole("Sourcing"))
                data = data.Where(x => x.Candidate.Vacancy.ApproverTwoId == profileId);

            #endregion

            var response = Service.GetDataTablesResponseByQuery<ContractorDataDto>(request, Mapper, data);
            return new DataTablesJsonResult(response, true);
        }

        [Route("PostPending")]
        [HttpPost]
        public IActionResult PostPending(IDataTablesRequest request)
        {
            Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;

            var profileId = _userHelper.GetUser(User).UserProfile.Id;

            var data = Service.GetAllQ(Includes);
                //.Where(x => x.IsLocked == false && x.IsActive == false);
            DataTablesResponse response;

            if (User.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting && x.ApproveOneId == profileId 
                && x.SrfBegin != null && x.SrfEnd != null && x.DateApproveStatusOne == null);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Waiting && x.ApproveTwoId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusThree == SrfApproveStatus.Waiting && x.ApproveThreeId == profileId && x.Departement.OperateOrNon == 1);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Non Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    //&& x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusFour == SrfApproveStatus.Waiting && x.ApproveFourId == profileId && x.Departement.OperateOrNon == 0);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Sourcing"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && x.ApproveStatusFive == SrfApproveStatus.Waiting && x.ApproveFiveId == profileId && x.RateType == RateType.SpecialRate);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Service Coordinator"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && ((x.RateType == RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Approved)
                        || (x.RateType != RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Waiting))
                    && x.ApproveStatusSix == SrfApproveStatus.Waiting && x.ApproveSixId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            response = DataTablesResponse.Create(request, 0, 0, new SrfRequestDto());
            return new DataTablesJsonResult(response, true);
        }

        [Route("PostPendingEsc")]
        [HttpPost]
        public IActionResult PostPendingEsc(IDataTablesRequest request)
        {
            Includes = new Expression<Func<SrfRequest, object>>[11];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Vacancy;
            Includes[2] = pack => pack.Candidate.Account;
            Includes[3] = pack => pack.Candidate.Agency;
            Includes[4] = pack => pack.ApproveOneBy;
            Includes[5] = pack => pack.ApproveSixBy;
            Includes[6] = pack => pack.DepartementSub;
            Includes[7] = pack => pack.ServicePack;
            Includes[8] = pack => pack.Escalation;
            Includes[9] = pack => pack.Departement;
            Includes[10] = pack => pack.Candidate.Vacancy.PackageType;

            var profileId = _userHelper.GetUser(User).UserProfile.Id;

            var data = Service.GetAllQ(Includes).Where(x => x.IsLocked == false && x.IsActive == false);
            DataTablesResponse response;

            if (User.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting && 
                x.ApproveOneId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Waiting && x.ApproveTwoId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusThree == SrfApproveStatus.Waiting && x.ApproveThreeId == profileId && x.Departement.OperateOrNon == 1);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Non Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusThree == SrfApproveStatus.Waiting && x.ApproveThreeId == profileId && x.Departement.OperateOrNon == 0);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Sourcing"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && x.ApproveStatusFive == SrfApproveStatus.Waiting && x.ApproveFiveId == profileId && x.RateType == RateType.SpecialRate);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Service Coordinator"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && ((x.RateType == RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Approved)
                        || (x.RateType != RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Waiting))
                    && x.ApproveStatusSix == SrfApproveStatus.Waiting && x.ApproveSixId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<SrfRequestDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            response = DataTablesResponse.Create(request, 0, 0, new SrfRequestDto());
            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("Recovery")]
        public IActionResult Recovery(IDataTablesRequest request)
        {
            var candidateIds = Service.GetAllQ().Select(x => x.CandidateId).ToList();
            var candidate = _candidate.GetAllQ().Where(x => x.AccountId.HasValue && candidateIds.Contains(x.Id)).Select(x => x.AccountId).ToList();
            var userManager = _userManager.GetUsersInRoleAsync("Contractor").Result;
            var data = _userProfile.GetAll().Where(x => userManager.Contains(x.ApplicationUser) && !candidate.Contains(x.Id)).OrderBy(x => x.UserName).AsQueryable();
            var response = Service.GetDataTablesResponseByQuery<UserProfileDto>(request, Mapper, data);
            return new DataTablesJsonResult(response, true);
        }

        [HttpGet("{id}")]
        [Route("GetOptionFormSrf")]
        public IActionResult GetOptionFormSrf(string id)
        {
            var Dept = _department.GetById(Guid.Parse(id));
            Dictionary<string, object> Data = new Dictionary<string, object>();

            String DepartmentSub = "<option disabled selected>-- Select Sub Organizational Unit --</option>";
            var ListDepartmentSub = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(Guid.Parse(id))).ToList();
            foreach (var row in ListDepartmentSub)
            {
                if (ListDepartmentSub.Count == 1)
                {
                    DepartmentSub += "<option value='" + row.Id + "' selected>" + row.SubName + "</option>";
                }
                else
                {
                    DepartmentSub += "<option value='" + row.Id + "'>" + row.SubName + "</option>";
                }
            }
            DepartmentSub += "<option></option>";

            String CostCenter = "<option disabled selected>-- Select Cost Center --</option>";
            var ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(Guid.Parse(id))).ToList();
            foreach (var row in ListCostCenter)
            {
                if (ListCostCenter.Count == 1)
                {
                    CostCenter += "<option value='" + row.Id + "' selected>" + row.DisplayName + "</option>";
                }
                else
                {
                    CostCenter += "<option value='" + row.Id + "'>" + row.DisplayName + "</option>";
                }
            }
            CostCenter += "<option></option>";

            String NetworkNumber = "<option disabled selected>-- - Select Network Number -- --</option>";
            var ListNetworkNumber = _networkNumber.GetAll().Where(x => x.DepartmentId.Equals(Guid.Parse(id))).ToList();
            foreach (var row in ListNetworkNumber)
            {
                if (ListNetworkNumber.Count == 1)
                {
                    NetworkNumber += "<option value='" + row.Id + "' selected>" + row.DisplayName + "</option>";
                }
                else
                {
                    NetworkNumber += "<option value='" + row.Id + "'>" + row.DisplayName + "</option>";
                }
            }
            NetworkNumber += "<option></option>";

            Data.Add("DepartementSub", DepartmentSub);
            Data.Add("CostCenter", CostCenter);
            Data.Add("NetworkNumber", NetworkNumber);

            if (Dept.OperateOrNon == 1)
            {
                Data.Add("IsOperation", true);
            }
            else
            {
                Data.Add("IsOperation", false);

            }
            return Ok(Data);
        }

       

        public override IActionResult Delete(Guid id)
        {
            var CurrentSrf = Service.GetById(id);
            if (null == CurrentSrf)
            {
                return Json(BadRequest());
            }
            else
            {
                var result = Service.DeleteSrf(id);
                if(result == true)
                {
                    return Ok(CurrentSrf);
                }
                else
                {
                    return NotFound();
                }
            }

        }


        private String UserApprover(String PreofileId)
        {
            if (User.IsInRole("Line Manager"))
            {
                return "ApproveOneId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Head Of Service Line"))
            {
               return "ApproveTwoId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Head Of Operation"))
            {
                return "ApproveThreeId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Head Of Non Operation"))
            {
                return "ApproveFourId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Head Of Sourcing"))
            {
                return "ApproveFiveId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Service Coordinator"))
            {
               return "ApproveSixId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("HR Agency"))
            {
                return "Candidate.AgencyId.ToString().Equals(\"" + PreofileId + "\")";
            }
            else if (User.IsInRole("Sourcing"))
            {
                return  "Candidate.Vacancy.ApproverTwoId.Equals(\"" + PreofileId + "\")";
            }
            return "ApproveOneId.ToString().Equals(\"" + PreofileId + "\")";
        }

      
    }
}
