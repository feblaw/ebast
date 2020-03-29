using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
using FileIO = System.IO.File;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.IO;
using System.Linq.Expressions;
using System.Globalization;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "HR Agency,Line Manager,Regional Project Manager,Administrator,Head Of Service Line,Head Of Operation")]
    public class VacancyController : BaseController<VacancyList, IVacancyListService, VacancyListViewModel, VacancyListFormModel, Guid>
    {

        private readonly IDepartementService _department;
        private readonly IAccountNameService _account;
        private readonly IServicePackCategoryService _serviceCategory;
        private readonly IPackageTypeService _packageType;
        private readonly IJobStageService _jobStage;
        private readonly IUserProfileService _user;
        private readonly INetworkNumberService _network;
        private readonly NotifHelper _notif;
        private readonly FileHelper _file;
        private readonly IUserHelper _userHelper;
        private readonly IUserService _userService;
        private readonly IUserProfileService _profileUser;
        private readonly IServicePackService _servicePack;
        private readonly IDepartementSubService _subDept;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HostConfiguration _hostConfiguration;
        private readonly ICostCenterService _costCenter;
        private readonly IVacancyListService _service;
        private readonly ExcelHelper _excel;


        public VacancyController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, IMapper mapper, 
            IVacancyListService service,
            IDepartementService department,
            IAccountNameService account,
            IServicePackCategoryService serviceCategory,
            IPackageTypeService packageType,
            IUserProfileService profileUser,
            IJobStageService jobStage,
            IUserProfileService user,
            IServicePackService servicePack,
            INetworkNumberService network,
            IDepartementSubService subDept,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            NotifHelper notif,
            FileHelper file,
            IHostingEnvironment env,
            ICostCenterService costCenter,
            ExcelHelper excel,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _department = department;
            _account = account;
            _serviceCategory = serviceCategory;
            _packageType = packageType;
            _jobStage = jobStage;
            _user = user;
            _notif = notif;
            _network = network;
            _userService = userService;
            _profileUser = profileUser;
            _servicePack = servicePack;
            _file = file;
            _userHelper = userHelper;
            _subDept = subDept;
            _env = env;
            _userManager = userManager;
            _hostConfiguration = hostConfiguration.Value;
            _costCenter = costCenter;
            _service = service;
            _excel = excel;
        }

        private string GetCurentUser()
        {
            var AppUser = _userHelper.GetUser(User);
            var UserProfile = _profileUser.GetByUserId(AppUser.Id);
            return UserProfile.Name;
        }

        [HttpGet]
        [Authorize(Roles = "HR Agency,Administrator,Line Manager,Sourcing,Head Of Service Line,Head Of Operation")]
        public override IActionResult Index()
        {
            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            ViewBag.UserRole = UserRole.FirstOrDefault();
            return base.Index();
        }

        [HttpGet]
        [Authorize(Roles = "Line Manager,Administrator")]
        public override IActionResult Create()
        {

            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            var CurrentUser = _userHelper.GetUser(User);
            var CurrentProfile = _profileUser.GetByUserId(CurrentUser.Id);
            if (UserRole.Contains("Line Manager"))
            {

                ViewBag.LineManagerId = CurrentProfile.Id;
                ViewBag.LineManagerName = CurrentProfile.Name;
            }
            else
            {
                ViewBag.LineManagerId = 0;
                ViewBag.LineManagerName = "";
            }

            Dictionary<string, int> ws = new Dictionary<string, int>();
            ws.Add("No", 0);
            ws.Add("Yes", 1);

            Dictionary<string, bool> com = new Dictionary<string, bool>();
            com.Add("No USIM",false);
            com.Add("USIM", true);

            Dictionary<string, bool> sign = new Dictionary<string, bool>();
            sign.Add("Non-HRMS", false);
            sign.Add("HRMS", true);

            ViewBag.OrganizationUnit = _department.GetAll().ToList();
            ViewBag.AccountName = _account.GetAll().ToList();
            ViewBag.PackageType = _packageType.GetAll().ToList();
            ViewBag.ServicePackCategory = _serviceCategory.GetAll().ToList();
            ViewBag.BasicServiceLevel = new List<int>(new int[] {0,20,30,40 }).Select(x=> new { Id = x, Name = x.ToString() });
            ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.Signum = sign.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
            ViewBag.Jobstage = _jobStage.GetAll().Where(x=>!string.IsNullOrEmpty(x.Description)).ToList();
            ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
            ViewBag.ProjectManager = _userHelper.GetByRoleName("Project Manager").ToList();
            ViewBag.ServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
            ViewBag.OpsHead = _userHelper.GetByRoleName("Head Of Operation").ToList();
            ViewBag.Rpm = _userHelper.GetByRoleName("Regional Project Manager").ToList();
            ViewBag.Vendor = _userHelper.GetByRoleName("HR Agency").ToList();
            ViewBag.NetworkNumber = _network.GetAll().ToList();
            return base.Create();
        }

        [Authorize(Roles = "Line Manager,Administrator")]
        protected override void Upload(VacancyList item,VacancyListFormModel model)
        {
            
            if (!string.IsNullOrWhiteSpace(model.Files))
            {
                string fileDestination = "uploads/vacancy";
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
                    if(listSaveAttachment!=null)
                    {
                        item.Files = JsonConvert.SerializeObject(listSaveAttachment);
                    }
                }
            }
          
        }

        [HttpGet]
        [Authorize(Roles = "HR Agency,Administrator,Line Manager,Sourcing,Head Of Service Line,Head Of Operation")]
        public override IActionResult Details(Guid id)
        {            
            try
            {
                Dictionary<string, int> ws = new Dictionary<string, int>();
                ws.Add("No", 0);
                ws.Add("Yes", 1);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                Dictionary<string, bool> sign = new Dictionary<string, bool>();
                sign.Add("Non-HRMS", false);
                sign.Add("HRMS", true);

                var data = _service.GetById(id);
                ViewBag.OrganizationUnit = _department.GetAll().ToList();
                ViewBag.AccountName = _account.GetAll().ToList();
                ViewBag.NetworkNumber = _network.GetAll().ToList();
                ViewBag.SubOrganizationUnit = _subDept.GetAll().Where(x => x.DepartmentId.Equals(data.DepartmentId)).ToList();
                ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(data.DepartmentId)).ToList();
                ViewBag.SSOW = _servicePack.GetAll().Where(x => x.ServicePackCategoryId.Equals(data.ServicePackCategoryId)).ToList();
                //ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _serviceCategory.GetAll().ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });
                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Signum = sign.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Jobstage = _jobStage.GetAll().Where(x => !string.IsNullOrEmpty(x.Description)).ToList();
                ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
                ViewBag.Sourcing = _userHelper.GetByRoleName("Sourcing").ToList();
                ViewBag.ProjectManager = _userHelper.GetByRoleName("Project Manager").ToList();
                ViewBag.ServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.OpsHead = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.Rpm = _userHelper.GetByRoleName("Regional Project Manager").ToList();
                ViewBag.Vendor = _userHelper.GetByRoleName("HR Agency").ToList();
                ViewBag.Id = id;

                List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
                var CurrentUser = _userHelper.GetUser(User);
                var CurrentProfile = _profileUser.GetByUserId(CurrentUser.Id);
                if (UserRole.Contains("Line Manager"))
                {

                    ViewBag.LineManagerId = CurrentProfile.Id;
                    ViewBag.LineManagerName = CurrentProfile.Name;
                }
                else
                {
                    ViewBag.LineManagerId = 0;
                    ViewBag.LineManagerName = "";
                }

                if (data != null)
                {
                    VacancyListFormModel model = Mapper.Map<VacancyListFormModel>(data);
                    if (!string.IsNullOrWhiteSpace(data.Files))
                    {
                        var attachments = JsonConvert.DeserializeObject<List<string>>(data.Files);
                        model.Files = string.Join("|", attachments.ToArray()) + "|";
                    }
                    model.NoarmalRate = model.NoarmalRate;
                    return View(model);
                }

            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Line Manager,Administrator")]
        public override IActionResult Edit(Guid id)
        {
            try
            {
                Dictionary<string, int> ws = new Dictionary<string, int>();
                ws.Add("No", 0);
                ws.Add("Yes", 1);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                Dictionary<string, bool> sign = new Dictionary<string, bool>();
                sign.Add("Non-HRMS", false);
                sign.Add("HRMS", true);

                var data = _service.GetById(id);
                ViewBag.OrganizationUnit = _department.GetAll().ToList();
                ViewBag.AccountName = _account.GetAll().ToList();
                ViewBag.NetworkNumber = _network.GetAll().ToList();
                ViewBag.SubOrganizationUnit = _subDept.GetAll().Where(x => x.DepartmentId.Equals(data.DepartmentId)).ToList();
                ViewBag.CostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(data.DepartmentId)).ToList();
                ViewBag.SSOW = _servicePack.GetAll().Where(x => x.ServicePackCategoryId.Equals(data.ServicePackCategoryId)).ToList();
                //ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _serviceCategory.GetAll().ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });
                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Signum = sign.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.Jobstage = _jobStage.GetAll().Where(x => !string.IsNullOrEmpty(x.Description)).ToList();
                ViewBag.LineManager = _userHelper.GetByRoleName("Line Manager").ToList();
                ViewBag.Sourcing = _userHelper.GetByRoleName("Sourcing").ToList();
                ViewBag.ProjectManager = _userHelper.GetByRoleName("Project Manager").ToList();
                ViewBag.ServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.OpsHead = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.Rpm = _userHelper.GetByRoleName("Regional Project Manager").ToList();
                ViewBag.Vendor = _userHelper.GetByRoleName("HR Agency").ToList();
                ViewBag.Id = id;

                List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
                var CurrentUser = _userHelper.GetUser(User);
                var CurrentProfile = _profileUser.GetByUserId(CurrentUser.Id);
                if (UserRole.Contains("Line Manager"))
                {

                    ViewBag.LineManagerId = CurrentProfile.Id;
                    ViewBag.LineManagerName = CurrentProfile.Name;
                }
                else
                {
                    ViewBag.LineManagerId = 0;
                    ViewBag.LineManagerName = "";
                }

                if (data != null)
                {
                    VacancyListFormModel model = Mapper.Map<VacancyListFormModel>(data);
                    if (!string.IsNullOrWhiteSpace(data.Files))
                    {
                        var attachments = JsonConvert.DeserializeObject<List<string>>(data.Files);
                        model.Files = string.Join("|", attachments.ToArray()) + "|";
                    }
                    model.NoarmalRate = model.NoarmalRate;
                    return View(model);
                }
                
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }

            return NotFound();
        }

        public IActionResult Pending()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public IActionResult Lmview()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public IActionResult Aspview()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public IActionResult Rpm()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        public IActionResult Expired()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        protected override void AfterCreateData(VacancyList item)
        {

            var callbackUrl = Url.Action("Details",
               "Vacancy",
                new { area = "Admin", id = item.Id },
               _hostConfiguration.Protocol,
               _hostConfiguration.Name);


            var UserAgency = _userHelper.GetByRoleName("HR Agency").ToList();
            if(UserAgency!=null)
            {
                foreach(var row in UserAgency)
                {
                    var AppUser = _userHelper.GetUser(row.ApplicationUserId);
                    var AppProfile = _profileUser.GetByUserId(row.ApplicationUserId);
                    _notif.Send(
                        User, // User From
                        "New job vacancy is submited", // Subject
                        AppProfile.Name, // User target name
                        AppUser.Email, // User target email
                        callbackUrl, // Link CallBack
                        "New job vacancy is submitted", // Email content or descriptions
                        item.Description, // Description
                        NotificationInboxStatus.Request, // Notif Status
                        Activities.Vacant // Activity Status
                    );
                }
            }

            base.AfterCreateData(item);
        }

        protected override void AfterUpdateData(VacancyList before, VacancyList after)
        {

            var callbackUrl = Url.Action("Details",
              "Vacancy",
               new { area = "Admin", id = after.Id },
              _hostConfiguration.Protocol,
              _hostConfiguration.Name);

            var UserLogin = _userHelper.GetLoginUser(User);

            if (before.ApproverOneId != after.ApproverOneId)
            {
                // Send Notif To Line Manager
                var LmProfile = _profileUser.GetById(after.ApproverOneId);
                var LmUser = _userService.GetById(LmProfile.ApplicationUserId);
               
                if (LmUser != null)
                {
                    _notif.Send(
                         User, // User From
                         "New job vacancy is submited", // Subject
                         LmProfile.Name, // User target name
                         LmUser.Email, // User target email
                         callbackUrl, // Link CallBack
                         "New job vacancy is submitted", // Email content or descriptions
                         after.Description, // Description
                         NotificationInboxStatus.Request, // Notif Status
                         Activities.Vacant // Activity Status
                    );
                }
            }

            if (before.ApproverTwoId != after.ApproverTwoId)
            {
                // Send Notif To Sourcing
                var SmProfile = _profileUser.GetById(after.ApproverTwoId);
                var SmUser = _userService.GetById(SmProfile.ApplicationUserId);
                if (SmUser != null)
                {
                    _notif.Send(
                         User, // User From
                         "New job vacancy is submited", // Subject
                         SmProfile.Name, // User target name
                         SmProfile.Email, // User target email
                         callbackUrl, // Link CallBack
                        "New job vacancy is submitted", // Email content or descriptions
                         after.Description, // Description
                         NotificationInboxStatus.Request, // Notif Status
                         Activities.Vacant // Activity Status
                    );
                }
            }

            TempData["Updated"] = "Success";
        }

        protected override void CreateData(VacancyList item)
        {
            item.RequestBy = _userHelper.GetUser(User).UserProfile;
            item.JoinDate = DateTime.Parse(item.JoinDate.ToString());
            item.VacancyStatus = ApproverStatus.Shortlist;
            item.CreatedAt = DateTime.Now;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.ApproverOne = _userHelper.GetUser(User).UserProfile;
            item.StatusOne = SrfApproveStatus.Approved;
            item.DateApprovedOne = DateTime.Now;
            item.Status = SrfStatus.Waiting;
        }

        protected override void UpdateData(VacancyList item, VacancyListFormModel model)
        {
            
            item.JoinDate = DateTime.Parse(model.JoinDate.ToString());
            item.DepartmentId = model.DepartmentId;
            item.DepartmentSubId = model.DepartmentSubId;
            item.CostCodeId = model.CostCodeId;
            item.NetworkId = model.NetworkId;
            item.JoinDate = model.JoinDate;
            item.PackageTypeId = model.PackageTypeId;
            item.ServicePackId = model.ServicePackId;
            item.ServicePackCategoryId = model.ServicePackCategoryId;
            item.OtLevel = model.OtLevel;
            item.isHrms = model.isHrms;
            item.isLaptop = (model.isLaptop == 1) ? true : false;
            item.isUsim = model.isUsim;
            item.JobStageId = model.JobStageId;
            item.Description = model.Description;
            
            //item.ApproverOneId = _userHelper.GetUser(User).UserProfile.Id;
            item.ApproverTwoId = model.ApproverTwoId;
            item.ApproverThreeId = model.ApproverThreeId;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.Quantity = model.Quantity;
            item.LastUpdateTime = DateTime.Now;
            item.StartDate = model.StartDate;
            item.EndDate = model.EndDate;
            item.Identifier = model.Identifier;
            item.VendorId = model.VendorId;
            item.NoarmalRate = model.NoarmalRate;
            item.NIK = model.NIK;
            item.RpmId = model.RpmId;
            item.Name = model.Name;
            if (User.IsInRole("Administrator"))
            {
                item.PONumber = model.PONumber;
                item.POInsertDate = DateTime.Now;
                item.POInsertedBy = _userHelper.GetUser(User).UserProfile.Name;
            }
            else
            {
                item.RequestBy = _userHelper.GetUser(User).UserProfile;
                item.ApproverOne = _userHelper.GetUser(User).UserProfile;
                item.ApproverOneId = _userHelper.GetUser(User).UserProfile.Id;
                item.StatusOne = SrfApproveStatus.Approved;
            }

           
            //item.ApproverOneId = _userHelper.GetUser(User).UserProfile;
        }

        private void MultiApprove(bool status, VacancyList item, string notes, string id, string type, int OperateOrNon, string Number = null)
        {
            #region MultiUser
            if (status == true)
            {
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                int SendApproval = 0;
                var Approver = UserApprover.LineManager;
                SendApproval = item.ApproverTwoId.Value;

                #region ServiceLine
                if (User.IsInRole("Head Of Service Line") && item.ApproverTwoId == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.StatusTwo = SrfApproveStatus.Approved;
                    item.DateApprovedTwo= DateTime.Now;
                    SendApproval = item.ApproverThreeId.Value;


                    Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region HeadOfOperation
                if (User.IsInRole("Head Of Operation") && item.ApproverThreeId == PreofileId)
                {
                    //item.NotesFirst = notes;
                    item.Status = SrfStatus.Done;
                    item.StatusThree = SrfApproveStatus.Approved;
                    item.DateApprovedThree = DateTime.Now;
                    //SendApproval = item.ApproveSixId.Value;

                    Approver = UserApprover.HeadOfOperation;
                }
                #endregion
                Service.Update(item);
            }
            else
            {
                // Send Notif Rejected
                //item.Status = SrfStatus.Waiting;
                item.StatusOne = SrfApproveStatus.Reject;
                item.StatusTwo= SrfApproveStatus.Reject;
                item.StatusThree= SrfApproveStatus.Reject;
                //item.ApproveStatusFour = SrfApproveStatus.Waiting;
                //item.ApproveStatusFive = SrfApproveStatus.Waiting;
                //item.ApproveStatusSix = SrfApproveStatus.Waiting;
                Service.Update(item);


            }
            #endregion
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
                        ApproveGeneral(id, false, remarks, null);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Pending");
        }

        private void ApproveGeneral(string id, bool status, string notes = null, string number = null)
        {
            var item = Service.GetById(Guid.Parse(id));
            var Dept = _department.GetById(item.DepartmentId);
            if (item != null)
            {
                MultiApprove(status, item, notes, id, "1", Dept.OperateOrNon, number);
            }

        }


        public IActionResult ApproveMultiple(string data, bool status)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {

                        if (User.IsInRole("Service Coordinator"))
                        {
                            ApproveGeneral(id, status, null);
                        }
                        else
                        {
                            ApproveGeneral(id, status, null, null);
                        }

                    }
                    if (status == true)
                    {
                        TempData["Approved"] = "OK";
                    }
                    else
                    {
                        TempData["Rejected"] = "OK";
                    }
                }
            }
            return RedirectToAction("Pending");
        }

        public IActionResult ApproveBastMultiple(string data, int status)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var SrfId = data.Split(',');
                if (SrfId != null)
                {
                    foreach (var id in SrfId)
                    {

                        
                            ApproveGeneralBast(id, status);
                        

                    }
                    TempData["Approved"] = "OK";
                    
                }
            }
            return RedirectToAction("Rpm");
        }

        private void ApproveGeneralBast(string id, int status)
        {
            var item = Service.GetById(Guid.Parse(id));
            //var Dept = _department.GetById(item.DepartmentId);
            if (item != null)
            {
                MultiApproveBast(status, item);
            }

        }

        private void MultiApproveBast(int status, VacancyList item)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            if(User.IsInRole("Regional Project Manager"))
            {
                if (status == 1)
                {
                    if (item.BastStatus1 == SrfApproveStatus.Waiting)
                    {
                        item.BastApprover1Id = PreofileId;
                        item.DateBastApproved1 = DateTime.Now;
                        item.BastStatus1 = SrfApproveStatus.Approved;
                    }
                }
                else if (status == 2)
                {
                    if (item.BastStatus1 == SrfApproveStatus.Approved && item.BastStatus2 == SrfApproveStatus.Waiting && item.BastStatusSL1 == SrfApproveStatus.Approved)
                    {
                        item.BastApprover2Id = PreofileId;
                        item.DateBastApproved2 = DateTime.Now;
                        item.BastStatus2 = SrfApproveStatus.Approved;
                    }

                }
                else if (status == 3)
                {
                    if (item.BastStatus2 == SrfApproveStatus.Approved && item.BastStatus3 == SrfApproveStatus.Waiting && item.BastStatusSL2 == SrfApproveStatus.Approved)
                    {
                        item.BastApprover3Id = PreofileId;
                        item.DateBastApproved3 = DateTime.Now;
                        item.BastStatus3 = SrfApproveStatus.Approved;
                    }

                }
            }
            else
            {
                if (status == 1 && item.BastStatus1 == SrfApproveStatus.Approved)
                {
                    if (item.BastStatusSL1 == SrfApproveStatus.Waiting)
                    {
                        item.BastApproverSL1Id = PreofileId;
                        item.DateBastApprovedSL1 = DateTime.Now;
                        item.BastStatusSL1 = SrfApproveStatus.Approved;
                    }
                }
                else if (status == 2 && item.BastStatus2 == SrfApproveStatus.Approved)
                {
                    if (item.BastStatusSL1 == SrfApproveStatus.Approved && item.BastStatusSL2 == SrfApproveStatus.Waiting)
                    {
                        item.BastApproverSL2Id = PreofileId;
                        item.DateBastApprovedSL2 = DateTime.Now;
                        item.BastStatusSL2 = SrfApproveStatus.Approved;
                    }

                }
                else if (status == 3 && item.BastStatus3 == SrfApproveStatus.Approved)
                {
                    if (item.BastStatusSL2 == SrfApproveStatus.Approved && item.BastStatusSL1 == SrfApproveStatus.Approved && item.BastStatusSL3 == SrfApproveStatus.Waiting)
                    {
                        item.BastApproverSL3Id = PreofileId;
                        item.DateBastApprovedSL3 = DateTime.Now;
                        item.BastStatusSL3 = SrfApproveStatus.Approved;
                    }

                }
            }
           
            Service.Update(item);
        }

        public IActionResult Terminate(string id, string notes, int submit, string date = null)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (User.IsInRole("Line Manager") || User.IsInRole("Administrator"))
            {
                //item.Status = SrfStatus.WaitingTerminate;
                item.EndDate = DateTime.Now;
                item.Status = SrfStatus.Terminate;
                item.TerminateDate = DateTime.Now;
                item.TerminateNote = notes;
                item.TerminateBy = _userHelper.GetLoginUser(User).Name;

                Service.Update(item);
                TempData["Terminated"] = "OK";
                return RedirectToAction("Index");

                // Send To Agency
                //var Candidate = _candidate.GetById(item.CandidateId);
                //var AgencyProfile = _profileService.GetById(Candidate.AgencyId);
                //var AgencyUser = _userHelper.GetUser(AgencyProfile.ApplicationUserId);
                //var LineManager = _userHelper.GetUserProfile(item.ApproveOneId.Value);
                //if (AgencyUser != null)
                //{
                //    var callbackUrl = Url.Action("Index",
                //      "Srf",
                //       new { area = "Admin" },
                //      _hostConfiguration.Protocol,
                //      _hostConfiguration.Name);

                //    string What = submit == 3 ? "Terminate" : "Blacklist";

                //    var Data = new Dictionary<string, string>()
                //        {
                //            { "AgencyName", AgencyProfile.Name},
                //            { "SubjectTitle", What},
                //            { "NameResource", Candidate.Name},
                //            { "ContractEnd", item.SrfEnd.Value.ToString("dd MMM yyyy") },
                //            { "Note", item.Description },
                //            { "LineManagerName", LineManager.Name },
                //        };

                //    _notif.Send(
                //        User, // User From
                //        "Request " + What + " Resource", // Subject
                //        AgencyProfile.Name, // User target name
                //        AgencyUser.Email, // User target email
                //        callbackUrl, // Link CallBack
                //        null, // Email content or descriptions
                //        null, // Description
                //        NotificationInboxStatus.Request, // Notif Status
                //        Activities.SRF, // Activity Status,
                //        null,
                //        "Emails/Terminate",
                //        Data
                //    );


                //    TempData["Terminate"] = "OK";
                //    return RedirectToAction("Index");
                //}
            }
            //else
            //{
            //    if (item != null)
            //    {
            //        var Candidate = _candidate.GetById(item.CandidateId);
            //        var UserProfile = _profileService.GetById(Candidate.AccountId);
            //        item.TeriminateNote = notes;
            //        item.TerimnatedBy = _userHelper.GetLoginUser(User).Name;
            //        if (!string.IsNullOrWhiteSpace(date))
            //        {
            //            item.TerminatedDate = DateTime.Parse(date);
            //        }
            //        else
            //        {
            //            item.TerminatedDate = DateTime.Now;
            //        }
            //        if (submit == 3)
            //        {
            //            item.Status = SrfStatus.Terminate;
            //            UserProfile.IsTerminate = true;
            //            UserProfile.IsActive = false;
            //            _profileService.Update(UserProfile);
            //        }
            //        else
            //        {
            //            item.Status = SrfStatus.Blacklist;
            //            UserProfile.IsBlacklist = true;
            //            UserProfile.IsActive = false;
            //            _profileService.Update(UserProfile);
            //        }
            //        Service.Update(item);
            //        TempData["Terminated"] = "OK";
            //        return RedirectToAction("Index");
            //    }
            //}
            return NotFound();
        }

        public IActionResult Extends(Guid id)
        {
            var item = Service.GetById(id);
            if (item != null)
            {

                VacancyList wp = new VacancyList
                {
                    //Type = SrfType.Extension,
                    //ApproveOneBy = item.ApproveOneBy,
                    //CandidateId = item.CandidateId,
                    AccountNameId = item.AccountNameId,
                    ApproverOneId = item.ApproverOneId,
                    ApproverTwoId = item.ApproverTwoId,
                    ApproverThreeId = item.ApproverThreeId,
                    CostCodeId = item.CostCodeId,
                    CreatedAt = DateTime.Now,
                    DateApprovedOne = DateTime.Now,
                    DepartmentId = item.DepartmentId,
                    DepartmentSubId = item.DepartmentSubId,
                    Description = item.Description,
                    Files = item.Files,
                    NetworkId = item.NetworkId,
                    NoarmalRate = item.NoarmalRate,
                    PackageTypeId = item.PackageTypeId,
                    ServicePackId = item.ServicePackId,
                    RequestById = item.RequestById,
                    ServicePackCategoryId = item.ServicePackCategoryId,
                    Name = item.Name,
                    Identifier = item.Identifier,
                    VendorId = item.VendorId,
                    StartDate = item.EndDate.AddDays(1),
                    EndDate = item.EndDate.AddMonths(3),
                    Quantity = item.Quantity,
                    NIK = item.NIK,
                    RpmId = item.RpmId,
                    StatusOne = SrfApproveStatus.Approved
                    
                };
                Service.Add(wp);
                TempData["Extend"] = "OK";
                return RedirectToAction("Edit", new { Id = wp.Id });
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
            int TOTAL_UPDATE = 0;
            var uploads = System.IO.Path.Combine(_env.WebRootPath, "temp");
            file = Request.Form.Files[0];
            using (var fileStream = new FileStream(System.IO.Path.Combine(uploads, file.FileName),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                await file.CopyToAsync(fileStream);
                try
                {
                    using (var Stream = new FileStream(System.IO.Path.Combine(uploads, file.FileName),
                          FileMode.OpenOrCreate,
                          FileAccess.ReadWrite,
                          FileShare.ReadWrite))
                    {
                        using (ExcelPackage package = new ExcelPackage(Stream))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                if (worksheet.Cells[row, 1].Value != null &&
                                    worksheet.Cells[row, 2].Value != null)
                                {
                                    var idWP = worksheet.Cells[row, 1].Value.ToString();
                                    var poNumber = worksheet.Cells[row, 2].Value.ToString();
                                    //var Department = worksheet.Cells[row, 3].Value.ToString();
                                    //var LineManager = worksheet.Cells[row, 4].Value.ToString();
                                    //var ProjectManager = worksheet.Cells[row, 5].Value.ToString();
                                    //var AccountName = worksheet.Cells[row, 6].Value.ToString();
                                    //var Description = (worksheet.Cells[row, 7].Value!=null) ? worksheet.Cells[row, 7].Value.ToString() : "";

                                    VacancyList vacancyFind = Service
                                       .GetAll()
                                       .FirstOrDefault(x => _excel.TruncateString(x.Id.ToString()) == _excel.TruncateString(idWP));

                                    //Projects ProjectFind = projects
                                    //    .GetAll()
                                    //    .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Project));

                                    //Departement DepartmentFind = departement
                                    //  .GetAll()
                                    //  .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Department));

                                    //UserProfile LineManagerUser = await FindUserByRole(LineManager, "Project Manager");
                                    //UserProfile ProjectManagerUser = await FindUserByRole(ProjectManager, "Project Manager");

                                    //AccountName AccountFind = accountName
                                    // .GetAll()
                                    // .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(AccountName.ToString()));


                                    //if(ProjectFind==null)
                                    //{
                                    //    Projects pj = new Projects { Name = Project };
                                    //    ProjectFind = projects.Add(pj);
                                    //}

                                    if (poNumber != null)
                                    {
                                        if (vacancyFind == null)
                                        {

                                            VacancyList nt = new VacancyList
                                            {
                                                PONumber = poNumber,
                                                POInsertDate = DateTime.Now,
                                                POInsertedBy = _userHelper.GetUser(User).UserProfile.Name
                                            };
                                            Service.Add(nt);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {

                                            VacancyList nt = Service.GetById(vacancyFind.Id);
                                            nt.PONumber = poNumber;
                                            nt.POInsertDate = DateTime.Now;
                                            nt.POInsertedBy = _userHelper.GetUser(User).UserProfile.Name;
                                            Service.Update(nt);
                                            TOTAL_UPDATE++;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT + " , Total Updated = " + TOTAL_UPDATE;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }

        public override IActionResult lmrtemplate()
        {
            var Profile = _userHelper.GetLoginUser(User);
            var ProfileId = Profile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/BAST.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                int i = 0;
                int index = 1;
                worksheet.Cells[index, i += 1].Value = "Employee Name";
                worksheet.Cells[index, i += 1].Value = "ASP";
                worksheet.Cells[index, i += 1].Value = "Account";
                worksheet.Cells[index, i += 1].Value = "Start Date";//ah id
                worksheet.Cells[index, i += 1].Value = "End Date";
                worksheet.Cells[index, i += 1].Value = "RPM";
                worksheet.Cells[index, i += 1].Value = "RPM Status - Month 1";
                worksheet.Cells[index, i += 1].Value = "RPM Approved Date - Month 1";
                worksheet.Cells[index, i += 1].Value = "SL Status - Month 1";
                worksheet.Cells[index, i += 1].Value = "SL Approved Date - Month 1";
                worksheet.Cells[index, i += 1].Value = "RPM Status - Month 2";
                worksheet.Cells[index, i += 1].Value = "RPM Approved Date - Month 2";
                worksheet.Cells[index, i += 1].Value = "SL Status - Month 2";
                worksheet.Cells[index, i += 1].Value = "SL Approved Date - Month 2";
                worksheet.Cells[index, i += 1].Value = "RPM Status - Month 3";
                worksheet.Cells[index, i += 1].Value = "RPM Approved Date - Month 3";
                worksheet.Cells[index, i += 1].Value = "SL Status - Month 3";
                worksheet.Cells[index, i += 1].Value = "SL Approved Date - Month 3";



                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;
                Expression<Func<VacancyList, object>>[] Includes = new Expression<Func<VacancyList, object>>[11];
                Includes[0] = pack => pack.ServicePack;
                Includes[1] = pack => pack.ServicePackCategory;
                Includes[2] = pack => pack.ApproverOne;
                Includes[3] = pack => pack.Candidate;
                Includes[4] = pack => pack.Departement;
                Includes[5] = pack => pack.DepartementSub;
                Includes[6] = pack => pack.Vendor;
                Includes[7] = pack => pack.ApproverTwo;
                Includes[8] = pack => pack.ApproverThree;
                Includes[9] = pack => pack.Network;
                Includes[10] = pack => pack.Rpm;


                var Data = Service.GetAll(Includes).Where(x => x.EndDate.AddMonths(2) > DateTime.Now && x.StatusOne == SrfApproveStatus.Approved && x.StatusTwo == SrfApproveStatus.Approved
                && x.StatusThree == SrfApproveStatus.Approved).ToList();

                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.Name;
                        worksheet.Cells[index, j += 1].Value = row.Vendor.Name;
                        worksheet.Cells[index, j += 1].Value = row.Departement.Name;
                        worksheet.Cells[index, j += 1].Value = row.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Rpm.Name;
                        worksheet.Cells[index, j += 1].Value = row.BastStatus1;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApproved1.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.BastStatusSL1;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApprovedSL1.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.BastStatus2;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApproved2.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.BastStatusSL2;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApprovedSL2.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.BastStatus3;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApproved3.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.BastStatusSL3;
                        worksheet.Cells[index, j += 1].Value = row.DateBastApprovedSL3.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        index++;
                    }
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return Redirect(URL);
        }
    }
}
