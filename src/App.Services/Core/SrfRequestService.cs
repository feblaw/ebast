using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using App.Domain.Models.Enum;

namespace App.Services.Core
{
    public class SrfRequestService : BaseService<SrfRequest, IRepository<SrfRequest>>, ISrfRequestService
    {
        private readonly ICandidateInfoService _contractor;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly IUserProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<CandidateInfo> _candidate;
        private readonly IRepository<AccountName> _account;
        private readonly IRepository<VacancyList> _vacancy;
        private readonly IRepository<UserProfile> _user;
        private readonly IRepository<Departement> _department;
        private readonly IRepository<DepartementSub> _departmentSub;

        public SrfRequestService(
            UserManager<ApplicationUser> userManager, 
            IUserProfileService profileService,
            IRepository<SrfRequest> repository,
            ICandidateInfoService contractor,
            IRepository<CandidateInfo> candidate,
            IRepository<AccountName> account,
            IRepository<VacancyList> vacancy,
            IRepository<UserProfile> user,
            IRepository<Departement> department,
            IRepository<DepartementSub> departmentSub,
            ISrfEscalationRequestService escalation) : 
            base(repository)
        {
            _contractor = contractor;
            _escalation = escalation;
            _profileService = profileService;
            _userManager = userManager;
            _candidate = candidate;
            _account = account;
            _vacancy = vacancy;
            _user = user;
            _department = department;
            _departmentSub = departmentSub;
        }

       
        public string GenerateNumnber()
        {
            var item = _repository.GetAll().Where(x=>!string.IsNullOrEmpty(x.Number) && x.CreatedAt.HasValue && x.CreatedAt.Value.Date <= DateTime.Now.Date.AddYears(1)).OrderByDescending(x => x.Number).FirstOrDefault();
            string result = "0001";
            int digit = 4;
            if (item==null)
            {
                return result;
            }
            else
            {
                if(!string.IsNullOrEmpty(item.Number))
                {
                    if(item.Number.Length>digit)
                    {
                        int Temp = int.Parse(item.Number)+1;
                        return Temp.ToString();
                    }
                    else
                    {
                        string Current = int.Parse(item.Number).ToString();
                        int index = int.Parse(Current);
                        int newIndex = index + 1;
                        int i_number = newIndex.ToString().Length;
                        string number = newIndex.ToString();
                        for (int i = digit; i > i_number; i--)
                        {
                            number = "0" + number;
                        }
                        return number;
                    }
                }
                else
                {
                    return result;
                }
            }
        }

        public void SetActive(Guid SrfId, Guid CandidateId,int UserProfileId)
        {
            var Candidate = _contractor.GetAll().Where(x => x.Id.Equals(CandidateId) && x.AccountId.Equals(UserProfileId)).FirstOrDefault();
            var CandidateSrf = _repository.GetAll().Where(x => x.CandidateId.Equals(Candidate.Id)).ToList();

            var ApproveGeneral = _repository.GetSingle(SrfId);
            var ApproveEscalasi = _escalation.GetAll().Where(x => x.SrfId.Equals(SrfId)).FirstOrDefault();

            if (CandidateSrf!=null)
            {
                foreach(var srf in CandidateSrf)
                {
                    var Temp = _repository.GetSingle(srf.Id);
                    Temp.IsActive = false;
                    Temp.IsLocked = true;
                    _repository.Update(Temp);
                }

                if(ApproveGeneral!=null || ApproveEscalasi!=null)
                {
                    var Srf = _repository.GetSingle(SrfId);
                    Srf.IsActive = true;
                    Srf.IsLocked = false;
                    _repository.Update(Srf);
                }
               
            }

            
        }

        public bool DeleteSrf(Guid Id)
        {
            var CurrentSrf = _repository.GetSingle(Id);

            if(CurrentSrf.Type == Domain.Models.Enum.SrfType.Extension)
            {
                // Locked on current
                CurrentSrf.IsActive = false;
                CurrentSrf.IsLocked = true;
                _repository.Update(CurrentSrf);

                // Actved on parent srf
                var RootForm = _repository.GetSingle(CurrentSrf.ExtendFrom);
                RootForm.IsActive = true;
                RootForm.IsLocked = false;
                _repository.Update(RootForm);
                return true;
            }
            return false;

        }

        public string GenerateNumnberByCustom(int index)
        {
            int i_number = index.ToString().Length;
            string number = index.ToString();
            for (int i = 4; i > i_number; i--)
            {
                number = "0" + number;
            }
            return number;
        }

        public bool DeleteContractor(Guid Id)
        {
            var Srf = _repository.GetSingle(Id);
            if (Srf != null)
            {
                var Contractor = _contractor.GetAll().Where(x => x.Id.Equals(Srf.CandidateId)).FirstOrDefault();
                if (Contractor != null)
                {
                    var UserProfile = _profileService.GetById(Contractor.AccountId);
                    if (UserProfile != null)
                    {
                        var AppUser = _userManager.FindByIdAsync(UserProfile.ApplicationUserId).Result;
                        if (AppUser != null)
                        {
                            // Delete SRF
                            var SrfContractor = _repository.GetAll().Where(x => x.CandidateId.Equals(Contractor.Id)).ToList();
                            if(SrfContractor!=null)
                            {
                                foreach(var row in SrfContractor)
                                {
                                    var Temp = _repository.GetSingle(row.Id);
                                    _repository.Delete(Temp);
                                }
                            }
                            _contractor.Delete(Contractor);
                            _profileService.Delete(UserProfile);
                            var Deleted = _userManager.DeleteAsync(AppUser).Result;
                            if(Deleted.Succeeded)
                            {
                                return true;
                            }

                        }
                    }
                }
            }
            return false;
        }

