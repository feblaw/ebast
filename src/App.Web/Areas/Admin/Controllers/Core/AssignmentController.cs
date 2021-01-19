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
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
using System.Net;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, ASP Admin")]
    public class AssignmentController : BaseController<Assignment, IAssignmentService, AssignmentViewModel, AssignmentFormModel, Guid>
    {

        private readonly IUserProfileService _user;
        private readonly NotifHelper _notif;
        private readonly FileHelper _file;
        private readonly IUserHelper _userHelper;
        private readonly IUserService _userService;
        private readonly IUserProfileService _profileUser;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HostConfiguration _hostConfiguration;
        private readonly IAssignmentService _service;
        private readonly ExcelHelper _excel;
        private readonly IASPService _asp;
        private readonly IMapAsgBastService _mappingAsgBast;
        private readonly IBastService _bast;
        private readonly IWebSettingService _webset;


        public AssignmentController(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IMapper mapper,
            IAssignmentService service,
            IUserProfileService profileUser,
            IUserProfileService user,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            NotifHelper notif,
            FileHelper file,
            IHostingEnvironment env,
            ExcelHelper excel,
            IASPService asp,
            IMapAsgBastService mappingAsgBast,
            IBastService bast,
            IWebSettingService webset,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _user = user;
            _notif = notif;
            _userService = userService;
            _profileUser = profileUser;
            _file = file;
            _userHelper = userHelper;
            _env = env;
            _userManager = userManager;
            _hostConfiguration = hostConfiguration.Value;
            _service = service;
            _excel = excel;
            _bast = bast;
            _mappingAsgBast = mappingAsgBast;
            _webset = webset;
            this._asp = asp;
        }

        private string GetCurentUser()
        {
            var AppUser = _userHelper.GetUser(User);
            var UserProfile = _profileUser.GetByUserId(AppUser.Id);
            return UserProfile.Name;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,ASP Admin")]
        public override IActionResult Index()
        {
            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            ViewBag.UserRole = UserRole.FirstOrDefault();
            return base.Index();
        }


        [HttpGet]
        [Authorize(Roles = "Administrator,ASP")]
        public override IActionResult Details(Guid id)
        {
            try
            {

                var data = _service.GetById(id);
                ViewBag.Id = id;

            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

            return NotFound();
        }


        protected override void AfterCreateData(Assignment item)
        {

            var callbackUrl = Url.Action("Details",
               "Vacancy",
                new { area = "Admin", id = item.Id },
               _hostConfiguration.Protocol,
               _hostConfiguration.Name);


            var UserAgency = _userHelper.GetByRoleName("HR Agency").ToList();
            //if (UserAgency != null)
            //{
            //    foreach (var row in UserAgency)
            //    {
            //        var AppUser = _userHelper.GetUser(row.ApplicationUserId);
            //        var AppProfile = _profileUser.GetByUserId(row.ApplicationUserId);
            //        _notif.Send(
            //            User, // User From
            //            "New job vacancy is submited", // Subject
            //            AppProfile.Name, // User target name
            //            AppUser.Email, // User target email
            //            callbackUrl, // Link CallBack
            //            "New job vacancy is submitted", // Email content or descriptions
            //                                            //item.Description, // Description
            //            NotificationInboxStatus.Request, // Notif Status
            //            Activities.Vacant // Activity Status
            //        );
            //    }
            //}

            base.AfterCreateData(item);
        }

        protected override void AfterUpdateData(Assignment before, Assignment after)
        {

            var callbackUrl = Url.Action("Details",
              "Vacancy",
               new { area = "Admin", id = after.Id },
              _hostConfiguration.Protocol,
              _hostConfiguration.Name);

            var UserLogin = _userHelper.GetLoginUser(User);

            //if (before.ApproverOneId != after.ApproverOneId)
            //{
            //    // Send Notif To Line Manager
            //    var LmProfile = _profileUser.GetById(after.ApproverOneId);
            //    var LmUser = _userService.GetById(LmProfile.ApplicationUserId);

            //    if (LmUser != null)
            //    {
            //        _notif.Send(
            //             User, // User From
            //             "New job vacancy is submited", // Subject
            //             LmProfile.Name, // User target name
            //             LmUser.Email, // User target email
            //             callbackUrl, // Link CallBack
            //             "New job vacancy is submitted", // Email content or descriptions
            //             after.Description, // Description
            //             NotificationInboxStatus.Request, // Notif Status
            //             Activities.Vacant // Activity Status
            //        );
            //    }
            //}

            //if (before.ApproverTwoId != after.ApproverTwoId)
            //{
            //    // Send Notif To Sourcing
            //    var SmProfile = _profileUser.GetById(after.ApproverTwoId);
            //    var SmUser = _userService.GetById(SmProfile.ApplicationUserId);
            //    if (SmUser != null)
            //    {
            //        _notif.Send(
            //             User, // User From
            //             "New job vacancy is submited", // Subject
            //             SmProfile.Name, // User target name
            //             SmProfile.Email, // User target email
            //             callbackUrl, // Link CallBack
            //            "New job vacancy is submitted", // Email content or descriptions
            //             after.Description, // Description
            //             NotificationInboxStatus.Request, // Notif Status
            //             Activities.Vacant // Activity Status
            //        );
            //    }
            //}

            TempData["Updated"] = "Success";
        }

        protected override void UpdateData(Assignment item, AssignmentFormModel model)
        {


            if (User.IsInRole("Administrator"))
            {
                item.AssignmentId = model.AssignmentId;
                item.SHID = model.SHID;
                item.ProjectName = model.ProjectName;
                item.SiteID = model.SiteID;
                item.SiteName = model.SiteName;
                item.AssignmentCreateDate = model.AssignmentCreateDate;
                item.AssignmentAcceptedDate = model.AssignmentAcceptedDate;
                item.PRNo = model.PRNo;
                item.PRDateCreated = model.PRDateCreated;
                item.PONumber = model.PONumber;
                item.PODate = model.PODate;
                item.LineItemPO = model.LineItemPO;
                item.AssignmentReady4Bast = model.AssignmentReady4Bast;
                item.ShortTextPO = model.ShortTextPO;
                item.ValueAssignment = model.ValueAssignment;
                item.TOP = model.TOP;
            }
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
                                    DateTime dt;
                                    var assigmentId = worksheet.Cells[row, 1].Value.ToString();
                                    var shId = worksheet.Cells[row, 2].Value.ToString();
                                    var siteId = "-";
                                    if (worksheet.Cells[row, 3].Value != null)
                                    {
                                        siteId = worksheet.Cells[row, 3].Value.ToString();
                                    }

                                    var siteName = "-";
                                    if (worksheet.Cells[row, 4].Value != null)
                                    {
                                        siteName = worksheet.Cells[row, 4].Value.ToString();
                                    }
                                    var assigmentAcceptDate = DateTime.Parse(worksheet.Cells[row, 5].Text);
                                    var prNumber = worksheet.Cells[row, 6].Value.ToString();
                                    var prDate = DateTime.Parse(worksheet.Cells[row, 7].Text);
                                    var poNumber = worksheet.Cells[row, 8].Value.ToString();
                                    var poDate = DateTime.Parse(worksheet.Cells[row, 9].Text);
                                    var poLineItem = worksheet.Cells[row, 10].Value.ToString();
                                    var shortextPo = worksheet.Cells[row, 11].Value.ToString();
                                    var project = worksheet.Cells[row, 12].Value.ToString();
                                    var valueAssigment = Decimal.Parse(worksheet.Cells[row, 13].Value.ToString());
                                    var top = worksheet.Cells[row, 14].Value.ToString();
                                    var assignmentCreateBy = worksheet.Cells[row, 15].Value.ToString();
                                    var assignmentCreateDate = DateTime.Parse(worksheet.Cells[row, 16].Text);
                                    var asp = worksheet.Cells[row, 17].Value.ToString();
                                    var assignmentCancel = Boolean.Parse(worksheet.Cells[row, 18].Value.ToString());
                                    var sow = worksheet.Cells[row, 19].Value.ToString();


                                    ASP aspFind = _asp.GetAll().FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(asp));




                                    Assignment AssignmentFind = Service
                                       .GetAll()
                                       .FirstOrDefault(x => _excel.TruncateString(x.AssignmentId.ToString()) == _excel.TruncateString(assigmentId));


                                    if (AssignmentFind == null)
                                    {

                                        Assignment nt = new Assignment
                                        {
                                            AssignmentId = assigmentId,
                                            ProjectName = project,
                                            SiteID = siteId,
                                            SiteName = siteName,
                                            AssignmentAcceptedDate = assigmentAcceptDate,
                                            PRNo = prNumber,
                                            PRDateCreated = prDate,
                                            PONumber = poNumber,
                                            PODate = poDate,
                                            LineItemPO = poLineItem,
                                            AssignmentReady4Bast = true,
                                            ShortTextPO = shortextPo,
                                            SHID = shId,
                                            ValueAssignment = valueAssigment,
                                            TOP = top,
                                            AssignmentCreateBy = assignmentCreateBy,
                                            AssignmentCreateDate = assignmentCreateDate,
                                            AspId = aspFind.Id,
                                            AssignmentCancel = assignmentCancel,
                                            Sow = sow
                                        };
                                        Service.Add(nt);
                                        //TempData["Messages"] = "Success add Assignment Id " + assigmentId;
                                        Console.WriteLine("Success add Assignment Id " + assigmentId);
                                        TOTAL_INSERT++;
                                    }
                                    else if (AssignmentFind != null && assignmentCancel != AssignmentFind.AssignmentCancel)
                                    {

                                        Assignment nt = Service.GetById(AssignmentFind.Id);
                                        nt.ProjectName = project;
                                        nt.SiteID = siteId;
                                        nt.SiteName = siteName;
                                        nt.AssignmentAcceptedDate = assigmentAcceptDate;
                                        nt.PRNo = prNumber;
                                        nt.PRDateCreated = prDate;
                                        nt.PONumber = poNumber;
                                        nt.PODate = poDate;
                                        nt.LineItemPO = poLineItem;
                                        nt.ShortTextPO = shortextPo;
                                        nt.SHID = shId;
                                        nt.ValueAssignment = valueAssigment;
                                        nt.TOP = top;
                                        nt.AssignmentCreateBy = assignmentCreateBy;
                                        nt.AssignmentCreateDate = assignmentCreateDate;
                                        nt.AspId = AssignmentFind.AspId;
                                        nt.AssignmentCancel = assignmentCancel;
                                        nt.AssignmentReady4Bast = true;
                                        nt.Sow = sow;
                                        nt.OtherInfo = shortextPo;
                                        Service.Update(nt);
                                        //TempData["Messages"] = "Success update Assignment Id " + assigmentId;
                                        Console.WriteLine("Success update Assignment Id " + assigmentId);
                                        TOTAL_UPDATE++;
                                    }



                                }
                            }
                        }
                    }
                    TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT + " , Total Updated = " + TOTAL_UPDATE;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    TempData["Messages"] = ex.ToString();
                }
            }

            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }

        public IActionResult ReqBast(string data, int status)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var AsgId = data.Split(',');
                if (AsgId != null)
                {
                    //initiate variabel yang dibutuhkan
                    var a = 1;
                    var cekValidPo = true;
                    var cekAsgNotValid = 0;
                    var noPoAwal = "";
                    var noPo = "";
                    var listAsg = 0;
                    var error = "";
                    var top = "";
                    var topFix = "";
                    bool final = false;
                    decimal topCalculation = 0;
                    decimal valueBast = 0;
                    string project = "";
                    string sow = "";
                    string account = "";
                    foreach (var id in AsgId)
                    {
                        //cek po valid
                        if (a == 1)
                        {
                            noPoAwal = getNoPO(id);
                            project = getProject(id);
                            sow = getSow(id);
                            account = getAccount(id);
                        }
                        noPo = getNoPO(id);

                        if (noPo != noPoAwal)
                        {
                            cekValidPo = false;

                        }
                        a = a + 1;

                        //cek assignment apakah sudah di bast atau belum
                        top = getTOP(id);
                        try
                        {
                            if (top == "100%")
                            {
                                listAsg = _mappingAsgBast.GetAllQ().Where(x => x.IdAsg.ToString() == id && x.Bast.TOP.Equals(top) &&
                                x.Bast.ApprovalOneStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalTwoStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalThreeStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalFourStatus != BastApproveStatus.Reject
                                )
                                .Count();

                                if (listAsg == 0)
                                {
                                    //cekAsgNotValid = true;
                                    topFix = "100%";
                                    final = true;
                                }
                                else
                                {
                                    cekAsgNotValid = cekAsgNotValid + 1;
                                }
                            }
                            else if (top == "30% - 70%" || top == "30%-70%" || top == "70%-30%")
                            {
                                //cek top pertama
                                var topp = "30% - 70%";
                                listAsg = _mappingAsgBast.GetAllQ().Where(x => x.IdAsg.ToString() == id && x.Bast.TOP == topp.Substring(0, 3) &&
                                x.Bast.ApprovalOneStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalTwoStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalThreeStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalFourStatus != BastApproveStatus.Reject
                                )
                                .Count();

                                if (listAsg == 0)
                                {
                                    //cekAsgNotValid = true;
                                    topFix = "30%";
                                }
                                else //cek top kedua
                                {
                                    listAsg = _mappingAsgBast.GetAllQ().Where(x => x.IdAsg.ToString() == id && x.Bast.TOP == topp.Substring(6, 3) &&
                                        x.Bast.ApprovalOneStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalTwoStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalThreeStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalFourStatus != BastApproveStatus.Reject
                                        )
                                        .Count();

                                    if (listAsg == 0)
                                    {
                                        topFix = "70%";
                                        final = true;
                                    }
                                    else
                                    {
                                        cekAsgNotValid = cekAsgNotValid + 1;
                                    }
                                }

                            }
                            else if (top == "50% - 50%" || top == "50%-50%")
                            {
                                //cek top pertama
                                var topp = "30% - 70%";
                                listAsg = _mappingAsgBast.GetAllQ().Where(x => x.IdAsg.ToString() == id && x.Bast.TOP == topp.Substring(0, 3) &&
                                x.Bast.BastFinal == false &&
                                x.Bast.ApprovalOneStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalTwoStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalThreeStatus != BastApproveStatus.Reject &&
                                x.Bast.ApprovalFourStatus != BastApproveStatus.Reject
                                )
                                .Count();

                                if (listAsg == 0)
                                {
                                    topFix = "50%";
                                }
                                else //cek top kedua
                                {
                                    listAsg = _mappingAsgBast.GetAllQ().Where(x => x.IdAsg.ToString() == id && x.Bast.TOP == topp.Substring(6, 3) &&
                                        x.Bast.BastFinal == true &&
                                        x.Bast.ApprovalOneStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalTwoStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalThreeStatus != BastApproveStatus.Reject &&
                                        x.Bast.ApprovalFourStatus != BastApproveStatus.Reject
                                        )
                                        .Count();

                                    if (listAsg == 0)
                                    {
                                        topFix = "50%";
                                        final = true;
                                    }
                                    else
                                    {
                                        cekAsgNotValid = cekAsgNotValid + 1;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            error = e.ToString();
                        }


                        if (topFix == "100%")
                        {
                            topCalculation = 1.00M;
                        }
                        else if (topFix == "30%")
                        {
                            topCalculation = 0.30M;
                        }
                        else if (topFix == "70%")
                        {
                            topCalculation = 0.70M;
                        }
                        else if (topFix == "50%")
                        {
                            topCalculation = 0.50M;
                        }
                        valueBast = valueBast + Math.Ceiling(getValue(id));
                    }

                    Guid idid;
                    if (cekValidPo == true && cekAsgNotValid == 0)
                    {
                        Bast bast = new Bast
                        {
                            ApprovalOneStatus = BastApproveStatus.Waiting,
                            ApprovalTwoStatus = BastApproveStatus.Waiting,
                            ApprovalThreeStatus = BastApproveStatus.Waiting,
                            ApprovalFourStatus = BastApproveStatus.Waiting,
                            RequestById = _userHelper.GetUser(User).UserProfile.Id,
                            TOP = topFix,
                            totalValue = (valueBast * topCalculation),
                            AspId = _userHelper.GetUser(User).UserProfile.ASPId,
                            CreatedBy = _userHelper.GetUser(User).UserProfile.Name,
                            Sow = sow,
                            Project = project,
                            BastReqNo = GenerateNumberBastReqNo(),
                            BastFinal = final,
                            OtherInfo = account
                        };
                        _bast.Add(bast);
                        idid = bast.Id;
                        foreach (var id in AsgId)
                        {
                            try
                            {
                                MapAsgBast map = new MapAsgBast
                                {
                                    IdBast = bast.Id,
                                    IdAsg = Guid.Parse(id),
                                    CreatedBy = _userHelper.GetUser(User).UserProfile.Name,
                                    StatusApi = "NOK"
                                };
                                _mappingAsgBast.Add(map);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                //error = e.ToString();
                            }

                        }
                        TempData["Messages"] = "OK";

                        //return RedirectToAction("BastEditor", new { Id = idid });
                        return RedirectToAction("Edit", "Bast", new { id = idid });

                    }
                    else
                    {
                        if (cekValidPo == false)
                        {
                            TempData["Messages"] = "PO not valid!";
                        }
                        else if (cekAsgNotValid > 0)
                        {
                            TempData["Messages"] = "Assignment already Submited!";
                        }
                    }
                }
            }

            return RedirectToAction("Index");
            //return NotFound();
        }

        public string GenerateNumberBastReqNo()
        {
            var item = _bast.GetAll().Where(x => !string.IsNullOrEmpty(x.BastReqNo) && x.CreatedAt.HasValue && x.CreatedAt.Value.Date <= DateTime.Now.Date.AddYears(1)).OrderByDescending(x => x.BastReqNo).FirstOrDefault();
            string result = "000001";
            int digit = 6;
            if (item == null)
            {
                return result;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.BastReqNo))
                {
                    if (item.BastReqNo.Length > digit)
                    {
                        int Temp = int.Parse(item.BastReqNo) + 1;
                        return Temp.ToString();
                    }
                    else
                    {
                        string Current = int.Parse(item.BastReqNo).ToString();
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
            //string result = "000001";
            ////int digit = 6;
            //if (item == null)
            //{
            //    return result;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(item.BastReqNo))
            //    {
            //        int Temp = int.Parse(item.BastReqNo) + 1;
            //        return Temp.ToString();
            //    }
            //    else
            //    {
            //        return result;
            //    }
            //}
        }

        private decimal getValue(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.ValueAssignment;

        }

        private string getTOP(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.TOP;

        }
        private string getProject(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.ProjectName;

        }
        private string getSow(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.Sow;

        }
        private string getAccount(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.ShortTextPO;

        }
        private string getNoPO(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.PONumber;

        }

        private string getAsgId(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            return item.AssignmentId;

        }

        public async Task<IActionResult> asgDPM(string poNumber)
        {
            try
            {
                var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
                var filter = "\"PO_Number\"";

                var api = _webset.GetAll().Where(x => x.Name == "apiGet").FirstOrDefault();

                //Api prod xl
                //var urll = "https://api.xl.pdb.e-dpm.com/xlpdbapi/prpo_assignment?where={" + filter + ":" + (char)34+ poNumber + (char)34 + "}";

                //api dev isat
                //var urll = "https://api-dev.isat.pdb.e-dpm.com/isatapi/prpo_assignment?where={" + filter + ":" + (char)34 + poNumber + (char)34 + "}";

                //api isat prod
                var urll = api.Value + filter + ":" + (char)34 + poNumber + (char)34 + "}";

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(urll);

                WebReq.Method = "GET";
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(
                    new Uri(urll),
                    "Basic",
                    new NetworkCredential("userebastapi", "Pm1RQ6edV4IZ")
                );

                WebReq.Credentials = credentialCache;

                HttpWebResponse WebResp = (HttpWebResponse)await WebReq.GetResponseAsync();

                Console.WriteLine(WebResp.StatusCode);
                //Console.WriteLine(WebResp.Server);

                string jsonString;
                using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
                {
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    jsonString = reader.ReadToEnd();
                }
                AssignmentDPM items = JsonConvert.DeserializeObject<AssignmentDPM>(jsonString);
                //List<AssignmentDPM> items = JsonConvert.DeserializeObject<List<AssignmentDPM>>(jsonString);
                int TOTAL_INSERT = 0;
                int TOTAL_UPDATE = 0;
                foreach (var a in items._items)
                {
                    //Console.WriteLine(a.Assignment_No);
                    ASP aspFind = _asp.GetAll().FirstOrDefault(x => x.OtherInfo == a.Vendor_Code_Actual);




                    Assignment AssignmentFind = Service
                       .GetAll()
                       .FirstOrDefault(x => x.AssignmentId == a.Assignment_No);


                    if (AssignmentFind == null)
                    {

                        Assignment nt = new Assignment
                        {
                            AssignmentId = a.Assignment_No,
                            ProjectName = a.Project,
                            SiteID = a.Site_ID,
                            SiteName = a.Site_Name,
                            AssignmentAcceptedDate = DateTime.Parse(a.ASP_Acceptance_Date),
                            PRNo = a.PR_for_ASP,
                            PRDateCreated = DateTime.Parse(a.PR_For_ASP_Date),
                            PONumber = a.PO_Number,
                            PODate = DateTime.Parse(a.PO_Date),
                            LineItemPO = a.PO_Item,
                            AssignmentReady4Bast = true,
                            ShortTextPO = a.Account_Name,
                            SHID = a.WP_ID,
                            ValueAssignment = a.Total_Service_Price,
                            TOP = a.Payment_Terms,
                            AssignmentCreateBy = a.Requisitioner,
                            AssignmentCreateDate = DateTime.Parse(a.created_on),
                            AspId = aspFind.Id,
                            AssignmentCancel = a.PR_PO_Cancelation,
                            Sow = a.SOW_Type,
                            idDPM = a._id,
                            id_project_doc = a.id_project_doc
                        };
                        Service.Add(nt);
                        //TempData["Messages"] = "Success add Assignment Id " + assigmentId;
                        Console.WriteLine("Success add Assignment Id " + a.Assignment_No);
                        TOTAL_INSERT++;
                    }
                    else if (AssignmentFind != null)
                    {

                        Assignment nt = Service.GetById(AssignmentFind.Id);
                        nt.ProjectName = a.Project;
                        nt.SiteID = a.Site_ID;
                        nt.SiteName = a.Site_Name;
                        nt.AssignmentAcceptedDate = DateTime.Parse(a.ASP_Acceptance_Date);
                        nt.PRNo = a.PR_for_ASP;
                        nt.PRDateCreated = DateTime.Parse(a.PR_For_ASP_Date);
                        nt.PONumber = a.PO_Number;
                        nt.PODate = DateTime.Parse(a.PO_Date);
                        nt.LineItemPO = a.PO_Item;
                        nt.ShortTextPO = a.Account_Name;
                        nt.SHID = a.WP_ID;
                        nt.ValueAssignment = a.Total_Service_Price;
                        nt.TOP = a.Payment_Terms;
                        nt.AssignmentCreateBy = a.Requisitioner;
                        nt.AssignmentCreateDate = DateTime.Parse(a.created_on);
                        nt.AspId = AssignmentFind.AspId;
                        nt.AssignmentCancel = a.PR_PO_Cancelation;
                        nt.AssignmentReady4Bast = true;
                        nt.Sow = a.SOW_Type;
                        nt.idDPM = a._id;
                        nt.id_project_doc = a.id_project_doc;
                        //nt.OtherInfo = shortextPo;
                        Service.Update(nt);
                        //TempData["Messages"] = "Success update Assignment Id " + assigmentId;
                        Console.WriteLine("Success update Assignment Id " + a.Assignment_No);
                        TOTAL_UPDATE++;
                    }

                }
                TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT + " , Total Updated = " + TOTAL_UPDATE;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["Messages"] = ex.ToString();
            }
            return RedirectToAction("Index");


            //var response = JsonConvert.DeserializeObject(jsonString);


            //Console.WriteLine(items.Count);






            //##################################################################################################
            //if (User.IsInRole("ASP Admin"))
            //{
            //    response = Service.GetDataTablesResponse<AssignmentDto>(request, Mapper, $"AspId.toString().Equals(\"{ASP.ToString()}\") && AssignmentCancel == (\"{false}\")", Includes);
            //}

            //return Ok(response);
            //return new DataTablesJsonResult(response, true);
        }
    }
}
