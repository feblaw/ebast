using App.Data.Repository;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using System.Linq.Expressions;
using MoreLinq;

namespace App.Services.Core
{
    public class DashboardService : BaseService<SrfRequest, IRepository<SrfRequest>>, IDashboardService
    {

        private readonly IClaimService _claim;
        private readonly IAttendaceExceptionListService _timesheet;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly IVacancyListService _vacancy;
        private readonly IAccountNameService _account;
        private readonly ICandidateInfoService _candidate;
        private readonly IDepartementService _department;
        private readonly IDepartementSubService _departementSub;
        private readonly IBastService _bast;
        private readonly Identity.IUserProfileService _user;


        public Expression<Func<SrfRequest, object>>[] Includes { get; private set; }

        public DashboardService(
            IRepository<SrfRequest> repository, 
            IClaimService claim,
            IAttendaceExceptionListService timesheet,
            ISrfEscalationRequestService escalation,
            IAccountNameService account,
            ICandidateInfoService candidate,
            IDepartementService department,
            IDepartementSubService departmentSub,
            IVacancyListService vacancy,
            IBastService bast,
            Identity.IUserProfileService user
            ) : base(repository)
        {
            _claim = claim;
            _timesheet = timesheet;
            _escalation = escalation;
            _vacancy = vacancy;
            _account = account;
            _candidate = candidate;
            _department = department;
            _departementSub = departmentSub;
            _user = user;
            _bast = bast;
        }

        public int CountTimeSheetByApprover(int Id)
        {
            return _timesheet.GetAllQ()
                .Where(x => (x.StatusOne == StatusOne.Waiting && x.ApproverOneId == Id) || (x.StatusTwo == StatusTwo.Waiting && x.ApproverTwoId == Id))
                .Count();
        }

        public int CountTravelByApprover(int Id)
        {
            return _claim.GetAllQ()
                .Where(x => x.ClaimType == ClaimType.TravelClaim && ((x.StatusOne == StatusOne.Waiting && x.ClaimApproverOneId == Id) || (x.StatusTwo == StatusTwo.Waiting && x.ClaimApproverTwoId == Id && x.StatusOne == StatusOne.Approved)))
                .Count();
        }

        public int CountBastByApprover(int Id)
        {
            return _bast.GetAllQ()
                .Where(x => x.BastReqNo != null && ((x.ApprovalOneStatus== BastApproveStatus.Waiting && x.ApprovalOneID == Id) || (x.ApprovalTwoStatus == BastApproveStatus.Waiting && x.ApprovalTwoID == Id && x.ApprovalOneStatus == BastApproveStatus.Approved) || (x.ApprovalThreeStatus == BastApproveStatus.Waiting && x.ApprovalThreeID == Id && x.ApprovalTwoStatus == BastApproveStatus.Approved) || (x.ApprovalFourStatus == BastApproveStatus.Waiting && x.ApprovalFourID == Id && x.ApprovalThreeStatus == BastApproveStatus.Approved)))
                .Count();
        }

        public int CountClaimByApprover(int Id)
        {
            return _claim.GetAllQ()
                .Where(x => x.ClaimType == ClaimType.GeneralClaim && ((x.StatusOne == StatusOne.Waiting && x.ClaimApproverOneId == Id) || (x.StatusTwo == StatusTwo.Waiting && x.ClaimApproverTwoId == Id && x.StatusOne == StatusOne.Approved)))
                .Count();
        }

        public int CountVacancyByApprover(int Id)
        {
            Expression<Func<CandidateInfo, object>>[] Includes = new Expression<Func<CandidateInfo, object>>[1];
            Includes[0] = pack => pack.Vacancy;

            return _candidate.GetAllQ(Includes)
                .Where(x => (x.ApproveOneStatus == ApproverStatus.Shortlist && x.Vacancy.ApproverOneId == Id) || (x.ApproveTwoStatus == ApproverStatus.Shortlist && x.Vacancy.ApproverTwoId == Id))
                .Count();
        }

