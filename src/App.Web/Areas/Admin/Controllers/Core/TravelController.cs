using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Enum;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "HR Agency,Contractor,Line Manager,Regional Project Manager,Administrator,Head Of Service Line")]
    public class TravelController : BaseController<Claim, IClaimService, TravelRequestViewModel, TravelRequestModelForm, Guid>
    {
        private readonly ICostCenterService _costCenter;
        private readonly INetworkNumberService _networkNumber;
        private readonly IActivityCodeService _activityCode;
        private readonly ICityService _city;
        private readonly NotifHelper _notif;
        private readonly IUserService _userService;
        private readonly IUserProfileService _profileUser;
        private readonly HostConfiguration _hostConfiguration;
        private readonly IUserHelper _userHelper;
        private readonly IVacancyListService _vacancy;
        private readonly ICandidateInfoService _candidate;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _profile;
        private readonly ITicketInfoService _ticket;


        public TravelController(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IMapper mapper,
            IClaimService service,
            ICostCenterService costCenter,
            INetworkNumberService networkNumber,
            IActivityCodeService activityCode,
            ICityService city,
            IVacancyListService vacancy,
            IUserProfileService profileUser,
            ICandidateInfoService candidate,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            NotifHelper notif,
            IUserProfileService profile,
            ITicketInfoService ticket,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _costCenter = costCenter;
            _networkNumber = networkNumber;
            _activityCode = activityCode;
            _city = city;
            _notif = notif;
            _userService = userService;
            _profileUser = profileUser;
            _hostConfiguration = hostConfiguration.Value;
            _userHelper = userHelper;
            _vacancy = vacancy;
            _candidate = candidate;
            _userManager = userManager;
            _profile = profile;
            _ticket = ticket;
        }

        public override IActionResult Index()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return base.Index();
        }

        [Authorize(Roles = "Contractor,Administrator,HR Agency")]
        public override IActionResult Create()
        {
            //var Srf = _userHelper.GetCurrentSrfByLogin(User);
            //var Candidate = _candidate.GetById(Srf.CandidateId);
            //var Vacancy = _vacancy.GetById(Candidate.VacancyId);

            //// Auto Fll Form SRF
            //ViewBag.CostCenterId = Vacancy.CostCodeId;
            //ViewBag.NetworkNumberId = Vacancy.NetworkId;
            var maxNumber = Service.GetAll().Select(x => x.TravelReqNo)
                .DefaultIfEmpty()
                .Max();

            if(maxNumber.Equals(null))
            {
                ViewBag.TravelReqNumber = 1;
            }
            else
            {
                ViewBag.TravelReqNumber = maxNumber+1;
            }

            
            //ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.Status == Status.Active).ToList();
            ViewBag.ContractorName = _vacancy.GetAll().Where(x => x.VendorId == _userHelper.GetUser(User).UserProfile.Id
                && x.EndDate >= DateTime.Now
                && x.StatusThree == SrfApproveStatus.Approved
                && x.TerminateBy == null).ToList();
            //ViewBag.NetworkNumber = _networkNumber.GetAll().Where(x=>x.IsClosed == false).ToList();
            //ViewBag.ActivityCode = _activityCode.GetAll().ToList();
            ViewBag.Schedule = from Schedule n in Enum.GetValues(typeof(Schedule)) select new { Id = (int)n, Name = Extension.GetEnumDescription(n) };
            ViewBag.Departure = _city.GetAll().ToList();
            ViewBag.Destination = _city.GetAll().ToList();
            ViewBag.ProjectManager = _userHelper.GetByRoleName("Regional Project Manager").ToList();
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            return base.Create();
        }

        [Authorize(Roles = "HR Agency,Contractor,Line Manager,Regional Project Manager,Administrator")]
        public override IActionResult Details(Guid id)
        {
            var item = Service.GetById(id);
            //ViewBag.CostCenter = _costCenter.GetById(item.CostCenterId).Code;
            //ViewBag.NetworkNumber = _networkNumber.GetById(item.NetworkNumberId).DisplayName;
            //ViewBag.ActivityCode = _activityCode.GetById(item.ActivityCodeId).DisplayName;
            ViewBag.ContractorName = _vacancy.GetById(item.VacancyId).Name;
            ViewBag.Departure = _city.GetById(item.DepartureId).Name;
            ViewBag.Destination = _city.GetById(item.DestinationId).Name;
            ViewBag.ProjectManager = _profileUser.GetById(item.ClaimApproverOneId).Name;
            ViewBag.LineManager = _profileUser.GetById(item.ClaimApproverTwoId).Name;
            return base.Details(id);
        }

        [HttpGet]
        [Authorize(Roles = "HR Agency,Contractor,Administrator")]
        public override IActionResult Edit(Guid id)
        {
            //ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.Status == Status.Active).ToList();
            //ViewBag.NetworkNumber = _networkNumber.GetAll().Where(x => x.IsClosed == false).ToList();
            //ViewBag.ActivityCode = _activityCode.GetAll().ToList();
            ViewBag.ContractorName = _vacancy.GetAll().Where(x => x.VendorId == _userHelper.GetUser(User).UserProfile.Id
                && x.EndDate >= DateTime.Now
                && x.StatusThree == SrfApproveStatus.Approved
                && x.TerminateBy == null).ToList();
            ViewBag.Schedule = from Schedule n in Enum.GetValues(typeof(Schedule)) select new { Id = (int)n, Name = Extension.GetEnumDescription(n) };
            ViewBag.Departure = _city.GetAll().ToList();
            ViewBag.Destination = _city.GetAll().ToList();
            ViewBag.ProjectManager = _userHelper.GetByRoleName("Regional Project Manager").ToList();
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            ViewBag.TravelReqNumber = Service.GetById(id).TravelReqNo;
            return base.Edit(id);
        }

        [HttpPost]
        [Authorize(Roles = "HR Agency,Contractor,Administrator")]
        public override IActionResult Edit(Guid id, TravelRequestModelForm model)
        {
            if(ModelState.IsValid)
            {
                var item = Service.GetById(id);
                if(item==null)
                {
                    return NotFound();
                }
                var before = item;
                Mapper.Map(model, item);
                item.LastEditedBy = _userHelper.GetUserId(User);
                item.LastUpdateTime = DateTime.Now;
                //item.TravelReqNo = model.TravelReqNo;
                Service.Update(item);
                AfterUpdateData(before, item);
                TempData["Success"] = "Travel request has been updated";
                return RedirectToAction("Details", new { id });
            }

            return View(model);
        }


        protected override void CreateData(Claim item)
        {

            var MyProfile = _userHelper.GetLoginUser(User);
            //var ContractorData = _candidate.GetAll().Where(x => x.AccountId.Equals(MyProfile.Id)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            item.CreatedAt = DateTime.Now;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.ClaimType = ClaimType.TravelClaim;
            item.Contractor = _userHelper.GetLoginUser(User);
            item.StatusOne = StatusOne.Waiting;
            item.StatusTwo = StatusTwo.Waiting;
            item.ClaimStatus = ActiveStatus.Active;
            item.AgencyId = _userHelper.GetUser(User).UserProfile.Id;
            var maxNumber = Service.GetAll().Select(x => x.TravelReqNo)
                .DefaultIfEmpty()
                .Max();
            item.TravelReqNo = maxNumber + 1;
            //if (ContractorData != null)
            //{
            //    item.AgencyId = ContractorData.AgencyId;
            //    item.ContractorProfileId = ContractorData.Id;
            //}
        }

        protected override void AfterCreateData(Claim item)
        {

            var callbackUrl = Url.Action("Index",
              "Travel",
               new { area = "Admin" },
              _hostConfiguration.Protocol,
              _hostConfiguration.Name);

            var AppProfile = _userHelper.GetUserProfile(item.ClaimApproverOneId.Value);
            var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
            _notif.Send(
                User, // User From
                "New Travel Request", // Subject
                AppProfile.Name, // User target name
                AppUser.Email, // User target email
                callbackUrl, // Link CallBack
                "New travel request ", // Email content or descriptions
                item.Description, // Description
                NotificationInboxStatus.Request, // Notif Status
                Activities.Travel // Activity Status
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
                   "Travel",
                    new { area = "Admin" },
                   _hostConfiguration.Protocol,
                   _hostConfiguration.Name);

                var AppProfile = _userHelper.GetUserProfile(after.ClaimApproverOneId.Value);
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);


                _notif.Send(
                  User, // User From
                  "Travel Request Correction", // Subject
                  AppProfile.Name, // User target name
                  AppUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Travel request has been correction please confirm.", // Email content or descriptions
                  after.Description, // Description
                  NotificationInboxStatus.Request, // Notif Status
                  Activities.Travel // Activity Status
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
                        DoApprove(Guid.Parse(id), ApprovalStatus);
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
        public IActionResult Approval(Guid TravelId, int ApprovalStatus, string ApprovalNotes)
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

        private void DoApprove(Guid TravelId, int ApprovalStatus,string ApprovalNotes = null)
        {
            var item = Service.GetById(TravelId);
            var UserLogin = _userHelper.GetUserId(User);
            var UserProfile = _profile.GetByUserId(UserLogin);

            var callbackUrl = Url.Action("Index",
                  "Travel",
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
                }

                if (item.ClaimApproverTwoId == ProfileId && (item.StatusTwo == StatusTwo.Waiting || item.StatusTwo == StatusTwo.Rejected))
                {
                    item.StatusTwo = StatusTwo.Approved;
                    item.ApprovedDateTwo = DateTime.Now;
                    item.ClaimStatus = ActiveStatus.Done;
                }

                Service.Update(item);

                var AppProfile = _userHelper.GetUserProfile(item.ContractorId.Value);
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
                _notif.Send(
                  User, // User From
                  "Travel request is approved by " + _userHelper.GetLoginUser(User).Name, // Subject
                  AppProfile.Name, // User target name
                  AppUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Travel request is approved by " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                  ApprovalNotes, // Description
                  NotificationInboxStatus.Approval, // Notif Status
                  Activities.Request // Activity Status
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
                  "Activity Request Rejected By " + _userHelper.GetLoginUser(User).Name, // Subject
                  Contractor.Name, // User target name
                  ContractorUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Activity request has been rejected please fill correction.", // Email content or descriptions
                  ApprovalNotes, // Description
                  NotificationInboxStatus.Request, // Notif Status
                  Activities.Travel // Activity Status
                );
            }

            if(item.StatusOne == StatusOne.Approved && item.StatusTwo == StatusTwo.Approved)
            {
                _ticket.Add(new TicketInfo() { Status = TicketInfoStatus.WaitingBooking , ClaimId = item.Id, Files = "", Description = "" });

                // Send To Agency

               callbackUrl = Url.Action("Booking",
                "Travel",
                 new { area = "Admin", id = item.Id  },
                _hostConfiguration.Protocol,
                _hostConfiguration.Name);

                var Agency = _userHelper.GetUserProfile(item.AgencyId.Value);
                var Contractor = _userHelper.GetUserProfile(item.ContractorId.Value);
                var UserAgency = _userHelper.GetUser(Agency.ApplicationUserId);
                var LineManager = _userHelper.GetUserProfile(item.ClaimApproverOneId.Value);

              
                var Data = new Dictionary<string, string>()
                {
                    { "CallbackUrl", callbackUrl},
                    { "AgencyName", Agency.Name},
                    { "ResourceName", Contractor.Name},
                    { "LineManagerName", LineManager.Name},
                    { "Departure", _city.GetById(item.DepartureId).Name},
                    { "Destination", _city.GetById(item.DestinationId).Name},
                    { "Description", item.Description},
                };

                _notif.Send(
                      User, // User From
                      "Book Ticket", // Subject
                      Agency.Name, // User target name
                      UserAgency.Email, // User target email
                      callbackUrl, // Link CallBack
                      null, // Email content or descriptions
                      null, // Description
                      NotificationInboxStatus.Request, // Notif Status
                      Activities.Travel, // Activity Status,
                      null,
                      "Emails/Booking",
                      Data
                  );

            }
        }



        [HttpGet]
        [Authorize(Roles = "HR Agency,Contractor")]
        public IActionResult Booking(Guid id)
        {
            var item = Service.GetById(id);
            var Ticket = _ticket.GetAll().Where(x => x.ClaimId.Equals(id)).FirstOrDefault();
            if (item != null)
            {
                ViewBag.TicketId = Ticket.Id;
                ViewBag.Travel = item;
                ViewBag.RequestBy = _userHelper.GetLoginUser(User).Name;
                ViewBag.Departure = _city.GetById(item.DepartureId).Name;
                ViewBag.Destination = _city.GetById(item.DestinationId).Name;
                ViewBag.ProjectManager = _profileUser.GetById(item.ClaimApproverOneId).Name;
                ViewBag.LineManager = _profileUser.GetById(item.ClaimApproverTwoId).Name;
                ViewBag.TicketInfo = Enum.GetValues(typeof(TicketInfoStatus)).Cast<TicketInfoStatus>().Select(v => new SelectListItem
                {
                    Text = Extension.GetEnumDescription(v),
                    Value = ((int)v).ToString()
                }).ToList();
                TicketInfoFormModel model = Mapper.Map<TicketInfoFormModel>(Ticket);
                if (!string.IsNullOrWhiteSpace(Ticket.Files))
                {
                    var attachments = JsonConvert.DeserializeObject<List<string>>(Ticket.Files);
                    model.Files = string.Join("|", attachments.ToArray()) + "|";
                    ViewBag.Files = attachments;
                }
                else
                {
                    ViewBag.Files = null;
                }
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "HR Agency")]
        public IActionResult Booking(Guid id,TicketInfoFormModel model)
        {
            if(ModelState.IsValid)
            {
                var Ticket = _ticket.GetAll().Where(x => x.ClaimId.Equals(id)).FirstOrDefault();
                Ticket.Note = model.Description;
                Ticket.Price = model.Price;
                Ticket.Description = model.Description;
                Ticket.Status = model.Status;
                _ticket.Update(Ticket);

                // Send Notif To Contractor if ticket exists
                if(!string.IsNullOrEmpty(Ticket.Files))
                {
                    var Travel = Service.GetById(Ticket.ClaimId);
                    var Contractor = _userHelper.GetUserProfile(Travel.ContractorId.Value);
                    var UserContractor = _userHelper.GetUser(Contractor.ApplicationUserId);
                    if(UserContractor!=null)
                    {
                        var callbackUrl = Url.Action("Index",
                        "Travel",
                         new { area = "Admin" },
                        _hostConfiguration.Protocol,
                        _hostConfiguration.Name);


                        _notif.Send(
                            User, // User From
                            "Booked Ticket", // Subject
                            Contractor.Name, // User target name
                            UserContractor.Email, // User target email
                            callbackUrl, // Link CallBack
                            " Your Ticket is issued as for below Thanks ", // Email content or descriptions
                            model.Description, // Description
                            NotificationInboxStatus.Request, // Notif Status
                            Activities.Travel, // Activity Status,
                            Ticket.Files
                        );
                    }
                }


                TempData["success"] = "OK";
                return RedirectToAction("Booking", new { id = id });
            }
            return NotFound();
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
                        this.DoApprove(Guid.Parse(id), 0, remarks);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Index");
        }


    }
        
      
}
