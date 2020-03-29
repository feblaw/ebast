using App.Domain.Models.Core;
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
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Globalization;
using App.Domain.Models.Enum;
using System.Security.Claims;

namespace App.Web.Areas.Admin.Controllers.Export
{

    [Area("Export")]
    [Authorize]
    public class ReconcileController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {

        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;

        public ReconcileController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISrfRequestService service,
            IHostingEnvironment env,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
        }

        public  IActionResult DoExport(string month = null,string year = null)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/Reconcile.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "ID";
                worksheet.Cells[index, i += 1].Value = "Signum";
                worksheet.Cells[index, i += 1].Value = "SRF Number";
                worksheet.Cells[index, i += 1].Value = "Contractor Name";
                worksheet.Cells[index, i += 1].Value = "Organization Unit";
                worksheet.Cells[index, i += 1].Value = "Sub Organization Unit";
                worksheet.Cells[index, i += 1].Value = "Account Name";
                worksheet.Cells[index, i += 1].Value = "Workstation";
                worksheet.Cells[index, i += 1].Value = "Communication";
                worksheet.Cells[index, i += 1].Value = "Rate";
                worksheet.Cells[index, i += 1].Value = "Price Type";
                worksheet.Cells[index, i += 1].Value = "Cost Center";
                worksheet.Cells[index, i += 1].Value = "Signum / Nonsignum";
                worksheet.Cells[index, i += 1].Value = "SRF Type";
                worksheet.Cells[index, i += 1].Value = "No NN";
                worksheet.Cells[index, i += 1].Value = "Jobstage";
                worksheet.Cells[index, i += 1].Value = "Service Work Package";
                worksheet.Cells[index, i += 1].Value = "Service Code";
                worksheet.Cells[index, i += 1].Value = "Contract Start";
                worksheet.Cells[index, i += 1].Value = "Contract End";
                worksheet.Cells[index, i += 1].Value = "Price";
                worksheet.Cells[index, i += 1].Value = "Annual Leave";
                worksheet.Cells[index, i += 1].Value = "Overtime";
                worksheet.Cells[index, i += 1].Value = "Hourly Rate";
                worksheet.Cells[index, i += 1].Value = "OT Lumpsum";
                worksheet.Cells[index, i += 1].Value = "Laptop Allowance";
                worksheet.Cells[index, i += 1].Value = "Usim Broadband";
                worksheet.Cells[index, i += 1].Value = "Special Rate";
                worksheet.Cells[index, i += 1].Value = "Price Per Month";
                worksheet.Cells[index, i += 1].Value = "Duration";
                worksheet.Cells[index, i += 1].Value = "Total Price Per Month * Duration";
                worksheet.Cells[index, i += 1].Value = "Project";
                worksheet.Cells[index, i += 1].Value = "Line Manager";
                worksheet.Cells[index, i += 1].Value = "Project Manager";
                worksheet.Cells[index, i += 1].Value = "Supplier";
                worksheet.Cells[index, i += 1].Value = "Submitted Date";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;

                Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[18];
                Includes[0] = pack => pack.Candidate;
                Includes[1] = pack => pack.ApproveOneBy;
                Includes[2] = pack => pack.Candidate.Agency;
                Includes[3] = pack => pack.Departement;
                Includes[4] = pack => pack.DepartementSub;
                Includes[5] = pack => pack.Account;
                Includes[6] = pack => pack.NetworkNumber;
                Includes[7] = pack => pack.NetworkNumber.ProjectManager;
                Includes[8] = pack => pack.CostCenter;
                Includes[9] = pack => pack.Candidate.Vacancy;
                Includes[10] = pack => pack.Candidate.Vacancy.JobStage;
                Includes[11] = pack => pack.ServicePack;
                Includes[12] = pack => pack.ServicePack.ServicePackCategory;
                Includes[13] = pack => pack.NetworkNumber.Project;
                Includes[14] = pack => pack.Escalation;
                Includes[15] = pack => pack.Escalation.ServicePack;
                Includes[16] = pack => pack.Candidate.Account;
                Includes[17] = pack => pack.Candidate.Vacancy.PackageType;

                var Data = ListData(month, year);

