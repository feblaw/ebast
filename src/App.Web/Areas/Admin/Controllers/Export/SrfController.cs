using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq.Expressions;
using App.Domain.Models.Enum;
using System.Globalization;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class SrfController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;

        public SrfController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment env,
            ISrfRequestService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
        }

        public override IActionResult Index()
        {
            var ProfileUser = _userHelper.GetLoginUser(User);
            int ProfileId = ProfileUser.Id;

            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/ServiceRequest.xlsx";
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
                worksheet.Cells[1, 1].Value = "SRF Number";
                worksheet.Cells[1, 2].Value = "Type Of Request";
                worksheet.Cells[1, 3].Value = "SIGNUM";
                worksheet.Cells[1, 4].Value = "Employee Name";
                worksheet.Cells[1, 5].Value = "Contract Begin";
                worksheet.Cells[1, 6].Value = "Contract End";
                worksheet.Cells[1, 7].Value = "Annual Leave";
                worksheet.Cells[1, 8].Value = "Notif";
                worksheet.Cells[1, 9].Value = "Position";
                worksheet.Cells[1, 10].Value = "Organization";
                worksheet.Cells[1, 11].Value = "Sub Organization";
                worksheet.Cells[1, 12].Value = "Escalation";
                worksheet.Cells[1, 13].Value = "Agency";

                worksheet.Cells[1, 14, 1, 16].Merge = true;
                worksheet.Cells[1, 14, 1, 16].Value = "Line Manager Approval";
                worksheet.Cells[1, 14, 1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[1, 17].Value = "Description	";
                worksheet.Cells[1, 18].Value = "Head Of Service Line";
                worksheet.Cells[1, 19].Value = "Head Of Operation";
                worksheet.Cells[1, 20].Value = "Head Of Non Operation";
                worksheet.Cells[1, 21].Value = "Head Of Sourcing";
              
                worksheet.Cells[1, 22, 1, 24].Merge = true;
                worksheet.Cells[1, 22, 1, 24].Value = "Service Coordinator Approval";
                worksheet.Cells[1, 22, 1, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 2].Value = "";
                worksheet.Cells[2, 3].Value = "";
                worksheet.Cells[2, 4].Value = "";
                worksheet.Cells[2, 5].Value = "";
                worksheet.Cells[2, 6].Value = "";
                worksheet.Cells[2, 7].Value = "";
                worksheet.Cells[2, 8].Value = "";
                worksheet.Cells[2, 9].Value = "";
                worksheet.Cells[2, 10].Value = "";
                worksheet.Cells[2, 11].Value = "";
                worksheet.Cells[2, 12].Value = "";
                worksheet.Cells[2, 13].Value = "";

                worksheet.Cells[2, 14].Value = "Status";
                worksheet.Cells[2, 15].Value = "Line Manager";
                worksheet.Cells[2, 16].Value = "Approve Date - Remark";

                worksheet.Cells[2, 17].Value = "";
                worksheet.Cells[2, 18].Value = "";
                worksheet.Cells[2, 19].Value = "";
                worksheet.Cells[2, 20].Value = "";
                worksheet.Cells[2, 21].Value = "";

                worksheet.Cells[2, 22].Value = "Status";
                worksheet.Cells[2, 23].Value = "Service Coordinator";
                worksheet.Cells[2, 24].Value = "Approve Date - Remark";

                for (int ii = 1; ii <= 2; ii++)
                {
                    for (int jj = 1; jj <= 23; jj++)
                    {
                        worksheet.Cells[ii, jj].Style.Font.Bold = true;
                        worksheet.Cells[ii, jj].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[ii, jj].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                        worksheet.Cells[ii, jj].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                }

                int index = 3;
                Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[11];
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

                var Data = Service.GetAll(Includes).Where(x => x.IsLocked == false).OrderBy(x=>x.SrfBegin).ToList();

                #region Filter Approval
                if (User.IsInRole("Line Manager"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.IsLocked == false && (x.ApproveOneId == ProfileId || x.ApproveTwoId == ProfileId || x.ApproveThreeId == ProfileId || x.ApproveFourId == ProfileId || x.ApproveFiveId == ProfileId || x.ApproveSixId == ProfileId)).OrderBy(x => x.SrfBegin).ToList();
                }
               
                if (User.IsInRole("HR Agency"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.IsLocked == false && x.Candidate.AgencyId == ProfileId).OrderBy(x => x.SrfBegin).ToList();
                }

                if (User.IsInRole("Sourcing"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.IsLocked == false && x.Candidate.Vacancy.ApproverTwoId == ProfileId).OrderBy(x => x.SrfBegin).ToList();
                }
                #endregion

                if(Data!=null)
                {
                    foreach(var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(row.Number) ? "e-EID/KI-" + row.CreatedAt.Value.ToString("yy", CultureInfo.InvariantCulture) + ":SRF: " + row.Number + " UEN" : "-";
                        #region TypeRequest
                        if (row.Status == Domain.Models.Enum.SrfStatus.Terminate || row.Status == Domain.Models.Enum.SrfStatus.Blacklist)
                        {
                            worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfStatus), row.Status).ToString();
                        }
                        else 
                        {
                            worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfType), row.Type).ToString();
                        }
                        # endregion
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Account.AhId;
                        worksheet.Cells[index, j += 1].Value = row.Candidate == null ? "-" : row.Candidate.Name;
                        worksheet.Cells[index, j += 1].Value = row.SrfBegin.HasValue ? row.SrfBegin.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "-";
                        worksheet.Cells[index, j += 1].Value = row.SrfEnd.HasValue ? row.SrfEnd.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture) : "-";
                        worksheet.Cells[index, j += 1].Value = row.AnnualLeave == 0 ? "Annual Leave is Expired" : row.AnnualLeave + " Days";
                        worksheet.Cells[index, j += 1].Value = row.IsEndSoon() == true ? "End Soon" : "-";
                        worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "-" : row.ServicePack.Name;
                        worksheet.Cells[index, j += 1].Value = row.Departement == null ? "-" : row.Departement.Name;
                        worksheet.Cells[index, j += 1].Value = row.DepartementSub == null ? "-" : row.DepartementSub.SubName;
                        worksheet.Cells[index, j += 1].Value = row.Escalation == null ? "Normal Rate" : "Special Rate";
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Agency == null ? "-" : row.Candidate.Agency.Name;

                        if (row.Escalation == null)
                        {
                            if(row.ApproveStatusOne == SrfApproveStatus.Approved)
                            {
                                worksheet.Cells[index, j += 1].Value = "Submitted";
                            }
                            else
                            {
                                worksheet.Cells[index, j += 1].Value = "Waiting Submitted";
                            }
                            
                        }
                        else
                        {
                            if(row.Escalation.Status == StatusEscalation.Submitted)
                            {
                                worksheet.Cells[index, j += 1].Value = "Submitted";
                            }
                            else
                            {
                                worksheet.Cells[index, j += 1].Value = "Waiting Submitted";
                            }
                        }

                        worksheet.Cells[index, j += 1].Value = row.ApproveOneBy == null ? "-" : row.ApproveOneBy.Name;
                        worksheet.Cells[index, j += 1].Value = row.DateApproveStatusOne.HasValue ? row.DateApproveStatusOne.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
                        worksheet.Cells[index, j += 1].Value = row.Description;
                       
                        if(row.Escalation == null)
                        {
                            if(row.Departement.OperateOrNon == 1)
                            {
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.ApproveStatusTwo).ToString();
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.ApproveStatusThree).ToString();
                                worksheet.Cells[index, j += 1].Value = "-";
                            }
                            else
                            {
                                worksheet.Cells[index, j += 1].Value = "-";
                                worksheet.Cells[index, j += 1].Value = "-";
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.ApproveStatusFour).ToString();
                            }
                            worksheet.Cells[index, j += 1].Value = "";
                            worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.ApproveStatusSix).ToString();
                          
                        }
                        else
                        {
                            if(row.Departement.OperateOrNon == 1)
                            {
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusOne).ToString();
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusTwo).ToString();
                                worksheet.Cells[index, j += 1].Value = "-";
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusThree).ToString();
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusFour).ToString();
                            }
                            else
                            {
                                worksheet.Cells[index, j += 1].Value = "-";
                                worksheet.Cells[index, j += 1].Value = "-";
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusTwo).ToString();
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusThree).ToString();
                                worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfApproveStatus), row.Escalation.ApproveStatusFour).ToString();
                            }
                           
                        }

                        worksheet.Cells[index, j += 1].Value = row.ApproveSixBy == null ? "-" : row.ApproveSixBy.Name;
                        worksheet.Cells[index, j += 1].Value = row.DateApproveStatusSix.HasValue ? row.DateApproveStatusSix.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
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
