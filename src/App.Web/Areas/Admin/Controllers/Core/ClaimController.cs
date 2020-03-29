using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class ClaimController : BaseController<Claim, IClaimService, ClaimViewModel, ClaimModelForm, Guid>
    {
        private readonly ICostCenterService _costCenter;
        private readonly INetworkNumberService _networkNumber;
        private readonly IProjectsService _project;
        private readonly IDepartementSubService _departmentSub;
        private readonly IActivityCodeService _activity;
        private readonly IUserProfileService _user;
        private readonly IClaimCategoryService _claimCategory;
        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;
        private readonly NotifHelper _notif;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IAllowanceFormService _allowForm;
        private readonly FileHelper _file;
        private readonly HostConfiguration _hostConfiguration;
        private readonly ISrfRequestService _srf;

        public ClaimController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, IMapper mapper,
            IClaimService service,
            ICostCenterService costCenter,
            INetworkNumberService networkNumber,
            IProjectsService project,
            IDepartementSubService departmentSub,
            IActivityCodeService activity,
            IUserProfileService user,
            ICandidateInfoService candidate,
            IVacancyListService vacacncy,
            IAllowanceFormService allowForm,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            IClaimCategoryService claimCategory,
            ISrfRequestService srf,
            NotifHelper notif,
            FileHelper file,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _costCenter = costCenter;
            _networkNumber = networkNumber;
            _project = project;
            _departmentSub = departmentSub;
            _activity = activity;
            _user = user;
            _notif = notif;
            _claimCategory = claimCategory;
            _candidate = candidate;
            _vacancy = vacacncy;
            _userManager = userManager;
            _userHelper = userHelper;
            _allowForm = allowForm;
            _file = file;
            _hostConfiguration = hostConfiguration.Value;
            _srf = srf;
        }

        public override IActionResult Index()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return base.Index();
        }

        protected override void Upload(Claim item, ClaimModelForm model)
        {

            if (!string.IsNullOrWhiteSpace(model.Files))
            {
                string fileDestination = "uploads/claim";
                var AttachmentUpload = model.Files.Split('|');
                var listSaveAttachment = new List<string>();
                if (AttachmentUpload != null)
                {
                    foreach (var file in AttachmentUpload)
                    {
                        if (!string.IsNullOrWhiteSpace(file))
                        {
                            string FileName = Path.GetFileNameWithoutExtension(file).ToSlug();
                            string MovedFiles = _file.FileMove(file, fileDestination, FileName, true);
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

        }

        public override IActionResult Create()
         {
            //var Srf = _userHelper.GetCurrentSrfByLogin(User);
            //var Candidate = _candidate.GetById(Srf.CandidateId);
            //var travel = Service.GetAll().Where(x => x.ClaimType == ClaimType.TravelClaim && x.ContractorId.Equals(_userHelper.GetLoginUser(User)) 
            //    && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Approved).Take(1);
            //var Vacancy =_vacancy.GetById(Candidate.VacancyId);
            //var Vacancy = Service.GetAll().Where(x => )
            var CheckTravel = Service
                .GetAll()
                .Where(x => x.ClaimType == ClaimType.TravelClaim && x.ContractorId.Equals(_userHelper.GetUser(User).UserProfile.Id) 
                && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Approved && x.VacancyId.HasValue)
                .ToList()
                .Count();

            var TravelClaimed = Service
                .GetAll()
                .Where(x => x.ClaimType == ClaimType.GeneralClaim && x.ContractorId.Equals(_userHelper.GetUser(User).UserProfile.Id))
                .Select(x => x.TravelReqNo)
                .ToList();

            ViewBag.TravelNotClaim = Service.GetAll().Where(x => !TravelClaimed.Contains(x.TravelReqNo) && x.ClaimType==ClaimType.TravelClaim && 
                x.ContractorId.Equals(_userHelper.GetUser(User).UserProfile.Id) && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Approved);

            //ViewBag.Vacancy = _vacancy.GetAll().Where(x => )

            //ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.Status == Status.Active).ToList();
            //ViewBag.NetworkCode = _networkNumber.GetAll().Where(x => x.IsClosed == false).ToList();
            ViewBag.Project = _project.GetAll().ToList();
            //ViewBag.SubOrganizationUnit = _departmentSub.GetAll().ToList();
            //ViewBag.Activity = _activity.GetAll().ToList();

            // Auto Fll Form SRF
            //ViewBag.CostCenterId = Vacancy.CostCodeId;
            //ViewBag.NetworkNumberId = Vacancy.NetworkId;
            //ViewBag.SubOrganizatonId = Vacancy.DepartmentSubId;
            ViewBag.ClaimCategory = _claimCategory.GetAll().ToList();

            if (CheckTravel > 0)
            {
                ViewBag.ClaimCategory = _claimCategory.GetAll().ToList();
            }
            else
            {
                ViewBag.ClaimCategory = _claimCategory.GetAll().Where(x => !x.Name.Equals("Travel Allowance")).ToList();
            }

            ViewBag.ProjectManager = _userHelper.GetByRoleName("Regional Project Manager").ToList();
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            ViewBag.DayType = Enum.GetValues(typeof(DayType)).Cast<DayType>().Select(v => new SelectListItem
            {
                Text = Extension.GetEnumDescription(v),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.Option = Enum.GetValues(typeof(AllowanceOptions)).Cast<AllowanceOptions>().Select(v => new SelectListItem
            {
                Text = Extension.GetEnumDescription(v),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.TripType = Enum.GetValues(typeof(TripType)).Cast<TripType>().Select(v => new SelectListItem
            {
                Text = Extension.GetEnumDescription(v),
                Value = ((int)v).ToString()
            }).ToList();
            ViewBag.AllowForm = JsonConvert.SerializeObject(_allowForm.GetAll().OrderBy(x=>x.Name).Select(x=>x.Value).ToList());
            //if (Vacancy.isManager)
            //{
            //    ViewBag.IsManager = '1';
            //}
            //else
            //{
                ViewBag.IsManager = '0';
            //}
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                //var Srf = _userHelper.GetCurrentSrfByLogin(User);
                //var Candidate = _candidate.GetById(Srf.CandidateId);
                var Vacancy = _vacancy.GetById(item.VacancyId);

                var CheckTravel = Service
                  .GetAll()
                  .Where(x => x.ClaimType == ClaimType.TravelClaim && x.ContractorId.Equals(_userHelper.GetLoginUser(User)) && x.StatusOne == StatusOne.Approved && x.StatusTwo == StatusTwo.Approved)
                  .ToList()
                  .Count();

                //ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.Status == Status.Active).ToList();
                //ViewBag.NetworkCode = _networkNumber.GetAll().Where(x => x.IsClosed == false).ToList();
                ViewBag.Project = _project.GetAll().ToList();
                //ViewBag.SubOrganizationUnit = _departmentSub.GetAll().ToList();
                //ViewBag.Activity = _activity.GetAll().ToList();

                if (CheckTravel > 0)
                {
                    ViewBag.ClaimCategory = _claimCategory.GetAll().ToList();
                }
                else
                {
                    ViewBag.ClaimCategory = _claimCategory.GetAll().Where(x => !x.Name.Equals("Travel Allowance")).ToList();
                }

                ViewBag.ProjectManager = _userHelper.GetByRoleName("Regional Project Manager").ToList();
                ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
                ViewBag.DayType = Enum.GetValues(typeof(DayType)).Cast<DayType>().Select(v => new SelectListItem
                {
                    Text = Extension.GetEnumDescription(v),
                    Value = ((int)v).ToString()
                }).ToList();
                ViewBag.Option = Enum.GetValues(typeof(AllowanceOptions)).Cast<AllowanceOptions>().Select(v => new SelectListItem
                {
                    Text = Extension.GetEnumDescription(v),
                    Value = ((int)v).ToString()
                }).ToList();
                ViewBag.TripType = Enum.GetValues(typeof(TripType)).Cast<TripType>().Select(v => new SelectListItem
                {
                    Text = Extension.GetEnumDescription(v),
                    Value = ((int)v).ToString()
                }).ToList();
                ViewBag.AllowForm = JsonConvert.SerializeObject(_allowForm.GetAll().OrderBy(x => x.Name).Select(x => x.Value).ToList());
                if (Vacancy.isManager)
                {
                    ViewBag.IsManager = '1';
                }
                else
                {
                    ViewBag.IsManager = '0';
                }
                //ViewBag.DepartmentSubId = Srf.DepartmentSubId;
                ViewBag.Id = id;

                var model = Mapper.Map<ClaimModelForm>(item);
                if (!string.IsNullOrWhiteSpace(item.Files))
                {
                    var attachments = JsonConvert.DeserializeObject<List<string>>(item.Files);
                    model.Files = string.Join("|", attachments.ToArray()) + "|";
                }

                return View(model);
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }
        }

        public override IActionResult Details(Guid id)
        {
            var item = Service.GetById(id);
            //var Contractor = _candidate.GetById(item.ContractorProfileId);
            //var Vacancy = _vacancy.GetById(Contractor.VacancyId);
            //ViewBag.CostCenter = _costCenter.GetById(item.CostCenterId).DisplayName;
            //ViewBag.NetworkCode = _networkNumber.GetById(item.NetworkNumberId).DisplayName;
            ViewBag.Project = _project.GetById(item.ProjectId).Description;
            //ViewBag.SubOrganizationUnit = _departmentSub.GetById(Vacancy.DepartmentSubId).SubName;
            //ViewBag.Activity = _activity.GetById(item.ActivityCodeId).DisplayName;
            ViewBag.ClaimCategory = _claimCategory.GetById(item.ClaimCategoryId).Name;
            ViewBag.ProjectManager = _user.GetById(item.ClaimApproverOneId).Name;
            ViewBag.LineManager = _user.GetById(item.ClaimApproverTwoId).Name;
            if (!string.IsNullOrEmpty(item.Files))
            {
                ViewBag.Files = JsonConvert.DeserializeObject<List<string>>(item.Files);
            }
            else
            {
                ViewBag.Files = null;
            }
            return base.Details(id);
        }


        protected override void CreateData(Claim item)
        {

            var UserLogin = _userHelper.GetLoginUser(User);
            var ContractorData = _candidate.GetAll().Where(x => x.AccountId.Equals(UserLogin.Id)).FirstOrDefault();
            item.CreatedAt = DateTime.Now;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.ClaimType = ClaimType.GeneralClaim;
            item.Contractor = _userHelper.GetLoginUser(User);
            item.StatusOne = StatusOne.Waiting;
            item.StatusTwo = StatusTwo.Waiting;
            item.ClaimStatus = ActiveStatus.Active;
            item.ClaimForId = null;
            item.DepartureId = null;
            item.DestinationId = null;
            item.RedeemForId = null;
            item.AddDate = DateTime.Now;
            if (ContractorData != null)
            {
                item.AgencyId = ContractorData.AgencyId;
                item.ContractorProfileId = ContractorData.Id;
            }

            var Category = _claimCategory.GetById(item.ClaimCategoryId);

            if (Category.Name.Trim().ToLower()== "Other Travel Allowance".Trim().ToLower())
            {
                item.Domallo1 = 0;
                item.Domallo2 = 0;
                item.Domallo3 = 0;
                item.Domallo4 = 0;
                item.Domallo5 = 0;
                item.Domallo6 = 0;
                item.Intallo1 = 0;
                item.Intallo2 = 0;
                item.Intallo3 = 0;
                item.Intallo4 = 0;
                item.Intallo5 = 0;
                item.Intallo6 = 0;
                item.OnCallShift = 0;
            }
            else
            {
                if (
                    Category.Name.Trim().ToLower() == "On Call Allowance".Trim().ToLower() ||
                    Category.Name.Trim().ToLower() == "Shift Allowance".Trim().ToLower())
                {
                    item.Domallo1 = 0;
                    item.Domallo2 = 0;
                    item.Domallo3 = 0;
                    item.Domallo4 = 0;
                    item.Domallo5 = 0;
                    item.Domallo6 = 0;
                    item.Intallo1 = 0;
                    item.Intallo2 = 0;
                    item.Intallo3 = 0;
                    item.Intallo4 = 0;
                    item.Intallo5 = 0;
                    item.Intallo6 = 0;
                }
                else
                {
                    if(item.TripType == TripType.Domestic)
                    {
                        item.Intallo1 = 0;
                        item.Intallo2 = 0;
                        item.Intallo3 = 0;
                        item.Intallo4 = 0;
                        item.Intallo5 = 0;
                        item.Intallo6 = 0;
                    }
                    else
                    {
                        item.Domallo1 = 0;
                        item.Domallo2 = 0;
                        item.Domallo3 = 0;
                        item.Domallo4 = 0;
                        item.Domallo5 = 0;
                        item.Domallo6 = 0;
                    }
                    item.OnCallShift = 0;
                  
                }
            }
        }

        protected override void UpdateData(Claim item, ClaimModelForm model)
        {
        }

        protected override bool ProperUpdateImplemented() => true;

        protected override Claim UpdateDataProper(Claim item, ClaimModelForm model)
        {
            var updated = base.UpdateDataProper(item, model);
            if (!string.IsNullOrWhiteSpace(model.Files))
            {
                var split = model.Files.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                updated.Files = JsonConvert.SerializeObject(split);
            }

            updated.ClaimType = ClaimType.GeneralClaim;
            updated.LastEditedBy = _userHelper.GetUserId(User);
            updated.LastUpdateTime = DateTime.Now;
            updated.Value = model.Value;
            var Category = _claimCategory.GetById(updated.ClaimCategoryId);
            if (Category.Name.Trim().ToLower() == "Other Travel Allowance".Trim().ToLower())
            {
                updated.Domallo1 = 0;
                updated.Domallo2 = 0;
                updated.Domallo3 = 0;
                updated.Domallo4 = 0;
                updated.Domallo5 = 0;
                updated.Domallo6 = 0;
                updated.Intallo1 = 0;
                updated.Intallo2 = 0;
                updated.Intallo3 = 0;
                updated.Intallo4 = 0;
                updated.Intallo5 = 0;
                updated.Intallo6 = 0;
                updated.OnCallShift = 0;
            }
            else
            {
                if (
                    Category.Name.Trim().ToLower() == "On Call Allowance".Trim().ToLower() ||
                    Category.Name.Trim().ToLower() == "Shift Allowance".Trim().ToLower())
                {
                    updated.Domallo1 = 0;
                    updated.Domallo2 = 0;
                    updated.Domallo3 = 0;
                    updated.Domallo4 = 0;
                    updated.Domallo5 = 0;
                    updated.Domallo6 = 0;
                    updated.Intallo1 = 0;
                    updated.Intallo2 = 0;
                    updated.Intallo3 = 0;
                    updated.Intallo4 = 0;
                    updated.Intallo5 = 0;
                    updated.Intallo6 = 0;
                }
                else
                {
                    if (updated.TripType == TripType.Domestic)
                    {
                        updated.Intallo1 = 0;
                        updated.Intallo2 = 0;
                        updated.Intallo3 = 0;
                        updated.Intallo4 = 0;
                        updated.Intallo5 = 0;
                        updated.Intallo6 = 0;
                    }
                    else
                    {
                        updated.Domallo1 = 0;
                        updated.Domallo2 = 0;
                        updated.Domallo3 = 0;
                        updated.Domallo4 = 0;
                        updated.Domallo5 = 0;
                        updated.Domallo6 = 0;
                    }
                    updated.OnCallShift = 0;

                }
            }

            return updated;
        }

        protected override void AfterCreateData(Claim item)
        {
            var ClaimType = _claimCategory.GetById(item.ClaimCategoryId);
            var callbackUrl = Url.Action("Index",
                "Claim",
                 new { area = "Admin" },
                _hostConfiguration.Protocol,
                _hostConfiguration.Name);

            var AppProfile = _userHelper.GetUserProfile(item.ClaimApproverOneId.Value);
            var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
            _notif.Send(
                User, // User From
                "New Claim Request", // Subject
                AppProfile.Name, // User target name
                AppUser.Email, // User target email
                callbackUrl, // Link CallBack
                "New Claim Request " + ClaimType.Name, // Email content or descriptions
                item.Description, // Description
                NotificationInboxStatus.Request, // Notif Status
                Activities.Claim // Activity Status
            );
            base.AfterCreateData(item);

        }

        protected override void AfterUpdateData(Claim before, Claim after)
        {
            int ProfileId = 0;
            bool Send = false;
            if (after.StatusOne == StatusOne.Reject)
            {
                ProfileId = after.ClaimApproverOneId.Value;
                Send = true;
            }

            if (after.StatusTwo == StatusTwo.Rejected)
            {
                ProfileId = after.ClaimApproverTwoId.Value;
                Send = true;
            }

            if (Send == true)
            {

                var callbackUrl = Url.Action("Index",
                   "Claim",
                    new { area = "Admin" },
                   _hostConfiguration.Protocol,
                   _hostConfiguration.Name);

                var AppProfile = _userHelper.GetUserProfile(after.ClaimApproverOneId.Value);
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);

                _notif.Send(
                  User, // User From
                  "Claim Request Correction", // Subject
                  AppProfile.Name, // User target name
                  AppUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Claim request Has been correction please confirm.", // Email content or descriptions
                  after.Description, // Description
                  NotificationInboxStatus.Request, // Notif Status
                  Activities.Claim // Activity Status
                );

                // Update Status
                var item = Service.GetById(after.Id);
                if (after.StatusOne == StatusOne.Reject)
                {
                    item.StatusOne = StatusOne.Waiting;
                }

                if (after.StatusTwo == StatusTwo.Rejected)
                {
                    item.StatusTwo = StatusTwo.Waiting;
                }

                Service.Update(item);
            }


        }

        [HttpPost]
        public IActionResult ApproveMultiple(string data, int ApprovalStatus)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {
                        DoApprove(Guid.Parse(id), ApprovalStatus, null);
                    }
                    if(ApprovalStatus==2)
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

        [HttpPost]
        public IActionResult Approval(Guid TravelId, int ApprovalStatus,string ApprovalNotes)
        {
            DoApprove(TravelId, ApprovalStatus, ApprovalNotes);
            if(ApprovalStatus==2)
            {
                TempData["Approved"] = "OK";
            }
            else
            {
                TempData["Rejected"] = "OK";
            }
            return RedirectToAction("Index");
        }

        private void DoApprove(Guid TravelId, int ApprovalStatus,string ApprovalNotes)
        {
            var item = Service.GetById(TravelId);
            var UserLogin = _userHelper.GetUserId(User);
            var UserProfile = _user.GetByUserId(UserLogin);

            var callbackUrl = Url.Action("Index",
                  "Claim",
                   new { area = "Admin"},
                  _hostConfiguration.Protocol,
                  _hostConfiguration.Name);

            if(ApprovalStatus == 2)
            {
                int ProfileId = UserProfile.Id;

                if (item.ClaimApproverOneId == ProfileId && (item.StatusOne == StatusOne.Waiting || item.StatusOne == StatusOne.Reject))
                {
                    item.StatusOne = StatusOne.Approved;
                    item.ApprovedDateOne = DateTime.Now;
                    item.ApproverOneNotes = ApprovalNotes;
                }

                if (item.ClaimApproverTwoId == ProfileId && (item.StatusTwo == StatusTwo.Waiting || item.StatusTwo == StatusTwo.Rejected))
                {
                    item.StatusTwo = StatusTwo.Approved;
                    item.ApprovedDateTwo = DateTime.Now;
                    item.ClaimStatus = ActiveStatus.Done;
                    item.ApproverOneNotes = ApprovalNotes;
                }

                Service.Update(item);

                var AppProfile = _userHelper.GetUserProfile(item.ContractorId.Value);
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
                _notif.Send(
                      User, // User From
                      "Cliam Request is Approved by " + _userHelper.GetLoginUser(User).Name, // Subject
                      AppProfile.Name, // User target name
                      AppUser.Email, // User target email
                      callbackUrl, // Link CallBack
                      "Claim Request is Approved by " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                      ApprovalNotes, // Description
                      NotificationInboxStatus.Approval, // Notif Status
                      Activities.Claim // Activity Status
                );
            }
            else
            {
                var Contractor = _userHelper.GetUserProfile(item.ContractorId.Value);
                var ContractorUser = _userHelper.GetUser(Contractor.ApplicationUserId);

                item.StatusOne = StatusOne.Waiting;
                item.StatusTwo = StatusTwo.Waiting;
                item.ApprovedDateOne = DateTime.Now;
                item.ApprovedDateTwo = DateTime.Now;
                Service.Update(item);

                _notif.Send(
                  User, // User From
                  "Claim Request Rejected By " + _userHelper.GetLoginUser(User).Name, // Subject
                  Contractor.Name, // User target name
                  ContractorUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Claim request has been rejected please fill correction", // Email content or descriptions
                  ApprovalNotes, // Description
                  NotificationInboxStatus.Request, // Notif Status
                  Activities.Claim // Activity Status
                );
            }
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
                        DoApprove(Guid.Parse(id), 0, remarks);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Index");
        }


    }
}
