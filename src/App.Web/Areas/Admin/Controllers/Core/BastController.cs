using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Globalization;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;
using System.Net.Http;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, ASP Admin, IM, PA, CPM, ASP PM")]
    public class BastController : BaseController<Bast, IBastService, BastViewModel, BastFormModel, Guid>
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
        private readonly IBastService _service;
        private readonly ExcelHelper _excel;
        private readonly IASPService _asp;
        private readonly IMapAsgBastService _mappingAsgBast;
        private readonly IAssignmentService _asg;
        private readonly IConverter _converter;
        private readonly IWebSettingService _webset;
        //private readonly IBastService _bast;
        private int sendCount = 0;
        private int asgBast = 0;


        public BastController(IHttpContextAccessor httpContextAccessor,
            IUserService userService, IMapper mapper,
            IBastService service,
            IUserProfileService profileUser,
            IUserProfileService user,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            NotifHelper notif,
            FileHelper file,
            IHostingEnvironment env,
            ExcelHelper excel,
            IASPService asp,
            IWebSettingService webset,
            IConverter converter,
            IMapAsgBastService mappingAsgBast,
            IAssignmentService asg,
            //IBastService bast,
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
            //_bast = bast;
            _mappingAsgBast = mappingAsgBast;
            _converter = converter;
            _asg = asg;
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
        [Authorize(Roles = "Administrator,ASP Admin, IM, PA, CPM, ASP PM")]
        public override IActionResult Index()
        {
            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            ViewBag.UserRole = UserRole.FirstOrDefault();
            return base.Index();
        }

        [HttpGet]
        public override IActionResult Edit(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                ViewBag.ASPPM = _userHelper.GetByRoleName("ASP PM").ToList();
                ViewBag.CPM = _userHelper.GetByRoleName("CPM").ToList();
                ViewBag.PA = _userHelper.GetByRoleName("PA").ToList();
                ViewBag.IM = _userHelper.GetByRoleName("IM").ToList();
                ViewBag.Id = id;

                return base.BastEditor(id);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        public IActionResult Pending()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            ViewBag.PreofileId = PreofileId;
            return View();
        }

        [HttpGet]
        public override IActionResult Details(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                ViewBag.ASPPM = _userHelper.GetByRoleName("ASP PM").ToList();
                ViewBag.CPM = _userHelper.GetByRoleName("CPM").ToList();
                ViewBag.PA = _userHelper.GetByRoleName("PA").ToList();
                ViewBag.IM = _userHelper.GetByRoleName("IM").ToList();
                ViewBag.Id = id;
                var data = _service.GetById(id);
                if (data == null) return NotFound();
                if (!string.IsNullOrEmpty(data.Files))
                {
                    ViewBag.Files = JsonConvert.DeserializeObject<List<string>>(data.Files);
                }
                else
                {
                    ViewBag.Files = null;
                }


                return base.BastEditor(id);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }
        private Guid getAsg(Guid id)
        {
            var item = _mappingAsgBast.GetAll().Where(x => x.IdBast == id).Select(x => x.IdAsg).ToList();
            return item.FirstOrDefault();

        }
        private string getUser(int id)
        {
            var item = _user.GetById(id);
            return item.Name;

        }

        private string getAspName(Guid id)
        {
            var item = _asp.GetById(id);
            return item.Name;

        }

        public IActionResult print(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;

                var data = _service.GetById(id);
                var vendor = "\"Vendor\"";
                var eid = "\"Ericsson\"";
                var style = "\"a\"";
                var styleLeft = "\"text-align:left\"";
                var styleCenter = "\"text-align:center\"";
                var styleWidth = "\"width:100%\"";
                var poNumber = _asg.GetById(getAsg(id));
                var pmName = getUser(item.ApprovalOneID);
                var tpmName = getUser(item.ApprovalFourID);
                var aspName = getAspName(item.AspId);

                //ini buat samain value halaman 1 dan 2
                var Result = _mappingAsgBast.GetAll().Where(x => x.IdBast == id).ToList();
                var counter = 1;
                var totalBAST = new Decimal();
                var accBAST = new Decimal();
                foreach (var roww in Result)
                {
                    var itemm = _asg.GetById(roww.IdAsg);


                    if (item.TOP == "30%")
                    {
                        accBAST = Math.Round(itemm.ValueAssignment * 0.3M);
                        //Math.Round(accBAST);
                    }
                    else if (item.TOP == "70%")
                    {
                        accBAST = itemm.ValueAssignment - (Math.Round(itemm.ValueAssignment * 0.3M));
                    }
                    else if (item.TOP == "100%")
                    {
                        accBAST = itemm.ValueAssignment * 1;
                    }
                    else if (item.TOP == "50%")
                    {
                        if (item.BastFinal == false)
                        {
                            accBAST = itemm.ValueAssignment * 0.5M;
                        }
                        else
                        {
                            accBAST = itemm.ValueAssignment - (Math.Round(itemm.ValueAssignment * 0.5M));
                        }

                    }
                    counter = counter + 1;
                    totalBAST = totalBAST + accBAST;
                }





                if (data == null)
                {
                    return NotFound();
                }
                else
                {
                    if (item.TOP == "100%" || item.BastFinal == true)
                    {
                        var head = $"<center><h3>HAND OVER CERTIFICATE<br>(BERITA ACARA SERAH TERIMA)</h3></center><br><p>Works: " + item.Sow + "<br>Project: " + item.Project + "</p><hr>" +
                            $"<center><p>BAST No: EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + "</p></center>" +
                            $"<br><p>On the date, ____________ we the undersigned: <br><ol><li>Name&emsp;&emsp;: " + pmName + "<br>Title&emsp;&emsp;&ensp;: PROJECT MANAGER<br>" +
                            $"On the matter acting for and behalf of " + aspName + " (hereinafter " + vendor + ") and:</li><br><li>Name&emsp;&emsp;: " + tpmName + "<br>Title&emsp;&emsp;&ensp;: TOTAL PROJECT MANAGER<br>" +
                            $"On the matter acting for and behalf of PT.ERICSSON INDONESIA (hereinafter referred to as " + eid + ")</li></ol>By virtue of: <br>PO Number: " + poNumber.PONumber + "/" + poNumber.PODate.ToString("dd-MM-yyyy") + "<br>" +
                            $"Vendor and Ericsson hereby stated the followings: <ol><li>Vendor has transferred the works and the title thereof to Ericsson at the Location in accordance with Purchase Order referred to above :<br></li><br>" +
                            $"<li>Ericsson has accepted the works and the title thereof satisfactorily, provided that :<br> <ol type=" + style + ">  <li>Completion of the works, (Number of days delay: " + item.TotalDelay + ")</li>" +
                            $"<li>Warranty period for the works shall apply for the period agreed within the Supply Agreement referred above. Any insufficiency or defect to the works encountered during such period due to workmanship or quality thereof shall become the responsibility of Vendor to rectify replace such insufficiency or defect.</li>" +
                            $"<li>Upon the expiry of such warranty period and the works has been functioning properly in accordance with the condition of the above referred Supply Agreement and/or Purchase Order, The Second Hand Over Certificate (or Berita Acara Serah Terima Kedua or " + "\"BAST\"" + ") will be issued accordingly by Ericsson.</li>" +
                            $"<li>There is/ There is no additional/ subtraction of works as basis of Amendment of the Purchase Order.</li></ol> </li><li>Total project cost (actual implemented/PO value revised): IDR " + Convert.ToInt32(totalBAST).ToString("N1", CultureInfo.InvariantCulture) + "(see attachment).</li>" +
                            $"</ol>This certificate is made in one original bearing sufficient stamp duties which shall have the same legal powers after being signed by their respective duly representatives.<br><br>&emsp;&emsp;" +

                            $"<table style=" + styleWidth + ">" +
                              $"<tr>" +
                                $"<th style=" + styleLeft + "> PT ERICSSON INDONESIA</th>" +
                                $"<th></th> " +
                                $"<th>" + aspName + "</th>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>Approved By " + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "><small> Approved By " + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>" + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + ">" + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>Total Project Manager</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "> Project Manager</td>" +
                              $"</tr>" +
                             $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>This is a computer-generated document. No signature is required.</td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                            $"</table>";

                        byte[] pdf;
                        HtmlToPdfDocument doc;
                        doc = new HtmlToPdfDocument()
                        {
                            GlobalSettings = {
                                    PaperSize = PaperKind.A4,
                                    Orientation = Orientation.Portrait,
                                },

                            Objects = {
                                    new ObjectSettings(){
                                        HtmlContent = head,
                                        },
                                    }
                        };


                        pdf = _converter.Convert(doc);


                        return new FileContentResult(pdf, "application/pdf");
                    }
                    else
                    {
                        var head = $"<center><h3>DOWN PAYMENT ACKNOWLEDGEMENT CERTIFICATE<br></h3></center><br><p>Works: " + item.Sow + "<br>Project: " + item.Project + "</p><hr>" +
                            $"<center><p>BAST No: EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + "</p></center>" +
                            $"<br><p>On the date, ____________ we the undersigned: <br><ol><li>Name&emsp;&emsp;: " + pmName + "<br>Title&emsp;&emsp;&ensp;: PROJECT MANAGER<br>" +
                            $"On the matter acting for and behalf of " + aspName + " (hereinafter " + vendor + ") and:</li><br><li>Name&emsp;&emsp;: " + tpmName + "<br>Title&emsp;&emsp;&ensp;: TOTAL PROJECT MANAGER<br>" +
                            $"On the matter acting for and behalf of PT.ERICSSON INDONESIA (hereinafter referred to as " + eid + ")</li></ol>By virtue of: <br>PO Number: " + poNumber.PONumber + "/" + poNumber.PODate.ToString("dd-MM-yyyy") + "<br>" +
                            $"Vendor and Ericsson hereby stated the followings: <ol><li>Attached to this Down Payment Certificate, are all required supporting documents for this project as agreed with Ericsson Project Manager, for example  (but not limited to): Purchase Order (PO)</li><br>" +
                            $"<li>Total project cost (actual implemented/PO value revised): IDR " + Convert.ToInt32(totalBAST).ToString("N1", CultureInfo.InvariantCulture) + "(see attachment).</li>" +
                            $"</ol>This certificate is made in one original bearing sufficient stamp duties which shall have the same legal powers after being signed by their respective duly representatives.<br><br>&emsp;&emsp;" +

                            $"<table style=" + styleWidth + ">" +
                              $"<tr>" +
                                $"<th style=" + styleLeft + "> PT ERICSSON INDONESIA</th>" +
                                $"<th></th> " +
                                $"<th>" + aspName + "</th>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>Approved By " + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "><small> Approved By " + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>" + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + ">" + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>Total Project Manager</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "> Project Manager</td>" +
                              $"</tr>" +
                             $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>This is a computer-generated document. No signature is required.</td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                            $"</table>";

                        //$"PT Ericsson Indonesia&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + aspName + "<br><br>&emsp;&emsp;<small>Approved by " + tpmName + "</small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small>Approved by " + pmName + "</small><br>&emsp;&emsp;<small></small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small></small><br><br><br>" +
                        //    $"&emsp;&emsp;" + tpmName + "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + pmName + "<br>&emsp;&emsp;TOTAL PROJECT MANAGER&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;PROJECT MANAGER<br><br><br><small>This is a computer-generated document. No signature is required.</small> </p>";



                        byte[] pdf;
                        HtmlToPdfDocument doc;
                        doc = new HtmlToPdfDocument()
                        {
                            GlobalSettings = {
                                    PaperSize = PaperKind.A4,
                                    Orientation = Orientation.Portrait,
                                },

                            Objects = {
                                    new ObjectSettings(){
                                        HtmlContent = head,
                                        },
                                    }
                        };


                        pdf = _converter.Convert(doc);


                        return new FileContentResult(pdf, "application/pdf");
                    }
                }



                return base.BastEditor(id);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }


        public IActionResult print2(Guid id)
        {
            try
            {
                var item = Service.GetById(id);
                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                var styleLeft = "\"text-align:left\"";
                var styleCenter = "\"text-align:center\"";
                var styleWidth = "\"width:100%\"";
                var data = _service.GetById(id);
                //var vendor = "\"Vendor\"";
                //var eid = "\"Ericsson\"";
                //var style = "\"a\"";
                var styletable = "\"width:100%;border-collapse:collapse;border:ridge\"";
                var stylee = "\"border-collapse:collapse;border:ridge\"";
                var poNumber = _asg.GetById(getAsg(id));
                var pmName = getUser(item.ApprovalOneID);
                var tpmName = getUser(item.ApprovalFourID);
                var aspName = getAspName(item.AspId);

                var header1 = "<th style=" + stylee + ">No</th>";
                var header2 = "<th style=" + stylee + ">Asg Id</th>";
                var header3 = "<th style=" + stylee + ">Site Name</th>";
                var header4 = "<th style=" + stylee + ">PO #</th>";
                var header5 = "<th style=" + stylee + ">Line Item</th>";
                var header6 = "<th style=" + stylee + ">Value As per PO</th>";
                var header7 = "<th style=" + stylee + ">TOP</th>";
                var header8 = "<th style=" + stylee + ">Acceptance Term</th>";
                var header9 = "<th style=" + stylee + ">Value</th>";
                var headers = $"<tr style=" + stylee + ">" + header1 + header2 + header3 + header4 + header5 + header6 + header7 + header8 + header9 + "</tr>";
                var rows = new StringBuilder();

                var Result = _mappingAsgBast.GetAll().Where(x => x.IdBast == id).ToList();
                var counter = 1;
                var totalBAST = new Decimal();
                var accBAST = new Decimal();
                foreach (var roww in Result)
                {
                    var itemm = _asg.GetById(roww.IdAsg);

                    var column1 = $"<td style=" + stylee + ">" + counter + "</td>";
                    var column2 = $"<td style=" + stylee + ">" + itemm.AssignmentId + "</td>";
                    var column3 = $"<td style=" + stylee + ">" + itemm.SiteName + "</td>";
                    var column4 = $"<td style=" + stylee + ">" + itemm.PONumber + "</td>";
                    var column5 = $"<td style=" + stylee + ">" + itemm.LineItemPO + "</td>";
                    var column6 = $"<td style=" + stylee + ">" + Convert.ToInt32(itemm.ValueAssignment).ToString("N1", CultureInfo.InvariantCulture) + "</td>";
                    var column7 = $"<td style=" + stylee + ">" + itemm.TOP + "</td>";
                    var column8 = $"<td style=" + stylee + ">" + item.TOP + "</td>";
                    var column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(itemm.ValueAssignment * 1).ToString("N1", CultureInfo.InvariantCulture) + "</td>";
                    if (item.TOP == "30%")
                    {
                        accBAST = Math.Round(itemm.ValueAssignment * 0.3M);
                        column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(accBAST).ToString("N1", CultureInfo.InvariantCulture) + "</td>";

                    }
                    else if (item.TOP == "70%")
                    {
                        accBAST = itemm.ValueAssignment - (Math.Round(itemm.ValueAssignment * 0.3M));
                        column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(accBAST).ToString("N1", CultureInfo.InvariantCulture) + "</td>";

                    }
                    else if (item.TOP == "100%")
                    {
                        column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(itemm.ValueAssignment * 1).ToString("N1", CultureInfo.InvariantCulture) + "</td>";
                        accBAST = itemm.ValueAssignment * 1;
                    }
                    else if (item.TOP == "50%")
                    {
                        if (item.BastFinal == false)
                        {
                            accBAST = Math.Round(itemm.ValueAssignment * 0.5M);
                            column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(accBAST).ToString("N1", CultureInfo.InvariantCulture) + "</td>";

                        }
                        else
                        {
                            accBAST = itemm.ValueAssignment - (Math.Round(itemm.ValueAssignment * 0.5M));
                            column9 = $"<td style=" + stylee + ">" + Convert.ToInt32(accBAST).ToString("N1", CultureInfo.InvariantCulture) + "</td>";

                        }

                    }


                    var row = $"<tr style=" + stylee + ">" + column1 + column2 + column3 + column4 + column5 + column6 + column7 + column8 + column9 + "</tr>";
                    rows.Append(row);
                    counter = counter + 1;
                    totalBAST = totalBAST + accBAST;
                }

                var rowTotal = $"</tr><tr style=" + stylee + ">" +
                        $"<td style= " + stylee + " ></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + "></td>" +
                        $"<td style=" + stylee + ">TOTAL</td>" +
                        $"<td style=" + stylee + ">" + Convert.ToInt32(totalBAST).ToString("N1", CultureInfo.InvariantCulture) + "</td>" +
                    $"</tr>";

                var table = $"<table style=" + styletable + ">" + headers + rows + rowTotal + "</table>";

                if (data == null)
                {
                    return NotFound();
                }
                else
                {
                    if (item.TOP == "100%" || item.BastFinal == true)
                    {

                        var head = $"<center><h3>HAND OVER CERTIFICATE<br></h3></center><br><p>Works: " + item.Sow + "<br>Project: " + item.Project + "</p><hr>" +
                           $"<center><p>BAST No: EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + "</p></center>" + table +


                           $"<br><br>&emsp;&emsp;" +

                           $"<table style=" + styleWidth + ">" +
                              $"<tr>" +
                                $"<th style=" + styleLeft + "> PT ERICSSON INDONESIA</th>" +
                                $"<th></th> " +
                                $"<th>" + aspName + "</th>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>Approved By " + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "><small> Approved By " + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>" + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + ">" + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>Total Project Manager</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "> Project Manager</td>" +
                              $"</tr>" +
                             $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>This is a computer-generated document. No signature is required.</td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                            $"</table>";

                        //$"PT Ericsson Indonesia&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + aspName + "<br><br>&emsp;&emsp;" +
                        //   $"<small>Approved by " + tpmName + "</small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small>Approved by " + pmName + "</small><br>&emsp;&emsp;<small>" +
                        //   "</small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small></small><br><br><br>" +
                        //   $"&emsp;&emsp;" + tpmName + "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + pmName + "<br>&emsp;&emsp;" +
                        //   $"TOTAL PROJECT MANAGER&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;PROJECT MANAGER<br><br><br>This is a computer-generated document. No signature is required. </p>";





                        byte[] pdf;
                        HtmlToPdfDocument doc;
                        doc = new HtmlToPdfDocument()
                        {
                            GlobalSettings = {
                                    PaperSize = PaperKind.A4,
                                    Orientation = Orientation.Landscape,
                                },

                            Objects = {
                                    new ObjectSettings(){
                                        HtmlContent = head,
                                        },
                                    }
                        };


                        pdf = _converter.Convert(doc);


                        return new FileContentResult(pdf, "application/pdf");
                    }
                    else
                    {
                        var head = $"<center><h3>DOWN PAYMENT ACKNOWLEDGEMENT CERTIFICATE<br></h3></center><br><p>Works: " + item.Sow + "<br>Project: " + item.Project + "</p><hr>" +
                            $"<center><p>BAST No: EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + "</p></center>" + table +


                            $"<br><br>&emsp;&emsp;" +

                            $"<table style=" + styleWidth + ">" +
                              $"<tr>" +
                                $"<th style=" + styleLeft + "> PT ERICSSON INDONESIA</th>" +
                                $"<th></th> " +
                                $"<th>" + aspName + "</th>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>Approved By " + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "><small> Approved By " + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>" + tpmName + "</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + ">" + pmName + "</td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td>Total Project Manager</td>" +
                                $"<td></td>" +
                                $"<td style=" + styleCenter + "> Project Manager</td>" +
                              $"</tr>" +
                             $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td></td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                              $"<tr>" +
                                $"<td><small>This is a computer-generated document. No signature is required.</td>" +
                                $"<td></td>" +
                                $"<td></td>" +
                              $"</tr>" +
                            $"</table>";

                        //$"PT Ericsson Indonesia&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + aspName + "<br><br>&emsp;&emsp;"+
                        //    $"<small>Approved by " + tpmName + "</small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small>Approved by " + pmName + "</small><br>&emsp;&emsp;<small>" + 
                        //    "</small>&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;<small></small><br><br><br>" +
                        //    $"&emsp;&emsp;" + tpmName + "&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;" + pmName + "<br>&emsp;&emsp;" +
                        //    $"TOTAL PROJECT MANAGER&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;PROJECT MANAGER<br><br><br>This is a computer-generated document. No signature is required. </p>";



                        byte[] pdf;
                        HtmlToPdfDocument doc;
                        doc = new HtmlToPdfDocument()
                        {
                            GlobalSettings = {
                                    PaperSize = PaperKind.A4,
                                    Orientation = Orientation.Landscape,
                                },

                            Objects = {
                                    new ObjectSettings(){
                                        HtmlContent = head,
                                        },
                                    }
                        };


                        pdf = _converter.Convert(doc);


                        return new FileContentResult(pdf, "application/pdf");
                    }
                }



                return base.BastEditor(id);
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }
        public async void resendDPM(Guid id)
        {
            var item = _service.GetById(id);
            sendCount = 0;
            try
            {

                //var Result = _mappingAsgBast.GetAll().Where(x => x.IdBast == id).ToList();

                //metode baru
                Expression<Func<MapAsgBast, object>>[] Includes = new Expression<Func<MapAsgBast, object>>[2];
                Includes[0] = pack => pack.Assignment;
                Includes[1] = pack => pack.Bast;

                var Result = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.Id == id).ToList();

                foreach (var roww in Result)
                {
                    //var itemm = _asg.GetAll().Where(x => x.Id == roww.IdAsg).FirstOrDefault();
                    var bastFinal = "";
                    if (item.BastFinal == true)
                    {
                        bastFinal = "Final";
                    }
                    else
                    {
                        bastFinal = "DP";
                    }
                    var GRPercent = "";
                    if (item.TOP == "30%")
                    {
                        GRPercent = "0.3";
                    }
                    else if (item.TOP == "70%")
                    {
                        GRPercent = "0.7";
                    }
                    else if (item.TOP == "50%")
                    {
                        GRPercent = "0.5";
                    }
                    else if (item.TOP == "100%")
                    {
                        GRPercent = "1";
                    }
                    //var account = _webset.GetAll().Where(x => x.Name == "AccountApi").FirstOrDefault();
                    var urll = "https://api2.bam-id.e-dpm.com/bamidapi/aspAssignment/updateBastNumberIntegration/" + roww.Assignment.idDPM;
                    string json = "{\"account_id\" :\"ISAT\"," +
                                      "\"data\" :[" +
                                      "{\"Request_Type\":\"New GR\"," +
                                      "\"id_assignment_doc\":" + (char)34 + roww.Assignment.idDPM + (char)34 + "," +
                                      "\"Assignment_No\":" + (char)34 + roww.Assignment.AssignmentId + (char)34 + "," +
                                      "\"Account_Name\":" + (char)34 + roww.Assignment.ShortTextPO + (char)34 + "," +
                                      "\"ASP_Acceptance_Date\":" + (char)34 + roww.Assignment.AssignmentAcceptedDate.ToString("yyyy-MM-dd hh:mm:ss") + (char)34 + "," +
                                      "\"WP_ID\":" + (char)34 + roww.Assignment.SHID + (char)34 + "," +
                                      "\"id_project_doc\":" + (char)34 + roww.Assignment.id_project_doc + (char)34 + "," +
                                      "\"Project\":" + (char)34 + roww.Assignment.ProjectName + (char)34 + "," +
                                      "\"SOW_Type\":" + (char)34 + roww.Assignment.Sow + (char)34 + "," +
                                      "\"BAST_No\":" + (char)34 + "EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + (char)34 + "," +
                                      "\"GR_Type\":" + (char)34 + bastFinal + (char)34 + "," +
                                      "\"Payment_Terms\":" + (char)34 + roww.Assignment.TOP + (char)34 + "," +
                                      "\"Payment_Terms_Ratio\":" + (char)34 + item.TOP + (char)34 + "," +
                                      "\"GR_Percentage\":" + (char)34 + GRPercent + (char)34 + "," +
                                      "\"PO_Number\":" + (char)34 + roww.Assignment.PONumber + (char)34 + "," +
                                      "\"PO_Item\":" + (char)34 + roww.Assignment.LineItemPO + (char)34 + "," +
                                      "\"Item_Status\":" + (char)34 + "Submit" + (char)34 + "}" +
                                      "]," +
                                      "\"user\" :{" +
                                      "\"username\":" + (char)34 + "ebastxl" + (char)34 + "," +
                                      "\"email\":" + (char)34 + "ebast.xl@eidtools.tech" + (char)34 + "}," +
                                      "\"timeStamp\":" + (char)34 + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + (char)34 + "}"
                                      ;

                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpClient hc = new HttpClient();
                    hc.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                           $"{"userebastapi"}:{"Pm1RQ6edV4IZ"}")));

                    var method = new HttpMethod("PATCH");

                    var request = new HttpRequestMessage(method, urll)
                    {
                        Content = stringContent
                    };


                    HttpResponseMessage hrm = await hc.SendAsync(request);

                    Console.WriteLine(hrm.StatusCode);
                    //if(hrm.StatusCode.ToString() == "OK")
                    //{
                    //    sendCount = sendCount + 1;
                    //}
                    //else
                    //{
                    //    TempData["Messages"] = "Failed to send BAST No to DPM";
                    //}

                    //get status from API DPM
                    //if (hrm.StatusCode.ToString() == "OK" || itemm.idDPM == null)
                    //{
                    //    UpdateStatusApi(roww.Id, "OK");
                    //    //itemm.OtherInfo = "OK";
                    //    //_asg.Update(itemm);
                    //}
                    //else
                    //{
                    //    UpdateStatusApi(roww.Id, "NOK");
                    //    //itemm.OtherInfo = "NOK";
                    //    //_asg.Update(itemm);
                    //}
                }
                //asgBast = Result.Count;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["Messages"] = ex.ToString();
            }
            //return RedirectToAction("Pending");
        }

        public string UpdateStatusApi(Guid id, string status)
        {
            var ittem = _mappingAsgBast.GetById(id);
            ittem.StatusApi = status;
            _mappingAsgBast.Update(ittem);
            return "OK";
        }

        protected override void AfterCreateData(Bast item)
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

        protected override void UpdateData(Bast item, BastFormModel model)
        {

            item.ApprovalOneID = model.ApprovalOneID;
            item.ApprovalTwoID = model.ApprovalTwoID;
            item.ApprovalThreeID = model.ApprovalThreeID;
            item.ApprovalFourID = model.ApprovalFourID;
            item.TotalDelay = model.TotalDelay;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;

        }

        private async void patchToDPM(string id, string BASTNO, Bast item)
        {
            try
            {
                var Result = _mappingAsgBast.GetAll().Where(x => x.IdBast == Guid.Parse(id)).ToList();
                foreach (var roww in Result)
                {
                    var itemm = _asg.GetById(roww.IdAsg);
                    var bastFinal = "";
                    if (item.BastFinal == true)
                    {
                        bastFinal = "Final";
                    }
                    else
                    {
                        bastFinal = "DP";
                    }
                    var GRPercent = "";
                    if (item.TOP == "30%")
                    {
                        GRPercent = "0.3";
                    }
                    else if (item.TOP == "70%")
                    {
                        GRPercent = "0.7";
                    }
                    else if (item.TOP == "50%")
                    {
                        GRPercent = "0.5";
                    }
                    else if (item.TOP == "100%")
                    {
                        GRPercent = "1";
                    }
                    var urll = "https://api2.bam-id.e-dpm.com/bamidapi/aspAssignment/updateBastNumberIntegration/" + itemm.idDPM;
                    //var urll = "https://api2-dev.bam-id.e-dpm.com/bamidapi/aspAssignment/updateBastNumberIntegration/" + itemm.idDPM;
                    //var account = _webset.GetAll().Where(x => x.Name == )

                    string json = "{\"account_id\" :\"ISAT\"," +
                                      "\"data\" :[" +
                                      "{\"Request_Type\":\"New GR\"," +
                                      "\"id_assignment_doc\":" + (char)34 + itemm.idDPM + (char)34 + "," +
                                      "\"Assignment_No\":" + (char)34 + itemm.AssignmentId + (char)34 + "," +
                                      "\"Account_Name\":" + (char)34 + itemm.ShortTextPO + (char)34 + "," +
                                      "\"ASP_Acceptance_Date\":" + (char)34 + itemm.AssignmentAcceptedDate.ToString("yyyy-MM-dd hh:mm:ss") + (char)34 + "," +
                                      "\"WP_ID\":" + (char)34 + itemm.SHID + (char)34 + "," +
                                      "\"id_project_doc\":" + (char)34 + itemm.id_project_doc + (char)34 + "," +
                                      "\"Project\":" + (char)34 + itemm.ProjectName + (char)34 + "," +
                                      "\"SOW_Type\":" + (char)34 + itemm.Sow + (char)34 + "," +
                                      "\"BAST_No\":" + (char)34 + "EID/" + item.OtherInfo + "/" + item.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/: " + item.BastNo + (char)34 + "," +
                                      "\"GR_Type\":" + (char)34 + bastFinal + (char)34 + "," +
                                      "\"Payment_Terms\":" + (char)34 + itemm.TOP + (char)34 + "," +
                                      "\"Payment_Terms_Ratio\":" + (char)34 + item.TOP + (char)34 + "," +
                                      "\"GR_Percentage\":" + (char)34 + GRPercent + (char)34 + "," +
                                      "\"PO_Number\":" + (char)34 + itemm.PONumber + (char)34 + "," +
                                      "\"PO_Item\":" + (char)34 + itemm.LineItemPO + (char)34 + "," +
                                      "\"Item_Status\":" + (char)34 + "Submit" + (char)34 + "}" +
                                      "]," +
                                      "\"user\" :{" +
                                      "\"username\":" + (char)34 + "ebastxl" + (char)34 + "," +
                                      "\"email\":" + (char)34 + "ebast.xl@eidtools.tech" + (char)34 + "}," +
                                      "\"timeStamp\":" + (char)34 + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + (char)34 + "}"
                                      ;

                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpClient hc = new HttpClient();
                    hc.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                           $"{"userebastapi"}:{"Pm1RQ6edV4IZ"}")));

                    var method = new HttpMethod("PATCH");

                    var request = new HttpRequestMessage(method, urll)
                    {
                        Content = stringContent
                    };


                    HttpResponseMessage hrm = await hc.SendAsync(request);

                    Console.WriteLine(hrm.StatusCode);

                    //get status from API DPM
                    if (hrm.StatusCode.ToString() == "OK" || itemm.idDPM == null)
                    {
                        var ittem = _mappingAsgBast.GetById(roww.Id);
                        ittem.StatusApi = "OK";
                        _mappingAsgBast.Update(ittem);
                    }
                    else
                    {
                        var ittem = _mappingAsgBast.GetById(roww.Id);
                        ittem.StatusApi = "FAILED";
                        _mappingAsgBast.Update(ittem);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["Messages"] = ex.ToString();
            }
        }

        private async void MultiApprove(bool status, Bast item, string notes, string id)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            #region MultiUser
            if (status == true)
            {
                //var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                //int SendApproval = 0;
                //var Approver = UserApprover.LineManager;
                //SendApproval = item.ApprovalOneID.Value;

                #region ASP PM
                if (User.IsInRole("ASP PM") && item.ApprovalOneID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalOneStatus = BastApproveStatus.Approved;
                    item.ApprovalOneDate = DateTime.Now;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region PA
                if (User.IsInRole("PA") && item.ApprovalTwoID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalTwoStatus = BastApproveStatus.Approved;
                    item.ApprovalTwoDate = DateTime.Now;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion
                #region IM
                if (User.IsInRole("IM") && item.ApprovalThreeID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalThreeStatus = BastApproveStatus.Approved;
                    item.ApprovalThreeDate = DateTime.Now;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region CPM
                if (User.IsInRole("CPM") && item.ApprovalFourID == PreofileId)
                {
                    item.ApprovalFourStatus = BastApproveStatus.Approved;
                    item.ApprovalFourDate = DateTime.Now;
                    item.BastNo = GenerateNumnberBastNo();


                }
                #endregion
                Service.Update(item);
                if (item.BastNo != null)
                {
                    patchToDPM(id, item.BastNo, item);
                }



            }
            else
            {
                // Send Notif Rejected
                //item.Status = SrfStatus.Waiting;
                //item.ApprovalOneStatus = BastApproveStatus.Reject;
                //item.ApprovalTwoStatus = BastApproveStatus.Reject;
                //item.ApprovalThreeStatus = BastApproveStatus.Reject;
                //item.ApprovalFourStatus = BastApproveStatus.Reject;
                //item.RejectionReason = notes;
                //item.ApproveStatusFour = BastApproveStatus.Waiting;
                //item.ApproveStatusFive = BastApproveStatus.Waiting;
                //item.ApproveStatusSix = BastApproveStatus.Waiting;


                #region ASP PM
                if (User.IsInRole("ASP PM") && item.ApprovalOneID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalOneStatus = BastApproveStatus.Reject;
                    item.ApprovalOneDate = DateTime.Now;
                    item.RejectionReason = notes;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region PA
                if (User.IsInRole("PA") && item.ApprovalTwoID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalTwoStatus = BastApproveStatus.Reject;
                    item.ApprovalTwoDate = DateTime.Now;
                    item.RejectionReason = notes;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion
                #region IM
                if (User.IsInRole("IM") && item.ApprovalThreeID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalThreeStatus = BastApproveStatus.Reject;
                    item.ApprovalThreeDate = DateTime.Now;
                    item.RejectionReason = notes;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion

                #region CPM
                if (User.IsInRole("CPM") && item.ApprovalFourID == PreofileId)
                {
                    //item.NotesFirst = notes;
                    //item.Status = SrfStatus.Waiting;
                    item.ApprovalFourStatus = BastApproveStatus.Reject;
                    item.ApprovalFourDate = DateTime.Now;
                    //item.BastNo = GenerateNumnberBastNo();
                    item.RejectionReason = notes;
                    //SendApproval = item.ApproverThreeId.Value;


                    //Approver = UserApprover.HeadOfServiceLine;
                }
                #endregion


                Service.Update(item);


            }
            #endregion
        }

        public IActionResult sendDPM(Guid id)
        {
            resendDPM(id);
            //if (sendCount == asgBast)
            //{
            //    TempData["Messages"] = "Success to send BAST No to DPM for " + sendCount + " assignments.";
            //}
            //else
            //{
            //    TempData["Messages"] = "Success to send BAST No to DPM for " + sendCount + " assignments.";
            //}
            TempData["Messages"] = "BAST No already sent.Please check on DPM.";
            return RedirectToAction("Index");
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
                        ApproveGeneral(id, false, remarks);
                    }
                    TempData["Rejected"] = "OK";
                }
            }
            return RedirectToAction("Pending");
        }

        private void ApproveGeneral(string id, bool status, string notes = null)
        {
            var item = Service.GetById(Guid.Parse(id));
            //var Dept = _department.GetById(item.DepartmentId);
            if (item != null)
            {
                MultiApprove(status, item, notes, id);
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
                        ApproveGeneral(id, status, null);
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

        public string GenerateNumnberBastNo()
        {
            var item = Service.GetAll().Where(x => !string.IsNullOrEmpty(x.BastNo) && x.CreatedAt.HasValue && x.CreatedAt.Value.Date <= DateTime.Now.Date.AddYears(1)).OrderByDescending(x => x.BastNo).FirstOrDefault();
            string result = "000001";
            int digit = 6;
            if (item == null)
            {
                return result;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.BastNo))
                {
                    if (item.BastNo.Length > digit)
                    {
                        int Temp = int.Parse(item.BastNo) + 1;
                        return Temp.ToString();
                    }
                    else
                    {
                        string Current = int.Parse(item.BastNo).ToString();
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

        public IActionResult Export()
        {
            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            var Profile = _userHelper.GetLoginUser(User);
            var ProfileId = Profile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/Pending.xlsx";
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

                worksheet.Cells[index, i += 1].Value = "Bast Request No";
                worksheet.Cells[index, i += 1].Value = "Assignment ID";
                worksheet.Cells[index, i += 1].Value = "ASP";
                worksheet.Cells[index, i += 1].Value = "Bast No";
                worksheet.Cells[index, i += 1].Value = "Project";
                worksheet.Cells[index, i += 1].Value = "TOP";//ah id
                worksheet.Cells[index, i += 1].Value = "Assignment Value";
                worksheet.Cells[index, i += 1].Value = "ASP PM";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Approve/Reject Date";
                worksheet.Cells[index, i += 1].Value = "Project Admin";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Approve/Reject Date";
                worksheet.Cells[index, i += 1].Value = "CPM";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Approve/Reject Date";
                worksheet.Cells[index, i += 1].Value = "TPM";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Approve/Reject Date";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;
                Expression<Func<MapAsgBast, object>>[] Includes = new Expression<Func<MapAsgBast, object>>[7];
                Includes[0] = pack => pack.Assignment;
                Includes[1] = pack => pack.Bast;
                Includes[2] = pack => pack.Assignment.Asp;
                Includes[3] = pack => pack.Bast.ApprovalOne;
                Includes[4] = pack => pack.Bast.ApprovalTwo;
                Includes[5] = pack => pack.Bast.ApprovalThree;
                Includes[6] = pack => pack.Bast.ApprovalFour;


                var Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.ApprovalOneStatus == BastApproveStatus.Waiting ||
                        x.Bast.ApprovalTwoStatus == BastApproveStatus.Waiting ||
                        x.Bast.ApprovalThreeStatus == BastApproveStatus.Waiting ||
                        x.Bast.ApprovalFourStatus == BastApproveStatus.Waiting).OrderByDescending(x => x.Bast.BastReqNo).ToList();

                if (User.IsInRole("ASP Admin"))
                {
                    Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Assignment.AspId.ToString().Equals(ASP.ToString())).ToList();
                }

                if (User.IsInRole("ASP PM"))
                {
                    Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.ApprovalOneID == ProfileId && x.Bast.ApprovalOneStatus == BastApproveStatus.Waiting).ToList();

                }

                if (User.IsInRole("PA"))
                {
                    Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.ApprovalTwoID == ProfileId && x.Assignment.AssignmentCancel == false
                        && x.Bast.ApprovalTwoStatus == BastApproveStatus.Waiting && x.Bast.ApprovalOneStatus == BastApproveStatus.Approved).ToList();
                }

                if (User.IsInRole("IM"))
                {
                    Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.ApprovalThreeID == ProfileId && x.Assignment.AssignmentCancel == false
                        && x.Bast.ApprovalThreeStatus == BastApproveStatus.Waiting && x.Bast.ApprovalTwoStatus == BastApproveStatus.Approved).ToList();
                }

                if (User.IsInRole("CPM"))
                {
                    Data = _mappingAsgBast.GetAll(Includes).Where(x => x.Bast.ApprovalFourID == ProfileId && x.Assignment.AssignmentCancel == false
                        && x.Bast.ApprovalFourStatus == BastApproveStatus.Waiting && x.Bast.ApprovalThreeStatus == BastApproveStatus.Approved).ToList();
                }


                if (Data != null)
                {

                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.Bast.BastReqNo;
                        worksheet.Cells[index, j += 1].Value = row.Assignment.AssignmentId;
                        worksheet.Cells[index, j += 1].Value = row.Assignment.Asp.Name;
                        worksheet.Cells[index, j += 1].Value = row.Bast.BastNo;
                        worksheet.Cells[index, j += 1].Value = row.Bast.Project;
                        worksheet.Cells[index, j += 1].Value = row.Bast.TOP;
                        worksheet.Cells[index, j += 1].Value = row.Assignment.ValueAssignment;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalOne.Name;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalOneStatus;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalOneDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalTwo.Name;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalTwoStatus;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalTwoDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalThree.Name;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalThreeStatus;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalThreeDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalFour.Name;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalFourStatus;
                        worksheet.Cells[index, j += 1].Value = row.Bast.ApprovalFourDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

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
