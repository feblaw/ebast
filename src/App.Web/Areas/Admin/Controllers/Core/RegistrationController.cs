using App.Domain.Models.Core;
using App.Domain.Models.Identity;
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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using App.Domain.Models.Enum;
using Newtonsoft.Json.Linq;
using System.IO;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Globalization;
using OfficeOpenXml.Style;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Authorize]
    [Area("Admin")]
    public class RegistrationController : BaseController<AttendaceExceptionList, IAttendaceExceptionListService, AttendaceExceptionListViewModel, AttendaceExceptionListModelForm, Guid>
    {

        private readonly IDepartementService _department;
        private readonly ICostCenterService _cost;
        private readonly IAccountNameService _account;
        private readonly INetworkNumberService _network;
        private readonly IProjectsService _project;
        private readonly IActivityCodeService _code;
        private readonly ISubOpsService _subOp;
        private readonly ICityService _city;
        private readonly IUserProfileService _profile;
        private readonly IUserHelper _userHelper;
        private readonly IActivityCodeService _actiivity;
        private readonly ITimeSheetTypeService _timesheet;
        private readonly ICandidateInfoService _contractor;
        private readonly IDepartementSubService _departmentSub;
        private readonly IAttendanceRecordService _attendanceRecord;
        private readonly NotifHelper _notif;
        private readonly HostConfiguration _hostConfiguration;
        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;
        private readonly ISrfRequestService _srf;

        public RegistrationController(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IMapper mapper,
            IAttendaceExceptionListService service,
            IDepartementService department,
            ICostCenterService cost,
            IAccountNameService account,
            INetworkNumberService network,
            IProjectsService project,
            IActivityCodeService code,
            ISubOpsService subOp,
            ICityService city,
            IUserProfileService profile,
            IActivityCodeService activity,
            ITimeSheetTypeService timesheet,
            IUserHelper userHelper,
            ISrfRequestService srf,
            ICandidateInfoService contractor,
            IDepartementSubService departmentSub,
            IUserProfileService profileUser,
            IAttendanceRecordService attendanceRecord,
            ICandidateInfoService candidate,
            IVacancyListService vacancy,
            IOptions<HostConfiguration> hostConfiguration,
            NotifHelper notif) :
        base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _department = department;
            _cost = cost;
            _account = account;
            _network = network;
            _project = project;
            _code = code;
            _subOp = subOp;
            _city = city;
            _userHelper = userHelper;
            _profile = profile;
            _actiivity = activity;
            _timesheet = timesheet;
            _contractor = contractor;
            _department = department;
            _departmentSub = departmentSub;
            _notif = notif;
            _hostConfiguration = hostConfiguration.Value;
            _attendanceRecord = attendanceRecord;
            _candidate = candidate;
            _vacancy = vacancy;
            _srf = srf;
        }

        public override IActionResult Index()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            //var TimeSheet = Service.GetAll().Where(x => x.ApproverOneId.Equals(PreofileId) || x.ApproverTwoId.Equals(PreofileId) || x.ContractorId.Equals(PreofileId)).Select(x => x.Id).ToArray();
            //if (User.IsInRole("Administrator") || User.IsInRole("HR Agency"))
            //{
            //    TimeSheet = Service.GetAll().Select(x => x.Id).ToArray();
            //}
            //ViewBag.SumHours = _attendanceRecord.GetAll().Where(x => TimeSheet.Contains(x.AttendaceExceptionListId)).Select(x => x.Hours).Sum();
            ViewBag.PreofileId = PreofileId;
            return base.Index();
        }


        public override IActionResult Create()
        {
            ViewBag.ContractorName = _vacancy.GetAll().Where(x => x.VendorId == _userHelper.GetUser(User).UserProfile.Id
                && x.EndDate.AddMonths(1) >= DateTime.Now
                && x.StatusThree == SrfApproveStatus.Approved
                && x.TerminateBy == null).ToList();

            //var Srf = _userHelper.GetCurrentSrfByLogin(User);
            //var Candidate = _candidate.GetById(Srf.CandidateId);
            //var Vacancy = _vacancy.GetById(Candidate.VacancyId);

            //ViewBag.OrganizationUnit = _department.GetAll().ToList();
            //ViewBag.CostCenter = _cost.GetAll().Where(x => x.Status == Status.Active).
            //    ToList();
            //ViewBag.AccountName = _account.GetAll().ToList();
            //ViewBag.NetworkNumber = _network.GetAll().Where(x=>x.IsClosed == false).ToList();
            //ViewBag.Project = _project.GetAll().ToList();
            //ViewBag.Code = _code.GetAll().ToList();
            //ViewBag.SubOperasional = _subOp.GetAll().ToList();
            //ViewBag.City = _city.GetAll().ToList();
            //ViewBag.Annual = Srf.AnnualLeave;

            // Auto Fll Form SRF
            //ViewBag.OrganizationUnitId = Vacancy.DepartmentId;
            //ViewBag.SubOrganizatonId = Vacancy.DepartmentSubId;
            //ViewBag.CostCenterId = Vacancy.CostCodeId;
            //ViewBag.AccountNameId = Vacancy.AccountNameId;
            //ViewBag.NetworkNumberId = Vacancy.NetworkId;


            // Start - Menampilkan Time Sheet
            //var jan1 = new DateTime(DateTime.Today.Year, 1, 1);

            //if(DateTime.Now.Month == 1)
            //{
            //    jan1 = new DateTime(DateTime.Today.Year - 1, 12, 1);
            //}

            //var firstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            //var weeks = Enumerable.Range(0, 54).Select(i => new
            //{ weekStart = firstWeek.AddDays(i * 7) })
            //    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
            //    .Select(x => new { x.weekStart, weekFinish = x.weekStart.AddDays(6) })
            //    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
            //    .Select((x, i) => new
            //    { x.weekStart, x.weekFinish, weekNum = i + 1 });

            //if(DateTime.Now.Month == 1)
            //{
            //    weeks = Enumerable.Range(0, 54).Select(i => new
            //    { weekStart = firstWeek.AddDays(i * 7) })
            //    .TakeWhile(x => x.weekStart.Year <= DateTime.Now.Year)
            //    .Select(x => new { x.weekStart, weekFinish = x.weekStart.AddDays(6) })
            //    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
            //    .Select((x, i) => new
            //    { x.weekStart, x.weekFinish, weekNum = i + 1 });
            //}

            //DateTime Today = Convert.ToDateTime(DateTime.Now.Date.ToString());
            //string SelectWeeks = "";
            //List<WeeksList> WeekList = new List<WeeksList>();
            //List<DateList> DateList = new List<DateList>();
            //List<DateList> DateRecord = new List<DateList>();
            //foreach (var X in weeks)
            //{
            //    DateTime CurrentDay = Today.Date;
            //    if (Today.DayOfWeek != DayOfWeek.Sunday)
            //    {
            //        CurrentDay = Today.Date.AddDays(7);
            //    }

            //    if(DateTime.Now.Month == 1)
            //    {
            //        if (X.weekStart.Date <= CurrentDay.Date && X.weekFinish.Date <= CurrentDay.Date)
            //        {
            //            SelectWeeks = X.weekStart.ToString("dd MMM yyyy") + " - " + X.weekFinish.ToString("dd MMM yyyy");
            //            var Value = X.weekStart.ToString("yyyy-MM-dd") + ";" + X.weekFinish.ToString("yyyy-MM-dd") + ";" + X.weekNum;
            //            WeekList.Add(new WeeksList { Weeks = Value, SelectDates = SelectWeeks });
            //            for (var dt = X.weekStart; dt <= X.weekFinish; dt = dt.AddDays(1))
            //            {
            //                DateList.Add(new DateList { Weeks = X.weekNum, Date = dt, DateName = dt.DayOfWeek.ToString() });
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (X.weekStart.Date <= CurrentDay.Date && X.weekFinish.Date <= CurrentDay.Date && X.weekFinish.Year == Today.Year)
            //        {
            //            SelectWeeks = X.weekStart.ToString("dd MMM yyyy") + " - " + X.weekFinish.ToString("dd MMM yyyy");
            //            var Value = X.weekStart.ToString("yyyy-MM-dd") + ";" + X.weekFinish.ToString("yyyy-MM-dd") + ";" + X.weekNum;
            //            WeekList.Add(new WeeksList { Weeks = Value, SelectDates = SelectWeeks });
            //            for (var dt = X.weekStart; dt <= X.weekFinish; dt = dt.AddDays(1))
            //            {
            //                DateList.Add(new DateList { Weeks = X.weekNum, Date = dt, DateName = dt.DayOfWeek.ToString() });
            //            }
            //        }
            //    }

                
            //}

            //foreach (var item in DateList)
            //{
            //    if (item.Weeks == WeekList.Count)
            //    {
            //        DateRecord.Add(new DateList { Weeks = item.Weeks, Date = item.Date, DateName = item.DateName });
            //    }
            //}


            //ViewBag.Weeks = JsonConvert.SerializeObject(WeekList);
            //ViewBag.Dates = JsonConvert.SerializeObject(DateList);
            //ViewBag.DatesArray = string.Join(",", DateRecord.Select(x => x.Date.ToString("yyyy-MM-dd")).ToArray());
            ViewBag.HeadOfServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
            ViewBag.RegionalProjectManager = _userHelper.GetByRoleName("Regional Project Manager").ToList();
            //ViewBag.Activity = _actiivity.GetAll().ToList();
            //ViewBag.TimeSheetType = _timesheet.GetAll().ToList();
            return base.Create();
        }

        public override IActionResult Create(AttendaceExceptionListModelForm model)
        {
            if (ModelState.IsValid)
            {
                //var TimeSheetType = _timesheet.GetById(model.TimeSheetTypeId);
                var item = Mapper.Map<AttendaceExceptionList>(model);

                Upload(item, model);

                //double TotalDays = (((model.DateEnd - model.DateStart)).TotalDays) + 1;
                //TotalDays = TotalDays < 1 ? 0 : TotalDays;


                //var CurrentUser = _userHelper.GetLoginUser(User);
                //var Contractor = _contractor.GetAll().Where(x => x.AccountId.Equals(CurrentUser.Id)).FirstOrDefault();
                //if (Contractor.AccountId != null)
                //{
                item.ContractorId = _userHelper.GetUser(User).UserProfile.Id;
                //item.AgencyId = Contractor.AgencyId;
                item.StatusOne = Domain.Models.Enum.StatusOne.Waiting;
                item.StatusTwo = Domain.Models.Enum.StatusTwo.Waiting;
                item.RequestStatus = ActiveStatus.Active;
                item.RemainingDays = 0;
                item.CreatedBy = _userHelper.GetUserId(User);
                item.CreatedAt = DateTime.Now;
                item.DateStart = model.DateStart;
                item.VacancyId = model.VacancyId;
                //}

                //if (!TimeSheetType.Type.ToLower().Trim().Equals("Annual leave".ToLower().Trim()))
                //{
                //    if (!string.IsNullOrEmpty(model.Hours) && !string.IsNullOrEmpty(model.AttendanceRecordDate))
                //    {
                //        var ListDate = model.AttendanceRecordDate.Split(',');
                //        item.DateStart = DateTime.Parse(ListDate[0]);
                //        item.DateEnd = DateTime.Parse(ListDate[ListDate.Length-1]);
                //    }
                //}
                Service.Add(item);

               
                //if(TimeSheetType.Type.ToLower().Trim().Equals("Annual leave".ToLower().Trim()))
                //{

                //    for (int i = 0; i < TotalDays; i++)
                //    {
                //        AttendanceRecord At = new AttendanceRecord()
                //        {
                //            AttendanceRecordDate = model.DateStart.AddDays(i),
                //            Hours = 8,
                //            AttendaceExceptionListId = item.Id
                //        };
                //        _attendanceRecord.Add(At);

                //    }

                //    // Add Hours
                //    var Tempz = Service.GetById(item.Id);
                //    Tempz.OtherInfo = (8 * TotalDays).ToString();
                //    Service.Update(Tempz);

                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(model.Hours) && !string.IsNullOrEmpty(model.AttendanceRecordDate))
                //    {
                //        var ListHours = model.Hours.Split(',');
                //        var ListDate = model.AttendanceRecordDate.Split(',');
                //        if (ListHours.Count() == ListHours.Count())
                //        {
                //            int TotalHours = 0;
                //            for (int i = 0; i < ListHours.Count(); i++)
                //            {

                //                AttendanceRecord At = new AttendanceRecord()
                //                {
                //                    AttendanceRecordDate = DateTime.Parse(ListDate[i]),
                //                    Hours = int.Parse(ListHours[i]),
                //                    AttendaceExceptionListId = item.Id
                //                };
                //                _attendanceRecord.Add(At);
                //                TotalHours += int.Parse(ListHours[i]);
                //            }

                //            // Add Hours
                //            var Tempz = Service.GetById(item.Id);
                //            Tempz.OtherInfo = TotalHours.ToString();
                //            Service.Update(Tempz);
                //        }
                //    }
                //}

                // Send Email To Approver
                //var callbackUrl = Url.Action("Index",
                //  "Registration",
                //   new { area = "Admin" },
                //  _hostConfiguration.Protocol,
                //  _hostConfiguration.Name);

                //var AppProfile = _profile.GetById(item.ApproverOneId.Value);
                //var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
                //_notif.Send(
                //    User, // User From
                //    "New Activity Request", // Subject
                //    AppProfile.Name, // User target name
                //    AppUser.Email, // User target email
                //    callbackUrl, // Link CallBack
                //    "New Activity Request : " + TimeSheetType.Type, // Email content or descriptions
                //    item.Description, // Description
                //    NotificationInboxStatus.Request, // Notif Status
                //    Activities.Request // Activity Status
                //);

                TempData["Success"] = "Success";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }

        protected override void UpdateData(AttendaceExceptionList item, AttendaceExceptionListModelForm model)
        {
            var TimeSheet = _timesheet.GetById(item.TimeSheetTypeId);
            if (TimeSheet.Type.ToLower().Trim().Equals("ANNUAL LEAVE".ToLower().Trim()))
            {
                var Total = (item.DateStart - item.DateEnd).TotalDays * 24;
                if (Total < 0)
                {
                    Total = Total * -1;
                }
                List<string> ListTotal = new List<string>() { Total.ToString() };
                //item.Hours = JsonConvert.SerializeObject(ListTotal);
            }
        }

       
        protected override void AfterUpdateData(AttendaceExceptionList before, AttendaceExceptionList after)
        {
            int ProfileId = 0;
            bool Send = false;
            if(after.StatusOne == StatusOne.Reject)
            {
                ProfileId = after.ApproverOneId.Value;
                Send = true;
            }

            if (after.StatusTwo == StatusTwo.Rejected)
            {
                ProfileId = after.ApproverTwoId.Value;
                Send = true;
            }

            if(Send==true)
            {

                var callbackUrl = Url.Action("Index",
                   "Registration",
                    new { area = "Admin"},
                   _hostConfiguration.Protocol,
                   _hostConfiguration.Name);

                var AppProfile = _profile.GetById(ProfileId);
                var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);

                _notif.Send(
                  User, // User From
                  "Activity Request Correction", // Subject
                  AppProfile.Name, // User target name
                  AppUser.Email, // User target email
                  callbackUrl, // Link CallBack
                  "Activity request has been correction please confirm.", // Email content or descriptions
                  after.Description, // Description
                  NotificationInboxStatus.Request, // Notif Status
                  Activities.Request // Activity Status
                );

                // Update Status
                var item = Service.GetById(after.Id);
                if(after.StatusOne== StatusOne.Reject)
                {
                    item.StatusOne = StatusOne.Waiting;
                    item.RequestStatus = ActiveStatus.Active;
                }

                if(after.StatusTwo == StatusTwo.Rejected)
                {
                    item.StatusTwo = StatusTwo.Waiting;
                    item.RequestStatus = ActiveStatus.Active;
                }

                Service.Update(item);
            }

            var TimeSheetType = _timesheet.GetById(after.TimeSheetTypeId);
            if (TimeSheetType.Type.ToLower().Trim().Equals("Annual leave".ToLower().Trim()))
            {
                var ContratorSrf = _userHelper.GetCurrentSrfByLogin(User);
                if (ContratorSrf != null)
                {
                    var srf = _srf.GetById(ContratorSrf.Id);
                    var Annual = (after.DateEnd.Month - after.DateStart.Month) <= 0 ? 0 : (after.DateEnd.Month - after.DateStart.Month);
                    srf.AnnualLeave = srf.AnnualLeave - Annual;
                    _srf.Update(srf);
                }

            }

        }

        [HttpPost]
        public IActionResult Approve(Guid id,int status)
        {
            DoAppove(id, status);
            TempData["Approved"] = "OK";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ApproveMultiple(string data, int status)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {
                        DoAppove(Guid.Parse(id), status);
                    }
                }
                TempData["Approved"] = "OK";
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public override IActionResult Edit(Guid id)
        {
            var item = Service.GetById(id);
            var Srf = _userHelper.GetCurrentSrfByLogin(User);
            ViewBag.Id = id;
            ViewBag.OrganizationUnit = _department.GetAll().ToList();
            ViewBag.CostCenter = _cost.GetAll().Where(x => x.Status == Status.Active).ToList();
            ViewBag.AccountName = _account.GetAll().ToList();
            ViewBag.NetworkNumber = _network.GetAll().Where(x => x.IsClosed == false).ToList();
            ViewBag.Project = _project.GetAll().ToList();
            ViewBag.Code = _code.GetAll().ToList();
            ViewBag.SubOperasional = _subOp.GetAll().ToList();
            ViewBag.City = _city.GetAll().ToList();
            ViewBag.Annual = Srf.AnnualLeave;
            // Start - Menampilkan Time Sheet
            var jan1 = new DateTime(DateTime.Today.Year, 1, 1);

            if (DateTime.Now.Month == 1)
            {
                jan1 = new DateTime(DateTime.Today.Year - 1, 12, 1);
            }

            var firstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks = Enumerable.Range(0, 54).Select(i => new
            { weekStart = firstWeek.AddDays(i * 7) })
                .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                .Select(x => new { x.weekStart, weekFinish = x.weekStart.AddDays(6) })
                .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                .Select((x, i) => new
                { x.weekStart, x.weekFinish, weekNum = i + 1 });

            if (DateTime.Now.Month == 1)
            {
                weeks = Enumerable.Range(0, 54).Select(i => new
                { weekStart = firstWeek.AddDays(i * 7) })
                .TakeWhile(x => x.weekStart.Year <= DateTime.Now.Year)
                .Select(x => new { x.weekStart, weekFinish = x.weekStart.AddDays(6) })
                .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                .Select((x, i) => new
                { x.weekStart, x.weekFinish, weekNum = i + 1 });
            }

            DateTime Today = Convert.ToDateTime(DateTime.Now.Date.ToString());
            string SelectWeeks = "";
            List<WeeksList> WeekList = new List<WeeksList>();
            List<DateList> DateList = new List<DateList>();
            int num = 0;
            foreach (var X in weeks)
            {
                DateTime CurrentDay = Today.Date;
                if (Today.DayOfWeek != DayOfWeek.Sunday)
                {
                    CurrentDay = Today.Date.AddDays(7);
                }

                if(DateTime.Now.Month == 1)
                {
                    if (X.weekStart.Date <= CurrentDay.Date && X.weekFinish.Date <= CurrentDay.Date)
                    {
                        SelectWeeks = X.weekStart.ToString("dd MMM yyyy") + " - " + X.weekFinish.ToString("dd MMM yyyy");
                        var Value = X.weekStart.ToString("yyyy-MM-dd") + ";" + X.weekFinish.ToString("yyyy-MM-dd") + ";" + X.weekNum;
                        WeekList.Add(new WeeksList { Weeks = Value, SelectDates = SelectWeeks });
                        for (var dt = X.weekStart; dt <= X.weekFinish; dt = dt.AddDays(1))
                        {
                            DateList.Add(new DateList { Weeks = X.weekNum, Date = dt, DateName = dt.DayOfWeek.ToString() });
                            num = X.weekNum;
                        }
                    }
                }
                else
                {
                    if (X.weekStart.Date <= CurrentDay.Date && X.weekFinish.Date <= CurrentDay.Date && X.weekFinish.Year == Today.Year)
                    {
                        SelectWeeks = X.weekStart.ToString("dd MMM yyyy") + " - " + X.weekFinish.ToString("dd MMM yyyy");
                        var Value = X.weekStart.ToString("yyyy-MM-dd") + ";" + X.weekFinish.ToString("yyyy-MM-dd") + ";" + X.weekNum;
                        WeekList.Add(new WeeksList { Weeks = Value, SelectDates = SelectWeeks });
                        for (var dt = X.weekStart; dt <= X.weekFinish; dt = dt.AddDays(1))
                        {
                            DateList.Add(new DateList { Weeks = X.weekNum, Date = dt, DateName = dt.DayOfWeek.ToString() });
                            num = X.weekNum;
                        }
                    }
                }

                
            }

            var TimeSelected = (string)item.DateStart.ToString("yyyy-MM-dd") + ";" + item.DateEnd.ToString("yyyy-MM-dd")+";"+num;
            var Selected = item.DateStart.ToString("dd MMM yyyy") + " - " + item.DateEnd.ToString("dd MMM yyyy");

            DateList.Add(new DateList { Weeks = num, Date = item.DateStart, DateName = item.DateStart.DayOfWeek.ToString() });
            WeekList.Add(new WeeksList { Weeks = TimeSelected, SelectDates = Selected });

            ViewBag.TimeSelected = TimeSelected;
            ViewBag.Weeks = JsonConvert.SerializeObject(WeekList);
            ViewBag.Dates = JsonConvert.SerializeObject(DateList);
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            ViewBag.ProjectManager = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.Activity = _actiivity.GetAll().ToList();
            ViewBag.TimeSheetType = _timesheet.GetAll().ToList();
            if (item != null)
            {
                var model = Mapper.Map<AttendaceExceptionListModelForm>(item);
                var ListHours = string.Join(",", _attendanceRecord.GetAll().Where(x => x.AttendaceExceptionListId.Equals(item.Id)).Select(x => x.Hours).ToArray());
                var ListDate = string.Join(",", _attendanceRecord.GetAll().Where(x => x.AttendaceExceptionListId.Equals(item.Id)).Select(x => x.AttendanceRecordDate.ToString("yyyy-MM-dd")).ToArray());
                model.Hours = ListHours;
                model.AttendanceRecordDate = ListDate;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public override IActionResult Edit(Guid id, AttendaceExceptionListModelForm model)
        {
            if (ModelState.IsValid)
            {

                var TimeSheetType = _timesheet.GetById(model.TimeSheetTypeId);
                var TempRecord = _attendanceRecord.GetAll().Where(x => x.AttendaceExceptionListId.Equals(id)).ToList();
                if (TempRecord != null)
                {
                    foreach (var row in TempRecord)
                    {
                        var Deleted = _attendanceRecord.GetById(row.Id);
                        if(Deleted!=null)
                        {
                            _attendanceRecord.Delete(Deleted);
                        }
                    }
                }

                var item = Service.GetById(id);
                if (item == null)
                {
                    return NotFound();
                }
                var before = item;
                Mapper.Map(model, item);

                var CurrentUser = _userHelper.GetLoginUser(User);
                var Contractor = _contractor.GetAll().Where(x => x.AccountId.Equals(CurrentUser.Id)).FirstOrDefault();
                if (Contractor.AccountId != null)
                {
                    item.ContractorId = Contractor.AccountId;
                    item.AgencyId = Contractor.AgencyId;
                    item.RemainingDays = 0;
                    item.LastEditedBy = _userHelper.GetUserId(User);
                    item.LastUpdateTime = DateTime.Now;
                }

                if (!TimeSheetType.Type.ToLower().Trim().Equals("Annual leave".ToLower().Trim()))
                {
                    if (!string.IsNullOrEmpty(model.Hours) && !string.IsNullOrEmpty(model.AttendanceRecordDate))
                    {
                        var ListDate = model.AttendanceRecordDate.Split(',');
                        item.DateStart = DateTime.Parse(ListDate[0]);
                        item.DateEnd = DateTime.Parse(ListDate[ListDate.Length-1]);
                    }
                }

                Service.Update(item);
                AfterUpdateData(before, item);

                if (TimeSheetType.Type.ToLower().Trim().Equals("Annual leave".ToLower().Trim()))
                {
                    double TotalDays = (((model.DateEnd - model.DateStart)).TotalDays) + 1;
                    TotalDays = TotalDays < 1 ? 0 : TotalDays;

                    for (int i = 1; i <= TotalDays; i++)
                    {
                        AttendanceRecord At = new AttendanceRecord()
                        {
                            AttendanceRecordDate = model.DateStart.AddDays(i),
                            Hours = 8,
                            AttendaceExceptionListId = item.Id
                        };
                        _attendanceRecord.Add(At);
                    }

                    // Add Hours
                    var Tempz = Service.GetById(item.Id);
                    Tempz.OtherInfo = (8 * TotalDays).ToString();
                    Service.Update(Tempz);
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Hours) && !string.IsNullOrEmpty(model.AttendanceRecordDate))
                    {
                        var ListHours = model.Hours.Split(',');
                        var ListDate = model.AttendanceRecordDate.Split(',');

                        if (ListHours.Count() == ListHours.Count())
                        {
                            int TotalHours = 0;
                            for (int i = 0; i < ListHours.Count(); i++)
                            {
                              
                                AttendanceRecord At = new AttendanceRecord()
                                {
                                    AttendanceRecordDate = DateTime.Parse(ListDate[i]),
                                    Hours = int.Parse(ListHours[i]),
                                    AttendaceExceptionListId = item.Id
                                };
                                _attendanceRecord.Add(At);
                                TotalHours += int.Parse(ListHours[i]);
                            }

                            // Add Hours
                            var Tempz = Service.GetById(item.Id);
                            Tempz.OtherInfo = TotalHours.ToString();
                            Service.Update(Tempz);
                        }
                    }
                }


                TempData["Success"] = "Activity registration request has been updated";
                return RedirectToAction("Details", new { id });
            }

            return View(model);
        }

        public override IActionResult Details(Guid id)
        {
            var item = Service.GetById(id);
            //ViewBag.Department = _department.GetById(item.DepartmentId).Name;
            //ViewBag.SubDepartment = _departmentSub.GetById(item.DepartmentSubId).SubName;
            //ViewBag.CostCenter = _cost.GetById(item.CostId).DisplayName;
            //ViewBag.AccountName = _account.GetById(item.AccountNameId).Name;
            //ViewBag.Network = _network.GetById(item.NetworkId).DisplayName;
            //ViewBag.Project = _project.GetById(item.ProjectId).Description;
            //ViewBag.Activity = _actiivity.GetById(item.ActivityId).DisplayName;
            //ViewBag.SubOperation = _subOp.GetById(item.SubOpsId).Description;
            //ViewBag.Location = _city.GetById(item.LocationId).Name;
            //ViewBag.TimesheetType = _timesheet.GetById(item.TimeSheetTypeId).Type.ToString();
            //ViewBag.TimesheetPeriode = item.DateStart.ToString("dd MMM yyyy") + " - " + item.DateEnd.ToString("dd MMM yyyy");
            ViewBag.Description = item.Description;
            ViewBag.ProjectManager = _profile.GetById(item.ApproverOneId).Name;
            ViewBag.LineManager = _profile.GetById(item.ApproverTwoId).Name;
            ViewBag.ContractorName = _vacancy.GetById(item.VacancyId).Name;
            return base.Details(id);
        }

        private void DoAppove(Guid id, int status,string remarks = null)
        {
            var item = Service.GetById(id);
            var UserLogin = _userHelper.GetUserId(User);
            var UserProfile = _profile.GetByUserId(UserLogin);

            var callbackUrl = Url.Action("Index",
                  "Registration",
                   new { area = "Admin"},
                  _hostConfiguration.Protocol,
                  _hostConfiguration.Name);

            // Approved
            if(status==2)
            {
                //int ContractorId = item.ContractorId.Value;

                if (UserProfile.Id == item.ApproverOneId && (item.StatusOne == StatusOne.Waiting || item.StatusOne == StatusOne.Reject))
                {
                    item.StatusOne = StatusOne.Approved;
                    item.ApprovedOneDate = DateTime.Now;
                }

                if (UserProfile.Id == item.ApproverTwoId && (item.StatusTwo == StatusTwo.Waiting || item.StatusTwo == StatusTwo.Rejected))
                {
                    item.StatusTwo = StatusTwo.Approved;
                    item.ApprovedTwoDate = DateTime.Now;
                    item.RequestStatus = ActiveStatus.Done;
                }

                var TypeTimeSheet = _timesheet.GetById(item.TimeSheetTypeId);
                //if (TypeTimeSheet.Type.ToLower().Trim() == "Annual Leave".ToLower().Trim())
                //{
                //    var Srf = _userHelper.GetCurrentSrfByUserProfile(item.ContractorId.Value);
                //    if (Srf != null)
                //    {
                //        TimeSpan ts = item.DateEnd.Subtract(item.DateStart);
                //        double total_day = ts.TotalDays == 0 ? 0 : ts.TotalDays;
                //        Srf.AnnualLeave = Srf.AnnualLeave - (int)total_day;
                //        _srf.Update(Srf);

                //        var Hours = _attendanceRecord.GetAll().Sum(x => x.Hours);
                //        var Annual = _userHelper.GetCurrentSrfByUserProfile(item.ContractorId.Value).AnnualLeave;
                //        item.RemainingDays = Annual;

                //    }
                //}

                //var AppProfile = _profile.GetById(ContractorId);
                //var AppUser = _userHelper.GetUser(AppProfile.ApplicationUserId);
                //_notif.Send(
                //    User, // User From
                //    "Activity request is approved by " + _userHelper.GetLoginUser(User).Name, // Subject
                //    AppProfile.Name, // User target name
                //    AppUser.Email, // User target email
                //    callbackUrl, // Link CallBack
                //    "Activity request is approved by " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                //    item.Description, // Description
                //    NotificationInboxStatus.Approval, // Notif Status
                //    Activities.Request // Activity Status
                // );

                Service.Update(item);
            }
            // Rejected
            else
            {
                item.StatusOne = StatusOne.Waiting;
                item.StatusTwo = StatusTwo.Waiting;
                item.ApprovedOneDate = DateTime.Now;
                item.ApprovedTwoDate = DateTime.Now;
                item.RequestStatus = ActiveStatus.Cancel;
                Service.Update(item);

                //var Contractor = _userHelper.GetUserProfile(item.ContractorId.Value);
                //var ContractorUser = _userHelper.GetUser(Contractor.ApplicationUserId);
                //_notif.Send(
                //  User, // User From
                //  "Activity Request Rejected", // Subject
                //  Contractor.Name, // User target name
                //  ContractorUser.Email, // User target email
                //  callbackUrl, // Link CallBack
                //  "Activity Has Been rejected By " + _userHelper.GetLoginUser(User).Name, // Email content or descriptions
                //  remarks, // Description
                //  NotificationInboxStatus.Reject, // Notif Status
                //  Activities.Request // Activity Status
                //);
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
                        DoAppove(Guid.Parse(id), 0, remarks);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Index");
        }

        private decimal GetRemaining(int hours,int annual)
        {
            var TotalAnnual = Math.Ceiling((decimal)hours / 24);
            var Remaining = annual - TotalAnnual;
            return Remaining <= 0 ? 0 : Remaining;
        }

        public IActionResult TestDate(string start ,string end)
        {
            var Start = start.Split('-');
            var End = end.Split('-');
            return Content(Start[0]+ " "+ Start[1]+" "+ Start[2]);
        }

        public IActionResult setTotalHours()
        {
            var data = Service.GetAll().ToList();
            if(data.Any())
            {
                foreach(var row in data)
                {
                    var item = Service.GetById(row.Id);
                    var totalHours = _attendanceRecord.GetAll().Where(x => x.AttendaceExceptionListId == row.Id).Sum(x => x.Hours);
                    item.OtherInfo = totalHours.ToString();
                    Service.Update(item);
                }
            }
            return Content("OK");
        }

       
    }
}
