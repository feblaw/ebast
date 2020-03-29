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
using System.Linq.Expressions;
using App.Domain.Models.Enum;
using System.Globalization;

namespace App.Web.Areas.Admin.Controllers.Export
{

    [Area("Export")]
    [Authorize]
    public class ContractorDataController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;
        private readonly ISrfEscalationRequestService _escalation;

        public ContractorDataController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IHostingEnvironment env,
            ISrfEscalationRequestService escalation,
            IMapper mapper, 
            ISrfRequestService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
            _escalation = escalation;
        }

        public override IActionResult Index()
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/ContractorData.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Type";
                worksheet.Cells[index, i += 1].Value = "Contractor Name";
                worksheet.Cells[index, i += 1].Value = "SRF Number";
                worksheet.Cells[index, i += 1].Value = "Email";
                worksheet.Cells[index, i += 1].Value = "Position";
                worksheet.Cells[index, i += 1].Value = "Contractor Start";
                worksheet.Cells[index, i += 1].Value = "Contractor End";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Rate";
                worksheet.Cells[index, i += 1].Value = "Organization Unit";
                worksheet.Cells[index, i += 1].Value = "Sub Organization Unit";
                worksheet.Cells[index, i += 1].Value = "Line Manager";
                worksheet.Cells[index, i += 1].Value = "Project Manager";
                worksheet.Cells[index, i += 1].Value = "Supplier";
                worksheet.Cells[index, i += 1].Value = "Terminate Note";
                worksheet.Cells[index, i += 1].Value = "Blacklist Note";
                worksheet.Cells[index, i += 1].Value = "Date";
                worksheet.Cells[index, i += 1].Value = "Signum";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;
                Expression<Func<SrfRequest, object>>[] Includes = new Expression<Func<SrfRequest, object>>[11];
                Includes[0] = pack => pack.Candidate;
                Includes[1] = pack => pack.Candidate.Account;
                Includes[2] = pack => pack.ServicePack;
                Includes[3] = pack => pack.ServicePack.ServicePackCategory;
                Includes[4] = pack => pack.Departement;
                Includes[5] = pack => pack.DepartementSub;
                Includes[6] = pack => pack.ApproveOneBy;
                Includes[7] = pack => pack.NetworkNumber;
                Includes[8] = pack => pack.NetworkNumber.ProjectManager;
                Includes[9] = pack => pack.Candidate.Agency;
                Includes[10] = pack => pack.Candidate.Vacancy.PackageType;

                var Data = (List<SrfRequest>)null;

                #region Approver
                if (User.IsInRole("Line Manager"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveOneId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }
                else if (User.IsInRole("Head Of Service Line"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveTwoId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }

                else if (User.IsInRole("Head Of Operation"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveThreeId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }

                else if (User.IsInRole("Head Of Non Operation"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveFourId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }

                else if (User.IsInRole("Head Of Sourcing"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveFiveId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }
                else if (User.IsInRole("Service Coordinator"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApproveSixId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }

                else if (User.IsInRole("HR Agency"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Candidate.AgencyId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }

                else if (User.IsInRole("Sourcing"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Candidate.Vacancy.ApproverTwoId == PreofileId && x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }
                else
                {
                    Data = Service.GetAll(Includes).Where(x => x.IsLocked == false && x.Candidate.IsUser == true && x.IsActive == true).ToList();
                }
                #endregion`

                if(Data!=null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(row.Number) ? row.Number : "-";
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Vacancy.PackageType == null ? "-" : row.Candidate.Vacancy.PackageType.Name;
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Name;
                        worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(row.Number) ? "e-EID/KI-" + row.CreatedAt.Value.ToString("yy", CultureInfo.InvariantCulture) + ":SRF: " + row.Number + " UEN" : "-";
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Email;
                        worksheet.Cells[index, j += 1].Value = row.ServicePack.ServicePackCategory == null ? "-" : row.ServicePack.ServicePackCategory.Name;
                        worksheet.Cells[index, j += 1].Value = row.SrfBegin.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.SrfEnd.Value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
                        
                        if(row.Status == SrfStatus.Terminate || row.Status == SrfStatus.Blacklist)
                        {
                            if(row.Status == SrfStatus.Terminate)
                            {
                                worksheet.Cells[index, j += 1].Value = "Terminate";
                            }
                            else
                            {
                                worksheet.Cells[index, j += 1].Value = "Blacklist";
                            }
                        }
                        else
                        {
                            worksheet.Cells[index, j += 1].Value = "Active";
                        }

                        var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(row.Id)).ToList();
                        if(Escalation.Any())
                        {
                            worksheet.Cells[index, j += 1].Value = "Special Rate";
                        }
                        else
                        {
                            worksheet.Cells[index, j += 1].Value = "Normal Rate";
                        }
                       
                        worksheet.Cells[index, j += 1].Value = row.Departement == null ? "-" : row.Departement.Name;
                        worksheet.Cells[index, j += 1].Value = row.DepartementSub == null ? "-" : row.DepartementSub.SubName;
                        worksheet.Cells[index, j += 1].Value = row.ApproveOneBy == null ? "-" : row.ApproveOneBy.Name;
                        worksheet.Cells[index, j += 1].Value = row.NetworkNumber.ProjectManager == null ? "-" : row.NetworkNumber.ProjectManager.Name;
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Agency == null ? "-" : row.Candidate.Agency.Name;
                        worksheet.Cells[index, j += 1].Value = row.TeriminateNote;
                        worksheet.Cells[index, j += 1].Value = row.TeriminateNote;
                        worksheet.Cells[index, j += 1].Value = row.CreatedAt.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        worksheet.Cells[index, j += 1].Value = row.Candidate.Account == null ? "-" : row.Candidate.Account.AhId;
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