                if (Data!=null)
                {
                    foreach(var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.Number;
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Account.AhId;
                        worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(row.Number) ? "e-EID/KI-" + row.CreatedAt.Value.ToString("yy", CultureInfo.InvariantCulture) + ":SRF: " + row.Number + " UEN" : "-";
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Account.Name;
                        worksheet.Cells[index, j += 1].Value = row.Departement == null ? "-" : row.Departement.Name;
                        worksheet.Cells[index, j += 1].Value = row.DepartementSub == null ? "-" : row.DepartementSub.SubName;
                        worksheet.Cells[index, j += 1].Value = row.Account == null ? "-" : row.Account.Name;
                        worksheet.Cells[index, j += 1].Value = row.isWorkstation == true ? "Yes" : "No";
                        worksheet.Cells[index, j += 1].Value = row.isCommunication == true ? "Yes" : "No";
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(RateType), row.RateType).ToString();
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Vacancy.PackageType == null ? "-" : row.Candidate.Vacancy.PackageType.Name;
                        worksheet.Cells[index, j += 1].Value = row.CostCenter == null ? "-" : row.CostCenter.DisplayName;
                        worksheet.Cells[index, j += 1].Value = row.IsHrms == true ? "HRMS" : "No HRMS";
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(SrfType), row.Type).ToString();
                        worksheet.Cells[index, j += 1].Value = row.NetworkNumber == null ? "-" : row.NetworkNumber.Code;
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Vacancy.JobStage == null ? "-" : row.Candidate.Vacancy.JobStage.Description;
                        worksheet.Cells[index, j += 1].Value = row.ServicePack.ServicePackCategory == null ? "-" : row.ServicePack.ServicePackCategory.Name;
                        worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "-" : row.ServicePack.Code;
                        worksheet.Cells[index, j += 1].Value = row.SrfBegin.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.SrfEnd.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
                       
                        if(row.Escalation == null)
                        {
                            worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "0" : row.ServicePack.Rate.ToString("#,##0"); // Price
                        }
                        else
                        {
                            worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "0" : row.Escalation.ServicePack.Rate.ToString("#,##0"); // Price
                        }

                        worksheet.Cells[index, j += 1].Value = row.AnnualLeave == 0 ? "Annual Leave is Expired" : row.AnnualLeave+" Days";