        public int CountWPByApprover(int id, ClaimsPrincipal user)
        {
            var data = _vacancy.GetAllQ();
            //.Where(x => x.IsLocked == false && x.IsActive == false);

            if (user.IsInRole("Line Manager"))
            {
                //data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                //&& x.ApproveOneId == id);
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Waiting && x.StatusThree == SrfApproveStatus.Waiting && x.ApproverOneId == id );
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Waiting && x.StatusOne == SrfApproveStatus.Approved&& x.ApproverTwoId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusOne == SrfApproveStatus.Approved 
                && x.StatusThree == SrfApproveStatus.Waiting 
                && x.ApproverThreeId == id);
                if (data.Any()) return data.Count();
            }
            if (user.IsInRole("Administrator"))
            {
                //data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                //&& x.ApproveOneId == id);
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Waiting && x.StatusThree == SrfApproveStatus.Waiting);
                if (data.Any()) return data.Count();
            }


            return 0;
        }

        public int CountWPActive(int id, ClaimsPrincipal user)
        {
            var data = _vacancy.GetAllQ();

            if (user.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverOneId == id
                && x.EndDate > DateTime.Now);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverTwoId == id
                && x.EndDate > DateTime.Now);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusOne == SrfApproveStatus.Approved
                && x.StatusThree == SrfApproveStatus.Approved
                && x.ApproverThreeId == id && x.EndDate > DateTime.Now);
                if (data.Any()) return data.Count();
            }
            if (user.IsInRole("Administrator"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.EndDate > DateTime.Now);
                if (data.Any()) return data.Count();
            }


            return 0;
        }

        public int CountWPEndSoon(int id, ClaimsPrincipal user)
        {
            var data = _vacancy.GetAllQ();

            var dateReminder = DateTime.Today.AddMonths(1);

            if (user.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverOneId == id
                && x.EndDate > DateTime.Now && x.EndDate < dateReminder && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverTwoId == id
                && x.EndDate > DateTime.Now && x.EndDate < dateReminder && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusOne == SrfApproveStatus.Approved
                && x.StatusThree == SrfApproveStatus.Approved
                && x.ApproverThreeId == id && x.EndDate > DateTime.Now && x.EndDate < dateReminder && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }
            if (user.IsInRole("Administrator"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.EndDate > DateTime.Now 
                && x.EndDate < dateReminder && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }


            return 0;
        }

        public int CountWPExpired(int id, ClaimsPrincipal user)
        {
            var data = _vacancy.GetAllQ();
            //.Where(x => x.IsLocked == false && x.IsActive == false);

            if (user.IsInRole("Line Manager"))
            {
                //data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                //&& x.ApproveOneId == id);
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverOneId == id
                && x.EndDate < DateTime.Now && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.ApproverTwoId == id
                && x.EndDate < DateTime.Now && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusOne == SrfApproveStatus.Approved
                && x.StatusThree == SrfApproveStatus.Approved
                && x.ApproverThreeId == id && x.EndDate < DateTime.Now && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }
            if (user.IsInRole("Administrator"))
            {
                //data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                //&& x.ApproveOneId == id);
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved && x.EndDate < DateTime.Now && x.Status != SrfStatus.Terminate);
                if (data.Any()) return data.Count();
            }


            return 0;
        }

        public int CountSrfByApprover(int id, ClaimsPrincipal user)
        {
            var data = _repository.GetAll();
               //.Where(x => x.IsLocked == false && x.IsActive == false);

            if (user.IsInRole("Line Manager"))
            {
                //data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                //&& x.ApproveOneId == id);
                data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting && x.ApproveOneId == id
                && x.SrfBegin != null && x.SrfEnd != null && x.DateApproveStatusOne == null);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Waiting && x.ApproveTwoId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusThree == SrfApproveStatus.Waiting && x.ApproveThreeId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Non Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusFour == SrfApproveStatus.Waiting && x.ApproveFourId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Sourcing"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && x.ApproveStatusFive == SrfApproveStatus.Waiting && x.ApproveFiveId == id && x.RateType == RateType.SpecialRate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Service Coordinator"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && ((x.RateType == RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Approved)
                        || (x.RateType != RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Waiting))
                    && x.ApproveStatusSix == SrfApproveStatus.Waiting && x.ApproveSixId == id);
                if (data.Any()) return data.Count();
            }

            return 0;
        }

        public int CountSrfEscByApprover(int id, ClaimsPrincipal user)
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

            var data = _repository.GetAll(Includes).Where(x => x.IsActive == false && x.IsLocked == false);
            if (user.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.ApproveStatusOne == SrfApproveStatus.Waiting
                && x.ApproveOneId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Waiting && x.ApproveTwoId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusTwo == SrfApproveStatus.Approved
                    && x.ApproveStatusThree == SrfApproveStatus.Waiting && x.ApproveThreeId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Non Operation"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && x.ApproveStatusFour == SrfApproveStatus.Waiting && x.ApproveFourId == id);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Head Of Sourcing"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && x.ApproveStatusFive == SrfApproveStatus.Waiting && x.ApproveFiveId == id && x.RateType == RateType.SpecialRate);
                if (data.Any()) return data.Count();
            }

            if (user.IsInRole("Service Coordinator"))
            {
                data = data.Where(x => (x.ApproveStatusOne == SrfApproveStatus.Approved || x.ApproveStatusOne == SrfApproveStatus.Submitted)
                    && ((x.Departement.OperateOrNon == 1 && x.ApproveStatusTwo == SrfApproveStatus.Approved && x.ApproveStatusThree == SrfApproveStatus.Approved)
                        || (x.Departement.OperateOrNon != 1 && x.ApproveStatusFour == SrfApproveStatus.Approved))
                    && ((x.RateType == RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Approved)
                        || (x.RateType != RateType.SpecialRate && x.ApproveStatusFive == SrfApproveStatus.Waiting))
                    && x.ApproveStatusSix == SrfApproveStatus.Waiting && x.ApproveSixId == id);
                if (data.Any()) return data.Count();
            }

            return 0;
        }


        private int CountPieChartAppover(Guid AccountId,int Id,ClaimsPrincipal User)
        {

            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[6];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Agency;
            Includes[2] = pack => pack.Candidate.Vacancy;
            Includes[3] = pack => pack.Candidate.Vacancy.JobStage;
            Includes[4] = pack => pack.Candidate.Vacancy.PackageType;
            Includes[5] = pack => pack.Candidate.Account;

            int data = _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();

            #region Approver
            if (User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Departement.HeadId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Head Of Service Line"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.DepartementSub.LineManagerid == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Head Of Sourcing"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.ApproveFiveId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Customer Operation Manager"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Account.Com == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Line Manager"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.ApproveOneId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Service Coordinator"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.ApproveSixId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("HR Agency"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AgencyId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            if (User.IsInRole("Sourcing"))
            {
                return _repository.GetAll(Includes).Where(x => x.AccountId == AccountId && x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.Vacancy.ApproverTwoId == Id && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false).Count();
            }

            #endregion
            return data;
        }

        private int CountPieChartAppoverWP(int AccountId, int Id, ClaimsPrincipal User)
        {

            Expression<Func<VacancyList, object>>[] Includes = new Expression<Func<VacancyList, object>>[1];
            Includes[0] = pack => pack.Vendor;

            int data = _vacancy.GetAll(Includes).Where(x => x.VendorId == AccountId && x.Status != SrfStatus.Terminate && x.EndDate > DateTime.Now && x.StatusThree == SrfApproveStatus.Approved).Count();

            #region Approver

            if (User.IsInRole("Line Manager"))
            {
                return _vacancy.GetAll(Includes).Where(x => x.VendorId == AccountId && x.Status != SrfStatus.Terminate && x.EndDate > DateTime.Now && x.ApproverOneId == Id && x.StatusThree == SrfApproveStatus.Approved).Count();
            }

            if (User.IsInRole("Head Of Service Line"))
            {
                return _vacancy.GetAll(Includes).Where(x => x.VendorId == AccountId && x.Status != SrfStatus.Terminate && x.EndDate > DateTime.Now && x.ApproverTwoId == Id && x.StatusThree == SrfApproveStatus.Approved).Count();
            }


            #endregion
            return data;
        }

        public string PieChartAccountNameBySrf(int Id, ClaimsPrincipal User)
        {

            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[6];
            Includes[0] = pack => pack.Candidate;
            Includes[1] = pack => pack.Candidate.Agency;
            Includes[2] = pack => pack.Candidate.Vacancy;
            Includes[3] = pack => pack.Candidate.Vacancy.JobStage;
            Includes[4] = pack => pack.Candidate.Vacancy.PackageType;
            Includes[5] = pack => pack.Candidate.Account;

            var FirstDateYear = new DateTime(DateTime.Now.Year, 1, 1);

            var AccountId = _repository
               .GetAll(Includes)
               .Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.SrfEnd.HasValue && x.SrfEnd.Value <= DateTime.Now && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false)
               .Select(x => x.AccountId)
               .ToList();

            var Result = _account.GetAll().Where(x => AccountId.Contains(x.Id)).ToList();
            var random = new Random();

            object[] PieChart = new object[Result.Count()];
            if (Result != null)
            {
                int i = 0;
                foreach (var row in Result)
                {
                    var color = String.Format("#{0:X6}", random.Next(0x1000000));
                    var Account = _account.GetById(row.Id);
                    var CountAccount = CountPieChartAppover(row.Id,Id,User);
                    PieChart[i] = new Dictionary<string, object>
                    {
                          {"provider", Account.Name},
                          {"allocation",CountAccount},
                          {"color",  color.ToString()}
                    };
                    i++;
                }
            }
            else
            {
                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                PieChart[0] = new Dictionary<string, object>
                {
                    {"provider", "No Account Name"},
                    {"allocation",0},
                    {"color",  color.ToString()}
                };
            }
            return JsonConvert.SerializeObject(PieChart);
        }

        public string PieChartAccountNameByWP(int Id, ClaimsPrincipal User)
        {

            Expression<Func<VacancyList, object>>[] Includes = new Expression<Func<VacancyList, object>>[1];
            Includes[0] = pack => pack.Vendor;
            //Includes[2] = pack => pack.Departement.Name;
            //Includes[1] = pack => pack.Candidate.Agency;
            //Includes[2] = pack => pack.Candidate.Vacancy;
            //Includes[3] = pack => pack.Candidate.Vacancy.JobStage;
            //Includes[4] = pack => pack.Candidate.Vacancy.PackageType;
            //Includes[5] = pack => pack.Candidate.Account;

            var FirstDateYear = new DateTime(DateTime.Now.Year, 1, 1);

            var AccountId = _vacancy
               .GetAll(Includes)
               .Where(x => x.Status != SrfStatus.Terminate && x.EndDate > DateTime.Now && x.StatusOne == SrfApproveStatus.Approved && x.StatusTwo == SrfApproveStatus.Approved && x.StatusThree == SrfApproveStatus.Approved)
               .Select(x => x.VendorId)
               .ToList();

            var Result = _user.GetAll().Where(x => AccountId.Contains(x.Id)).ToList();
            var random = new Random();

            object[] PieChart = new object[Result.Count()];
            if (Result != null)
            {
                int i = 0;
                foreach (var row in Result)
                {
                    var color = String.Format("#{0:X6}", random.Next(0x1000000));
                    var Vendor = _user.GetById(row.Id);
                    var CountAccount = CountPieChartAppoverWP(row.Id, Id, User);
                    PieChart[i] = new Dictionary<string, object>
                    {
                          {"provider", Vendor.Name},
                          {"allocation",CountAccount},
                          {"color",  color.ToString()}
                    };
                    i++;
                }
            }
            else
            {
                var color = String.Format("#{0:X6}", random.Next(0x1000000));
                PieChart[0] = new Dictionary<string, object>
                {
                    {"provider", "No Account Name"},
                    {"allocation",0},
                    {"color",  color.ToString()}
                };
            }
            return JsonConvert.SerializeObject(PieChart);
        }


        private IQueryable<SrfRequest> getSrf(int id, ClaimsPrincipal user, DateTime firstDate, DateTime lastDate, Guid? departmentId = null, Guid? subDepartmentId = null)
        {
            #region GetSrf
            Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[18];
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

            var data = _repository.GetAll(Includes).Where(x => firstDate >= x.SrfBegin
                && x.SrfEnd >= lastDate
                && x.IsActive == true
                && x.IsLocked == false
                && x.Candidate.AccountId.HasValue
                && x.Candidate.IsUser == true
                && x.Candidate.Account.IsBlacklist == false
                && x.Candidate.Account.IsTerminate == false);


            if (user.IsInRole("Head Of Operation") || user.IsInRole("Head Of Non Operation"))
            {
                if (departmentId != null)
                    return data.Where(x => x.Departement.HeadId == id
                        && x.DepartmentId == departmentId);
                else if (subDepartmentId != null)
                    return data.Where(x => x.Departement.HeadId == id
                        && x.DepartmentSubId == subDepartmentId);
                else
                    return data.Where(x => x.Departement.HeadId == id);
            }

            if (user.IsInRole("Head Of Service Line"))
                return data .Where(x => x.DepartementSub.LineManagerid == id);

            if (user.IsInRole("Head Of Sourcing"))
                return data.Where(x => x.ApproveFiveId == id);

            if (user.IsInRole("Customer Operation Manager"))
                return data.Where(x => x.Account.Com == id);

            if (user.IsInRole("Line Manager"))
                return data.Where(x => x.ApproveOneId == id);

            if (user.IsInRole("Service Coordinator"))
                return data.Where(x => x.ApproveSixId == id);

            if (user.IsInRole("HR Agency"))
                return data.Where(x => x.Candidate.AgencyId == id);

            if (user.IsInRole("Sourcing"))
                return data.Where(x => x.Candidate.Vacancy.ApproverTwoId == id);

            return data;

            #endregion
        }

        public String AllSrfChart(int Id, ClaimsPrincipal User, Guid? DepartmentId = null, Guid? SubDepartmentId = null)
        {
            int NowYears = DateTime.Now.Year;
            int NowMonth = DateTime.Now.Month;
            int TotalDaysInMonths = DateTime.DaysInMonth(NowYears, NowMonth);

            DateTime start = new DateTime(NowYears - 1, 12, 1);
            if (DateTime.Now.Month != 1)
            {
                start = new DateTime(NowYears, 1, 1);
            }
            DateTime end = new DateTime(NowYears, NowMonth, TotalDaysInMonths);
            int TotalMonth = (end.Month + end.Year * 12) - (start.Month + start.Year * 12);

            object[] BarChart = new object[TotalMonth+1];
            int i = 0;
            while (start < end)
            {
                int TotalDays = DateTime.DaysInMonth(start.Year, start.Month);
                DateTime FirstDate = new DateTime(start.Year, start.Month, 1);
                DateTime LastDate = new DateTime(start.Year, start.Month, TotalDays);

                var DataSrf = getSrf(Id, User, FirstDate,LastDate, DepartmentId, SubDepartmentId);
                string fullMonthName = new DateTime(DateTime.Now.Year, start.Month, 1).ToString("MMM", CultureInfo.InvariantCulture);

                decimal rateSignum = 0;
                decimal rateNonSignum = 0;
                int Signum = 0;
                int NonSignum = 0;
                var Srf = DataSrf.ToList(); 

                foreach (var row in Srf)
                {
                    if (row.Escalation == null)
                    {
                        decimal otLevel = row.ServiceLevel;
                        decimal hourly = row.ServicePack.Hourly;
                        decimal otLumpsum = otLevel * hourly;
                        decimal serviceRate = row.ServicePack.Rate;
                        decimal specialRate = row.SpectValue;
                        decimal laptop = row.isWorkstation == false ? 0 : row.ServicePack.Laptop;
                        decimal usin = row.Candidate.Vacancy.isUsim == false ? 0 : row.ServicePack.Usin;
                        decimal pricePerMonth = serviceRate + otLumpsum + laptop + usin + specialRate;

                        if (row.IsHrms == true)
                        {
                            rateSignum += pricePerMonth;
                            Signum++;
                        }
                        else
                        {
                            rateNonSignum += pricePerMonth;
                            NonSignum++;
                        }

                    }
                    else
                    {

                        decimal otLevel = row.Escalation.OtLevel;
                        decimal hourly = row.Escalation.ServicePack.Hourly;
                        decimal otLumpsum = otLevel * hourly;
                        decimal serviceRate = row.Escalation.ServicePack.Rate;
                        decimal specialRate = row.SpectValue;
                        decimal laptop = row.Escalation.IsWorkstation == false ? 0 : row.Escalation.ServicePack.Laptop;
                        decimal usin = row.Candidate.Vacancy.isUsim == false ? 0 : row.Escalation.ServicePack.Usin;
                        decimal pricePerMonth = serviceRate + otLumpsum + laptop + usin + specialRate;

                        if (row.IsHrms == true)
                        {
                            rateSignum += pricePerMonth;
                            Signum++;
                        }
                        else
                        {
                            rateNonSignum += pricePerMonth;
                            NonSignum++;
                        }

                    }
                }

                BarChart[i] = new Dictionary<string, object>
                {
                    {"category", fullMonthName},
                    {"rateSignum",decimal.ToInt64(rateSignum)},
                    {"rateNonSignum",decimal.ToInt64(rateNonSignum)},
                    {"srfSignum",Signum},
                    {"srfNonSignum",NonSignum },
                };

                start = start.AddMonths(1);
                i++;
            }

            return JsonConvert.SerializeObject(BarChart);

        }

        public string ChartByHeadDepartment(int Id, ClaimsPrincipal User)
        {
            Expression<Func<Departement, object>>[] Includes = new Expression<Func<Departement, object>>[1];
            Includes[0] = pack => pack.Head;

            var Department = _department.GetAll(Includes).Where(x => x.Head.Id == Id).OrderBy(x=>x.Name).Select(x => new { x.Id, x.Name }).ToList();
            object[] Data = new object[Department.Count()];
            int i = 0;
            if (Department.Any())
            {
                foreach (var row in Department)
                {

                    int j = 0;
                    var SubDept = _departementSub.GetAll().Where(x => x.DepartmentId == row.Id).OrderBy(x => x.SubName).Select(x => new { x.Id, x.SubName }).ToList();
                    object[] SubData = new object[SubDept.Count()];
                    if (SubDept.Any())
                    {
                        foreach (var s in SubDept)
                        {
                            SubData[j] = new Dictionary<string, object>
                            {
                                {"Id" , s.Id },
                                {"Name", s.SubName.ToUpper()},
                                {"Data",AllSrfChart(Id,User,null,s.Id)}
                            };
                            j++;
                        }
                    }

                    Data[i] = new Dictionary<string, object>
                    {
                        {"Id" , row.Id },
                        {"Name", row.Name.ToUpper()},
                        {"SubData",SubData},
                        {"Data",AllSrfChart(Id,User,row.Id,null)}
                    };
                    i++;
                }
            }

            return JsonConvert.SerializeObject(Data);
        }

        public string ChartByLineManager(int Id, ClaimsPrincipal User)
        {
            Expression<Func<DepartementSub, object>>[] Includes = new Expression<Func<DepartementSub, object>>[1];
            Includes[0] = pack => pack.LineManager;

            var DepartmentSub = _departementSub.GetAll(Includes).Where(x => x.LineManager.Id == Id).OrderBy(x => x.SubName).Select(x => new { x.Id, x.SubName }).ToList();
            object[] Data = new object[DepartmentSub.Count()];
            int i = 0;
            if (DepartmentSub.Any())
            {
                foreach (var row in DepartmentSub)
                {
                   
                    Data[i] = new Dictionary<string, object>
                    {
                        {"Id" , row.Id },
                        {"Name", row.SubName.ToUpper()},
                        {"Data",AllSrfChart(Id,User,null,row.Id)}
                    };
                    i++;
                }
            }

            return JsonConvert.SerializeObject(Data);
        }
    }
    
}
