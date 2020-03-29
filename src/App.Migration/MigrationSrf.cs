using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using App.Services.Core.Interfaces;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.Data.DAL;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using App.Services.Identity;
using System.IO;
using App.Domain.Models.Core;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Migration
{
    public class MigrationSrf
    {
        private IServiceProvider _service;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVacancyListService _vacancy;
        private readonly IAccountNameService _account;
        private readonly IUserProfileService _userProfile;
        private readonly ICostCenterService _costCenter;
        private readonly IDepartementService _department;
        private readonly IDepartementSubService _departmentSub;
        private readonly IJobStageService _jobstage;
        private readonly INetworkNumberService _networkNumber;
        private readonly IServicePackService _ssow;
        private readonly IServicePackCategoryService _ssow_category;
        private readonly ICityService _city;
        private readonly IPackageTypeService _packageType;
        private readonly ICandidateInfoService _candidate;
        private readonly IActivityCodeService _activity;
        private readonly ISrfRequestService _srf;
        private readonly ISrfEscalationRequestService _escalation;


        private int Count { get; set; }

        public MigrationSrf(IServiceProvider service, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _roleManager = roleManager;
            _userManager = userManager;
            _vacancy = _service.GetService<IVacancyListService>();
            _account = _service.GetService<IAccountNameService>();
            _userProfile = _service.GetService<IUserProfileService>();
            _costCenter = _service.GetService<ICostCenterService>();
            _department = _service.GetService<IDepartementService>();
            _departmentSub = _service.GetService<IDepartementSubService>();
            _jobstage = _service.GetService<IJobStageService>();
            _networkNumber = _service.GetService<INetworkNumberService>();
            _ssow = _service.GetService<IServicePackService>();
            _ssow_category = _service.GetService<IServicePackCategoryService>();
            _city = _service.GetService<ICityService>();
            _packageType = _service.GetService<IPackageTypeService>();
            _candidate = _service.GetService<ICandidateInfoService>();
            _activity = _service.GetService<IActivityCodeService>();
            _srf = _service.GetService<ISrfRequestService>();
            _escalation = _service.GetService<ISrfEscalationRequestService>();
        }

        public void Run()
        {
            // Create Candidate User
            int i = 0;
            List<string> Failed = new List<string>();
            List<string> UserFailed = new List<string>();
            List<string> IdExists = new List<string>();
            var CandidateList = _candidate.GetAll().Where(x=>x.Id == Guid.Parse("8934b42f-4818-472c-8e49-f808bf30be84")).OrderBy(x => x.Email).ToList();
            foreach(var row in CandidateList)
            {
                var CheckEmail = _userManager.FindByEmailAsync(row.Email).Result;
                if(CheckEmail==null)
                {
                    var UserProfile = new UserProfile();
                    UserProfile.Address = row.Address;
                    UserProfile.Birthdate = row.DateOfBirth;
                    UserProfile.Email = row.Email;
                    UserProfile.Gender = row.Gender;
                    UserProfile.HomePhoneNumber = row.HomePhoneNumber;

                    if (row.IdNumber.Length > 16)
                    {
                        UserProfile.IdNumber = row.IdNumber.Substring(0, 15);
                    }
                    else
                    {
                        UserProfile.IdNumber = row.IdNumber;
                    }

                    UserProfile.IsActive = true;
                    UserProfile.MobilePhoneNumber = row.MobilePhoneNumber;
                    UserProfile.Name = row.Name;
                    UserProfile.Roles = "Contractor";
                    UserProfile.UserName = "indira.dwicahyaningpuri";
                    UserProfile.IsBlacklist = false;
                    UserProfile.IsTerminate = false;

                    List<String> Role = new List<string>() { "Contractor" };
                    ApplicationUser NewUser = new ApplicationUser()
                    {
                        Email = row.Email,
                        UserName = "indira.dwicahyaningpuri",
                        UserProfile = UserProfile
                    };

                    var created = _userManager.CreateAsync(NewUser, "welcome1!").Result;
                    if (created.Succeeded)
                    {
                        var addRole = _userManager.AddToRolesAsync(NewUser, Role).Result;
                        if (addRole.Succeeded)
                        {
                            Console.WriteLine(row.Email + " has been inserted");
                            //Console.WriteLine("A");
                            i++;
                        }
                        else
                        {
                           UserFailed.Add("'" + row.Email + "',");
                           Console.WriteLine(created.Errors.ToString());
                           //Console.WriteLine("B");
                        }
                    }
                    else
                    {
                        Failed.Add("'"+ row.Id.ToString()+"',");
                        Console.WriteLine(created.Errors.ToString());
                        //Console.WriteLine("C");
                    }
                }
                else
                {
                    UserFailed.Add("'" + row.Email + "',");
                    IdExists.Add(CheckEmail.Id);
                    //Console.WriteLine("D");
                }
            }

            Console.WriteLine("Total Insert " + i);

            if (Failed!=null)
            {
                foreach(var row in Failed)
                {
                    Console.WriteLine(row);
                }
                Console.WriteLine("================================");
                foreach (var row in UserFailed)
                {
                    Console.WriteLine(row);
                }
                Console.WriteLine("================================");
                foreach (var row in IdExists)
                {
                    Console.WriteLine("'"+row+"',");
                }
            }

        }

        private void VacancyAndCandidate()
        {
            Data dt = new Data("vacany_candidate");
            foreach (var row in dt.GetData())
            {
                int index = 1;
                var Data = (List<String>)row;
                var Org = _department.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var SubOrg = _departmentSub.GetAll().Where(x => Truncate(x.SubName) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var CostCr = _costCenter.GetAll().Where(x => Truncate(x.Code) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var AnnName = _account.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var Net = _networkNumber.GetAll().Where(x => Truncate(x.Code) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                if(Net==null)
                {
                    Net = _networkNumber.GetAll().OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                }
                var JoinDate = DateTime.Parse(Data[index].ToString()); index += 1;
                var PackageType = _packageType.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var SsowCategory = _ssow_category.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var Ssow = _ssow.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var BasiServiceLevel = int.Parse(Data[index].ToString()); index += 1;
                var IsWorkstation = int.Parse(Data[index].ToString()); index += 1;
                var IsUsim = int.Parse(Data[index].ToString()); index += 1;
                var IsManager = int.Parse(Data[index].ToString()); index += 1;
                var NormalRate = int.Parse(Data[index].ToString()); index += 1;
                var IsSignum = int.Parse(Data[index].ToString()); index += 1;
                var Jobstage = _jobstage.GetAll().Where(x => Truncate(x.Stage) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var LineManager = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;
                var Sourcing = _userProfile.GetAll().Where(x => Truncate(x.Email) == Truncate(Data[index].ToString())).FirstOrDefault(); index += 1;

                var NewVacancy = new VacancyList();
                NewVacancy.AccountName = AnnName;
                NewVacancy.ApproverOne = LineManager;
                NewVacancy.ApproverTwo = Sourcing;
                NewVacancy.CostCenter = CostCr;
                NewVacancy.CreatedAt = JoinDate;
                NewVacancy.Departement = Org;
                NewVacancy.DepartementSub = SubOrg;
                NewVacancy.JobStage = Jobstage;
                NewVacancy.JoinDate = JoinDate;
                NewVacancy.Network = Net;
                NewVacancy.NoarmalRate = NormalRate;
                NewVacancy.OtLevel = BasiServiceLevel;
                NewVacancy.RequestBy = LineManager;
                NewVacancy.PackageType = PackageType;
                NewVacancy.ServicePackCategory = SsowCategory;
                NewVacancy.ServicePack = Ssow;
                NewVacancy.Status = VacanStatusFive.Done;
                NewVacancy.VacancyStatus = ApproverStatus.Completed;
                NewVacancy.isHrms = IsSignum == 1 ? true : false;
                NewVacancy.isLaptop = IsWorkstation == 1 ? true : false;
                NewVacancy.isManager = IsManager == 1 ? true : false;
                NewVacancy.isUsim = IsUsim == 1 ? true : false;
                NewVacancy.StatusOne = VacanStatusFirst.Selected;
                NewVacancy.StatusTwo = VacanStatusSecond.Approved;
                NewVacancy.StatusThree = VacanStatusThirth.Done;
                NewVacancy.StatusFourth = VacanStatusFourth.Approved;
               
                var AgencyType = int.Parse(Data[index].ToString()); index += 1;
                var CandidateName = Data[index].ToString(); index += 1;
                var NickName = Data[index].ToString(); index += 1;
                var Email = Data[index].ToString(); index += 1;
                var IdNumber = Data[index].ToString(); index += 1;
                if(IdNumber.Length>16)
                {
                    IdNumber = IdNumber.Substring(0, 16);
                }
                var Nationality = Data[index].ToString(); index += 1;
                var PlaceOfBirth = Data[index].ToString(); index += 1;
                var DateOfBirth = DateTime.Parse(Data[index].ToString()); index += 1;
                var Address = Data[index].ToString(); index += 1;
                var Gender = Data[index].ToString(); index += 1;
                var Martial = Data[index].ToString(); index += 1;
                var PhoneNMobile = Data[index].ToString(); index += 1;
                var PhoneHome = Data[index].ToString(); index += 1;
                var Description = Data[index].ToString(); index += 1;
                var HomeBase = Data[index].ToString(); index += 1;
                var AhId = Data[index].ToString(); index += 1;
                var FormEricsson = int.Parse(Data[index].ToString()); index += 1;
                var Agency = Data[index].ToString();

                var UserCurrent = _userManager.FindByEmailAsync(Email).Result;
                if(UserCurrent!=null)
                {
                    var delete = _userManager.DeleteAsync(UserCurrent).Result;
                }

                var UserAgency = _userProfile.GetAll().Where(x => Truncate(x.Email) == Truncate(Agency)).FirstOrDefault();
                var UserProfile = new UserProfile();
                UserProfile.Address = Address;
                UserProfile.Birthdate = DateOfBirth;
                UserProfile.Email = Email;
                UserProfile.Gender = Gender.Equals(Truncate("F")) ? Domain.Models.Enum.Gender.Female : Domain.Models.Enum.Gender.Male;
                UserProfile.HomePhoneNumber = PhoneHome;
                UserProfile.IdNumber = IdNumber;
                UserProfile.IsActive = true;
                UserProfile.MobilePhoneNumber = PhoneNMobile;
                UserProfile.Name = CandidateName;
                UserProfile.Roles = "Contractor";
                UserProfile.UserName = Email;
                UserProfile.IsBlacklist = false;
                UserProfile.IsTerminate = false;
                UserProfile.AhId = AhId;

                List<String> Role = new List<string>() { "Contractor" };
                ApplicationUser NewUser = new ApplicationUser()
                {
                    Email = Email,
                    UserName = Email,
                    UserProfile = UserProfile
                };
                var created = _userManager.CreateAsync(NewUser, "welcome1!").Result;
                if (created.Succeeded)
                {
                    var addRole = _userManager.AddToRolesAsync(NewUser, Role).Result;
                    if (addRole.Succeeded)
                    {
                        _vacancy.Add(NewVacancy);

                        var code = _userManager.GenerateEmailConfirmationTokenAsync(NewUser).Result;
                        var Succseed = _userManager.ConfirmEmailAsync(NewUser, code).Result;
                        var Candidate = new CandidateInfo();
                        Candidate.Account = NewUser.UserProfile;
                        Candidate.Address = Address;
                        Candidate.Agency = UserAgency;
                        Candidate.AgencyType = (AgencyType)AgencyType;
                        Candidate.ApproveOneDate = JoinDate;
                        Candidate.ApproveOneStatus = ApproverStatus.Selected;
                        Candidate.ApproveTwoeDate = JoinDate;
                        Candidate.CreatedAt = JoinDate;
                        Candidate.Email = Email;
                        Candidate.Gender = Gender.Equals(Truncate("F")) ? Domain.Models.Enum.Gender.Female : Domain.Models.Enum.Gender.Male;
                        Candidate.HomePhoneNumber = PhoneNMobile;
                        Candidate.IdNumber = IdNumber;
                        Candidate.IsCandidate = false;
                        Candidate.IsContractor = true;
                        Candidate.IsUser = true;
                        Candidate.Martial = (Martial)Enum.Parse(typeof(Martial), Martial, true);
                        Candidate.MobilePhoneNumber = PhoneNMobile;
                        Candidate.Name = CandidateName;
                        Candidate.Nationality = Nationality;
                        Candidate.NickName = NickName;
                        Candidate.PlaceOfBirth = PlaceOfBirth;
                        Candidate.DateOfBirth = DateOfBirth;
                        Candidate.RequestBy = UserAgency;
                        Candidate.ApproveTwoStatus = ApproverStatus.Completed;
                        Candidate.Vacancy = NewVacancy;
                        _candidate.Add(Candidate);

                        if (Candidate != null)
                        {
                            Console.WriteLine(Email + " has been added ");
                        }

                        Count++;
                    }
                }

            }
            Console.WriteLine(Count + " Candidate record has been saved ");
        }
       
        private void Srf()
        {
            Data dt = new Data("srf");
            foreach (var row in dt.GetData())
            {
                int index = 1;
                var Data = (List<String>)row;
                var Email = Data[index].ToString(); index += 1;
                var CreatedAt = Data[index].ToString(); index += 1;
                var SrfNumber = Data[index].ToString(); index += 1;
                var IsOps = Data[index].ToString(); index += 1;
                var SrfLineManager = Data[index].ToString(); index += 1;
                var SrfHeadOfSL = Data[index].ToString(); index += 1;
                var SrfHeadOfOps = Data[index].ToString(); index += 1;
                var SrfOrNon = Data[index].ToString(); index += 1;
                var SrfHsrc = Data[index].ToString(); index += 1;
                var SerCoord = Data[index].ToString(); index += 1;
                var DateApprov1 = Data[index].ToString(); index += 1;
                var DateApprov2 = Data[index].ToString(); index += 1;
                var DateApprov3 = Data[index].ToString(); index += 1;
                var DateApprov4 = Data[index].ToString(); index += 1;
                var DateApprov5 = Data[index].ToString(); index += 1;
                var DateApprov6 = Data[index].ToString(); index += 1;
                var SrfStatus = Data[index].ToString(); index += 1;
                var SrfBegin = Data[index].ToString(); index += 1;
                var SrfEnd = Data[index].ToString(); index += 1;
                var RateType = Data[index].ToString(); index += 1;
                var ActivityCod = Data[index].ToString(); index += 1;
                var SRF_LM = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SrfLineManager)).FirstOrDefault();
                var SRF_HSL = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SrfHeadOfSL)).FirstOrDefault();
                var SRF_OPS = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SrfHeadOfOps)).FirstOrDefault();
                var SRF_NONOPS = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SrfHeadOfOps)).FirstOrDefault();
                var SRF_HOSRC = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SrfHsrc)).FirstOrDefault();
                var SRF_SRCRD = _userProfile.GetAll().Where(x => Truncate(x.Name) == Truncate(SerCoord)).FirstOrDefault();
                bool WhatOps = !string.IsNullOrEmpty(SrfHeadOfOps) ? true : false;

                var Candidate = _candidate.GetAll().Where(x => Truncate(x.Email) == Truncate(Email)).FirstOrDefault();
                if(Candidate!=null)
                {
                    var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                    var Network = _networkNumber.GetById(Vacancy.NetworkId);
                    var Type = SrfType.New;
                    var AnnualLeave = MonthDistance(DateTime.Parse(SrfBegin), DateTime.Parse(SrfEnd));
                    var ParentId = (String)null;
                    var CheckCandidateSrf = _srf.GetAll().Where(x => x.Candidate == Candidate).OrderByDescending(x=>x.SrfEnd).FirstOrDefault();
                    if(CheckCandidateSrf!=null)
                    {
                        Type = SrfType.Extension;
                        ParentId = CheckCandidateSrf.Id.ToString();
                        AnnualLeave = AnnualLeave + CheckCandidateSrf.AnnualLeave;
                    }

                    //# Create NewSrf SRF
                    SrfRequest NewSrf = new SrfRequest();
                    NewSrf.CreatedAt = !string.IsNullOrEmpty(CreatedAt) ? DateTime.Parse(CreatedAt) : DateTime.MinValue;
                    NewSrf.Number = SrfNumber;
                    NewSrf.Type = Type;
                    NewSrf.ApproveStatusOne = SrfApproveStatus.Submitted;
                    if (Truncate(RateType) == Truncate("Normal"))
                    {
                        NewSrf.ApproveStatusTwo = SrfApproveStatus.Approved;
                        if (WhatOps == true)
                        {
                            NewSrf.ApproveStatusThree = SrfApproveStatus.Approved;
                            NewSrf.ApproveStatusFour = SrfApproveStatus.Waiting;
                        }
                        else
                        {
                            NewSrf.ApproveStatusThree = SrfApproveStatus.Waiting;
                            NewSrf.ApproveStatusFour = SrfApproveStatus.Approved;
                        }
                        NewSrf.ApproveStatusSix = SrfApproveStatus.Approved;
                    }
                    else
                    {
                        NewSrf.ApproveStatusFive = SrfApproveStatus.Approved;
                    }

                    NewSrf.RequestBy = SRF_LM.Name;
                    NewSrf.SrfBegin = !string.IsNullOrEmpty(SrfBegin) ? DateTime.Parse(SrfBegin) : DateTime.MinValue;
                    NewSrf.SrfEnd = !string.IsNullOrEmpty(SrfEnd) ? DateTime.Parse(SrfEnd) : DateTime.MinValue;
                    NewSrf.ServiceLevel = Vacancy.OtLevel;
                    NewSrf.isWorkstation = Vacancy.isLaptop;
                    NewSrf.isCommunication = Vacancy.isUsim;
                    NewSrf.IsHrms = Vacancy.isHrms;
                    NewSrf.IsOps = WhatOps;
                    NewSrf.IsManager = false;

                    if (Truncate(RateType) == Truncate("Normal"))
                    {
                        NewSrf.RateType = Domain.Models.Enum.RateType.Normal;
                    }
                    else
                    {
                        NewSrf.RateType = Domain.Models.Enum.RateType.SpecialRate;
                        NewSrf.SpectValue = Vacancy.NoarmalRate;
                    }

                    NewSrf.IsExtended = (Type == SrfType.New) ? false : true;
                    NewSrf.IsLocked = false;
                    NewSrf.Status = Domain.Models.Enum.SrfStatus.Done;
                    NewSrf.SpectValue = 0;
                    NewSrf.IsActive = false;
                    NewSrf.ServicePackId = Vacancy.ServicePackId;
                    NewSrf.NetworkId = Vacancy.NetworkId;
                    NewSrf.CostCenterId = Vacancy.CostCodeId;
                    NewSrf.LineManagerId = Vacancy.ApproverOneId.Value;
                    NewSrf.ActivityCode = _activity.GetAll().Where(x => Truncate(x.Code) == Truncate(ActivityCod)).FirstOrDefault();
                    NewSrf.DepartmentId = Vacancy.DepartmentId;
                    NewSrf.DepartmentSubId = Vacancy.DepartmentSubId;
                    NewSrf.ProjectManagerId = Network.ProjectManagerId;
                    NewSrf.ApproveOneBy = SRF_LM;
                    NewSrf.ApproveTwoBy = SRF_HSL;
                    if (WhatOps == true)
                    {
                        NewSrf.ApproveThreeBy = SRF_OPS;
                    }
                    else
                    {
                        NewSrf.ApproveFourBy = SRF_NONOPS;
                    }
                    NewSrf.ApproveSixBy = SRF_SRCRD;
                    NewSrf.Candidate = Candidate;
                    NewSrf.AccountId = Vacancy.AccountNameId;

                    if(!string.IsNullOrEmpty(ParentId))
                    {
                        NewSrf.ExtendFrom = Guid.Parse(ParentId);
                    }

                    NewSrf.AnnualLeave = AnnualLeave;
                    _srf.Add(NewSrf);

                    if (Truncate(RateType) != Truncate("Normal"))
                    {
                        // # Create Escalasi in NewSrf

                        var Escalation = new SrfEscalationRequest();
                        Escalation.OtLevel = NewSrf.ServiceLevel;
                        Escalation.IsWorkstation = NewSrf.isWorkstation;
                        Escalation.IsCommnunication = NewSrf.isCommunication;
                        Escalation.SparateValue = Vacancy.NoarmalRate;
                        Escalation.Status = StatusEscalation.Done;
                        Escalation.ApproveStatusOne = SrfApproveStatus.Approved;
                        Escalation.ApproveStatusTwo = SrfApproveStatus.Approved;
                        Escalation.ApproveStatusThree = SrfApproveStatus.Approved;
                        Escalation.ApproveStatusFour = SrfApproveStatus.Approved;
                        Escalation.Note = "";
                        Escalation.ServicePackId = NewSrf.ServicePackId;
                        Escalation.SrfRequest = NewSrf;
                        _escalation.Add(Escalation);
                    }

                    if (NewSrf != null)
                    {
                        Count++;
                        Console.WriteLine(Data[0] + " " + Email + " Srf  with " + SrfNumber + "  from " + SrfBegin + " to " + SrfEnd + "  has been saved ");
                    }

                }
                else
                {
                    Console.WriteLine(Data[0] + " " + Email);
                }


            }

            Console.WriteLine(Count + " Srf record has been saved ");
        }

        private void Actived()
        {
            var AllSrf = _srf.GetAll().OrderByDescending(x => x.SrfEnd).ToList();
            foreach(var row in AllSrf)
            {
                var CandidateSrf = _srf.GetAll().Where(x => x.CandidateId.Equals(row.CandidateId) && x.IsActive == true).FirstOrDefault();
                if(CandidateSrf==null)
                {
                    var Temp = _srf.GetAll().Where(x => x.CandidateId.Equals(row.CandidateId)).FirstOrDefault();
                    Temp.IsActive = true;
                    _srf.Update(Temp);
                    if(Temp.Id!=null)
                    {
                        Console.WriteLine(Temp.Number+" has been actived ");
                    }
                }
            }
        }

        private bool CheckEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        private string Truncate(string val)
        {
            return val.ToLower().Trim();
        }

        private int MonthDistance(DateTime first, DateTime last)
        {
            return Math.Abs((first.Month - last.Month) + 12 * (first.Year - last.Year));
        }
    }
}
