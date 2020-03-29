using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Domain.Models.Core;
using App.Web.Models.ViewModels.Core.Business;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq.Expressions;
using System.Globalization;
using App.Domain.Models.Enum;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class TravelController : BaseController<Claim, IClaimService, TravelRequestViewModel, TravelRequestModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;

        public TravelController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IClaimService service,
            IHostingEnvironment env,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
        }

        public override IActionResult Index()
        {
            var Profile = _userHelper.GetLoginUser(User);
            var ProfileId = Profile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/TravelRequest.xlsx";
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
                worksheet.Cells[1, 1].Value = "Travel No";
                worksheet.Cells[1, 2].Value = "Supplier";
                worksheet.Cells[1, 3].Value = "Request By";
                worksheet.Cells[1, 4].Value = "Departure";
                worksheet.Cells[1, 5].Value = "Main Destination";
                worksheet.Cells[1, 6].Value = "Start Date";
                worksheet.Cells[1, 7].Value = "End Date";
                worksheet.Cells[1, 8].Value = "Request Description";
                //worksheet.Cells[1, 8].Value = "Ticket Status";
                //worksheet.Cells[1, 9].Value = "Status"; // Status Travel

                worksheet.Cells[1, 9, 1, 10].Merge = true;
                worksheet.Cells[1, 9, 1, 10].Value = "Line Manager Approval";
                worksheet.Cells[1, 9, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[1, 12, 1, 13].Merge = true;
                worksheet.Cells[1, 12, 1, 13].Value = "Project Manager Approval";
                worksheet.Cells[1, 12, 1, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //worksheet.Cells[1, 16].Value = "Cost Center";
                //worksheet.Cells[1, 17].Value = "Network Number";
                //worksheet.Cells[1, 18].Value = "Activity";
                //worksheet.Cells[1, 19].Value = "Organization Unit";
                //worksheet.Cells[1, 20].Value = "Sub Organization Unit";

                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 2].Value = "";
                worksheet.Cells[2, 3].Value = "";
                worksheet.Cells[2, 4].Value = "";
                worksheet.Cells[2, 5].Value = "";
                worksheet.Cells[2, 6].Value = "";
                worksheet.Cells[2, 7].Value = "";
                worksheet.Cells[2, 8].Value = "";
                //worksheet.Cells[2, 9].Value = "";
                worksheet.Cells[2, 9].Value = "Status";
                worksheet.Cells[2, 10].Value = "Project Manager";
                worksheet.Cells[2, 11].Value = "Approve Date - Remark";
                worksheet.Cells[2, 12].Value = "Status";
                worksheet.Cells[2, 13].Value = "Line Manager";
                worksheet.Cells[2, 14].Value = "Approve Date - Remark";
                worksheet.Cells[2, 15].Value = "";
                worksheet.Cells[2, 16].Value = "";
                worksheet.Cells[2, 17].Value = "";
                worksheet.Cells[2, 18].Value = "";
                worksheet.Cells[2, 19].Value = "";

                for (int ii = 1; ii <= 2; ii++)
                {
                    for (int jj = 1; jj <= 20; jj++)
                    {
                        worksheet.Cells[ii, jj].Style.Font.Bold = true;
                        worksheet.Cells[ii, jj].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[ii, jj].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                        worksheet.Cells[ii, jj].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                }

                int index = 3;
                Expression<Func<Claim, object>>[] Includes = new Expression<Func<Claim, object>>[14];
                Includes[0] = pack => pack.Contractor;
                Includes[1] = pack => pack.Agency;
                Includes[2] = pack => pack.Departure;
                Includes[3] = pack => pack.Destination;
                Includes[4] = pack => pack.ClaimApproverOne;
                Includes[5] = pack => pack.ClaimApproverTwo;
                Includes[6] = pack => pack.CostCenter;
                Includes[7] = pack => pack.NetworkNumber;
                Includes[8] = pack => pack.ActivityCode;
                Includes[9] = pack => pack.ContractorProfile;
                Includes[10] = pack => pack.ContractorProfile.Vacancy;
                Includes[11] = pack => pack.ContractorProfile.Vacancy.Departement;
                Includes[12] = pack => pack.ContractorProfile.Vacancy.DepartementSub;
                Includes[13] = pack => pack.Ticket;

                #region DataSource
                var Data = Service.GetAll(Includes).Where(x => x.ClaimType == Domain.Models.Enum.ClaimType.TravelClaim).ToList();
                //if (User.IsInRole("Contractor"))
                //{
                //    Data = Service.GetAll(Includes).Where(x => x.ClaimType == Domain.Models.Enum.ClaimType.TravelClaim && x.ContractorId == ProfileId).ToList();
                //}
                if (User.IsInRole("Line Manager"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ClaimType == Domain.Models.Enum.ClaimType.TravelClaim && x.ClaimApproverTwoId == ProfileId).ToList();
                }
                else if (User.IsInRole("Project Manager"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ClaimType == Domain.Models.Enum.ClaimType.TravelClaim && x.ClaimApproverOneId == ProfileId).ToList();
                }
                else if (User.IsInRole("HR Agency"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ClaimType == Domain.Models.Enum.ClaimType.TravelClaim && x.AgencyId == ProfileId).ToList();
                }
                #endregion


                if(Data!=null)
                {
                    foreach(var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.TravelReqNo;
                        worksheet.Cells[index, j += 1].Value = row.Agency == null ? "-" : row.Agency.Name;
                        worksheet.Cells[index, j += 1].Value = row.Contractor == null ? "-" : row.Contractor.Name;
                        worksheet.Cells[index, j += 1].Value = row.Departure == null ? "-" : row.Departure.Name;
                        worksheet.Cells[index, j += 1].Value = row.Destination == null ? "-" : row.Destination.Name;
                        worksheet.Cells[index, j += 1].Value = row.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Description;
                        //worksheet.Cells[index, j += 1].Value = row.Ticket == null ? "-" : Enum.GetName(typeof(TicketInfoStatus), row.Ticket.Status).ToString();
                        //worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(ActiveStatus), row.ClaimStatus).ToString();
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(StatusOne), row.StatusOne).ToString();
                        worksheet.Cells[index, j += 1].Value = row.ClaimApproverOne == null ? "-" : row.ClaimApproverOne.Name;
                        worksheet.Cells[index, j += 1].Value = row.ApprovedDateOne.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(StatusTwo), row.StatusTwo).ToString();
                        worksheet.Cells[index, j += 1].Value = row.ClaimApproverTwo == null ? "-" : row.ClaimApproverTwo.Name;
                        worksheet.Cells[index, j += 1].Value = row.ApprovedDateTwo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //worksheet.Cells[index, j += 1].Value = row.CostCenter == null ? "-" : row.CostCenter.DisplayName;
                        //worksheet.Cells[index, j += 1].Value = row.NetworkNumber == null ? "-" : row.NetworkNumber.DisplayName;
                        //worksheet.Cells[index, j += 1].Value = row.ActivityCode == null ? "-" : row.ActivityCode.DisplayName;
                        //worksheet.Cells[index, j += 1].Value = row.ContractorProfile.Vacancy.Departement == null ? "-" : row.ContractorProfile.Vacancy.Departement.Name;
                        //worksheet.Cells[index, j += 1].Value = row.ContractorProfile.Vacancy.DepartementSub == null ? "-" : row.ContractorProfile.Vacancy.DepartementSub.SubName;
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