        public int ActualSrf(int Userid,DateTime DateSrf, Boolean IsActive, Guid? DepartmentId = default(Guid?), Guid? DepartementSubId = default(Guid?), ClaimsPrincipal User = null)
        {
           
            #region FilterUser

            if(User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation") || User.IsInRole("Line Manager"))
            {
                if(User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
                {
                    if(DepartmentId!=null && DepartementSubId==null)
                    {
                        if (IsActive == true)
                        {
                            //penambahan filter untuk tgl srf end yang lebih besar dr tgl hari ini
                            var data = from srf in _repository.Table.Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AccountId.HasValue && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false && x.DepartmentId == DepartmentId && x.SrfEnd > System.DateTime.Now)
                                       join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                       join user in _user.Table on candidate.AccountId equals user.Id
                                       join dept in _department.Table on srf.DepartmentId equals dept.Id
                                       select new { Id = srf.Id };

                            return data.Count();
                        }
                        else
                        {
                            var data = from srf in _repository.Table
                                  .Where(x => x.IsActive == false && x.IsLocked == false && x.DepartmentId == DepartmentId && x.SrfEnd > System.DateTime.Now)
                                  .Where(x => x.ApproveStatusSix == SrfApproveStatus.Waiting)
                                       join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                       join user in _user.Table on candidate.AccountId equals user.Id
                                       join dept in _department.Table on srf.DepartmentId equals dept.Id
                                       select new { Id = srf.Id };

                            return data.Count();
                        }
                    }

                    if(DepartmentId!=null && DepartementSubId!=null)
                    {
                        if (IsActive == true)
                        {
                            var data = from srf in _repository.Table.Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AccountId.HasValue && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false && x.DepartmentSubId == DepartementSubId && x.SrfEnd > System.DateTime.Now)
                                       join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                       join user in _user.Table on candidate.AccountId equals user.Id
                                       join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                       select new { Id = srf.Id };

                            return data.Count();
                        }
                        else
                        {
                            var data = from srf in _repository.Table
                                    .Where(x => x.IsActive == false && x.IsLocked == false && x.DepartmentSubId == DepartementSubId && x.SrfEnd > System.DateTime.Now)
                                    .Where(x => x.ApproveStatusSix == SrfApproveStatus.Waiting)
                                       join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                       join user in _user.Table on candidate.AccountId equals user.Id
                                       join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                       select new { Id = srf.Id };

                            return data.Count();
                        }
                    }

                }

                if (User.IsInRole("Line Manager"))
                {
                    if (IsActive == true)
                    {
                        var data = from srf in _repository.Table.Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AccountId.HasValue && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false && x.DepartmentSubId == DepartementSubId && x.SrfEnd > System.DateTime.Now)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }
                    else
                    {
                        var data = from srf in _repository.Table
                                    .Where(x => x.IsActive == false && x.IsLocked == false && x.DepartmentSubId == DepartementSubId && x.SrfEnd > System.DateTime.Now)
                                    .Where(x => x.ApproveStatusSix == SrfApproveStatus.Waiting)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }
                }

            }
            else
            {
                if (DepartmentId != null && DepartementSubId == null)
                {
                   
                    if(IsActive==true)
                    {
                        var data = from srf in _repository.Table.Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AccountId.HasValue && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false && x.DepartmentId == DepartmentId)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join dept in _department.Table on srf.DepartmentId equals dept.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }
                    else
                    {
                        var data = from srf in _repository.Table
                                   .Where(x => x.IsActive == false && x.IsLocked == false && x.DepartmentId == DepartmentId)
                                   .Where(x=> x.ApproveStatusSix == SrfApproveStatus.Waiting)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join dept in _department.Table on srf.DepartmentId equals dept.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }

                }

                if (DepartmentId != null && DepartementSubId != null)
                {
                    if(IsActive ==true)
                    {
                        var data = from srf in _repository.Table.Where(x => x.IsActive == true && x.IsLocked == false && x.Candidate.IsUser == true && x.Candidate.AccountId.HasValue && x.Candidate.Account.IsBlacklist == false && x.Candidate.Account.IsTerminate == false && x.DepartmentSubId == DepartementSubId)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }
                    else
                    {
                        var data = from srf in _repository.Table
                                   .Where(x => x.IsActive == false && x.IsLocked == false && x.DepartmentSubId == DepartementSubId)
                                   .Where(x => x.ApproveStatusSix == SrfApproveStatus.Waiting)
                                   join candidate in _candidate.Table on srf.CandidateId equals candidate.Id
                                   join user in _user.Table on candidate.AccountId equals user.Id
                                   join deptSub in _departmentSub.Table on srf.DepartmentSubId equals deptSub.Id
                                   select new { Id = srf.Id };

                        return data.Count();
                    }
                }

            }



            #endregion

            return 0;
           
        }
    }
}