                        if(row.Escalation==null)
                        {
                            decimal otLevel = row.ServiceLevel;
                            decimal hourly = row.ServicePack.Hourly;
                            decimal otLumpsum = otLevel * hourly;
                            decimal serviceRate = row.ServicePack.Rate;
                            decimal specialRate = row.SpectValue;
                            decimal laptop = row.isWorkstation == false ? 0 : row.ServicePack.Laptop;
                            decimal usin = row.Candidate.Vacancy.isUsim == false ? 0 : row.ServicePack.Usin;
                            decimal pricePerMonth = serviceRate + otLumpsum + laptop + usin + specialRate;

                            worksheet.Cells[index, j += 1].Value = otLevel;
                            worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "0": row.ServicePack.Hourly.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.ServicePack == null ? "0" : (row.ServicePack.Hourly * row.Candidate.Vacancy.OtLevel).ToString("#,##0"); 
                            worksheet.Cells[index, j += 1].Value = row.isWorkstation == false ? "0": row.ServicePack.Laptop.ToString("#,##0"); 
                            worksheet.Cells[index, j += 1].Value = row.Candidate.Vacancy.isUsim == false ? "0" : row.ServicePack.Usin.ToString("#,##0"); 
                            worksheet.Cells[index, j += 1].Value = row.SpectValue.ToString("#,##0"); 
                            worksheet.Cells[index, j += 1].Value = pricePerMonth.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.GetDuration();
                            worksheet.Cells[index, j += 1].Value = (row.GetDuration() * pricePerMonth).ToString("#,##0");
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

                            worksheet.Cells[index, j += 1].Value = row.Escalation.OtLevel;
                            worksheet.Cells[index, j += 1].Value = row.Escalation.ServicePack == null ? "0" : row.Escalation.ServicePack.Hourly.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.Escalation.ServicePack == null ? "0" : (row.Escalation.ServicePack.Hourly * row.Escalation.OtLevel).ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.Escalation.IsWorkstation == false ? "0" : row.Escalation.ServicePack.Laptop.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.Candidate.Vacancy.isUsim == false ? "0" : row.Escalation.ServicePack.Usin.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.SpectValue.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = pricePerMonth.ToString("#,##0");
                            worksheet.Cells[index, j += 1].Value = row.GetDuration();
                            worksheet.Cells[index, j += 1].Value = (row.GetDuration() * pricePerMonth).ToString("#,##0");
                        }

                        
                        worksheet.Cells[index, j += 1].Value = row.NetworkNumber.Project == null ? "-" : row.NetworkNumber.Project.Name;
                        worksheet.Cells[index, j += 1].Value = row.ApproveOneBy == null ? "-" : row.ApproveOneBy.Name;
                        worksheet.Cells[index, j += 1].Value = row.NetworkNumber.ProjectManager == null ? "-" : row.NetworkNumber.ProjectManager.Name;
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Agency == null ? "-" : row.Candidate.Agency.Name;
                        worksheet.Cells[index, j += 1].Value = row.CreatedAt.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        index++;
                    }
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return Redirect(URL);
        }

        public List<SrfRequest> ListData(string month = null, string year = null)
        {
            var userLogin = _userHelper.GetLoginUser(User);
            var profileId = userLogin.Id;
            Expression<Func<SrfRequest, object>>[] includes = new Expression<Func<SrfRequest, object>>[18];
            includes[0] = pack => pack.Candidate;
            includes[1] = pack => pack.ApproveOneBy;
            includes[2] = pack => pack.Candidate.Agency;
            includes[3] = pack => pack.Departement;
            includes[4] = pack => pack.DepartementSub;
            includes[5] = pack => pack.Account;
            includes[6] = pack => pack.NetworkNumber;
            includes[7] = pack => pack.ProjectManager;
            includes[8] = pack => pack.CostCenter;
            includes[9] = pack => pack.Candidate.Vacancy;
            includes[10] = pack => pack.Candidate.Vacancy.JobStage;
            includes[11] = pack => pack.ServicePack;
            includes[12] = pack => pack.ServicePack.ServicePackCategory;
            includes[13] = pack => pack.NetworkNumber.Project;
            includes[14] = pack => pack.Escalation;
            includes[15] = pack => pack.Escalation.ServicePack;
            includes[16] = pack => pack.Candidate.Vacancy.PackageType;
            includes[17] = pack => pack.Candidate.Account;

            var data = Service.GetAll(includes)
                .Where(x => x.ApproveStatusSix == SrfApproveStatus.Approved
                    && x.Candidate.AccountId.HasValue
                    && x.Candidate.IsUser == true
                    && x.Candidate.Account.IsBlacklist == false
                    && x.Candidate.Account.IsTerminate == false);

            int m, y;
            if (!string.IsNullOrWhiteSpace(month) && !string.IsNullOrWhiteSpace(year)
                && int.TryParse(month, out m) && int.TryParse(year, out y))
            {
                var lastDayMonth = DateTime.DaysInMonth(y, m);
                var firstDate = new DateTime(y, m, 1);
                var lastDate = new DateTime(y, m, lastDayMonth);
                data = data.Where(x => (firstDate >= x.SrfBegin && firstDate <= x.SrfEnd)
                    || (lastDate >= x.SrfBegin && lastDate <= x.SrfEnd));
            }

            if (User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
                return data.Where(x => x.Departement.HeadId == profileId).ToList();

            if (User.IsInRole("Head Of Service Line"))
                return data.Where(x => x.DepartementSub.LineManagerid == profileId).ToList();

            if (User.IsInRole("Head Of Sourcing"))
                return data.Where(x => x.ApproveFiveId == profileId).ToList();

            if (User.IsInRole("Customer Operation Manager"))
                return data.Where(x => x.Account.Com == profileId).ToList();

            if (User.IsInRole("Line Manager"))
                return data.Where(x => x.ApproveOneId == profileId).ToList();

            if (User.IsInRole("Service Coordinator"))
                return data.Where(x => x.ApproveSixId == profileId).ToList();

            if (User.IsInRole("HR Agency"))
                return data.Where(x => x.Candidate.AgencyId == profileId).ToList();

            if (User.IsInRole("Sourcing"))
                return data.Where(x => x.Candidate.Vacancy.ApproverTwoId == profileId).ToList();

            return data.ToList();
        }
    }
}
