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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Globalization;
using App.Domain.Models.Enum;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class BastController : BaseController<Bast, IBastService, BastViewModel, BastFormModel, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;
        public BastController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IBastService service,
            IHostingEnvironment env,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
        }

        public override IActionResult Index()
        {
            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            var Profile = _userHelper.GetLoginUser(User);
            var ProfileId = Profile.Id;
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/Bast.xlsx";
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

                if (!User.IsInRole("HR Agency"))
                {
                    worksheet.Cells[index, i += 1].Value = "Bast Req No";
                    worksheet.Cells[index, i += 1].Value = "Bast No";
                    worksheet.Cells[index, i += 1].Value = "TOP";
                    worksheet.Cells[index, i += 1].Value = "Value";
                    worksheet.Cells[index, i += 1].Value = "Req By";//ah id
                    worksheet.Cells[index, i += 1].Value = "Project";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                }
                else
                {
                    worksheet.Cells[index, i += 1].Value = "Bast Req No";
                    worksheet.Cells[index, i += 1].Value = "Bast No";
                    worksheet.Cells[index, i += 1].Value = "TOP";
                    worksheet.Cells[index, i += 1].Value = "Value";
                    worksheet.Cells[index, i += 1].Value = "Req By";//ah id
                    worksheet.Cells[index, i += 1].Value = "Project";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";
                    worksheet.Cells[index, i += 1].Value = "ASP PM";
                    worksheet.Cells[index, i += 1].Value = "Status";
                    worksheet.Cells[index, i += 1].Value = "Approval Date";

                    //worksheet.Cells[index, i += 1].Value = "Unit Price";
                    //worksheet.Cells[index, i += 1].Value = "Quantity/Monthly";
                    //worksheet.Cells[index, i += 1].Value = "Service Value/Monthly";
                    //worksheet.Cells[index, i += 1].Value = "Start Date";
                    //worksheet.Cells[index, i += 1].Value = "End Date";
                }


                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;
                Expression<Func<Bast, object>>[] Includes = new Expression<Func<Bast, object>>[5];
                Includes[0] = pack => pack.ApprovalFour;
                Includes[1] = pack => pack.ApprovalOne;
                Includes[2] = pack => pack.ApprovalThree;
                Includes[3] = pack => pack.ApprovalTwo;
                Includes[4] = pack => pack.Asp;
                //Includes[0] = pack => pack.ServicePack;
                //Includes[1] = pack => pack.ServicePackCategory;
                //Includes[2] = pack => pack.ApproverOne;
                //Includes[3] = pack => pack.Candidate;
                //Includes[4] = pack => pack.Departement;
                //Includes[5] = pack => pack.DepartementSub;
                //Includes[6] = pack => pack.Vendor;
                //Includes[7] = pack => pack.ApproverTwo;
                //Includes[8] = pack => pack.ApproverThree;
                //Includes[9] = pack => pack.Network;


                var Data = Service.GetAll(Includes).ToList();

                if (User.IsInRole("ASP Admin"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.AspId.ToString().Equals(ASP.ToString())).ToList();
                    //response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"AspId.toString().Equals(\"{ASP.ToString()}\")", Includes);
                }

                if (User.IsInRole("ASP PM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApprovalOneID == ProfileId).ToList();
                    
                }

                if (User.IsInRole("PA"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApprovalTwoID == ProfileId).ToList();
                }

                if (User.IsInRole("IM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApprovalThreeID == ProfileId).ToList();
                }

                if (User.IsInRole("CPM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.ApprovalFourID == ProfileId).ToList();
                }
                

                if (Data != null)
                {
                    if (!User.IsInRole("HR Agency"))
                    {

                        foreach (var row in Data)
                        {
                            int j = 0;
                            worksheet.Cells[index, j += 1].Value = row.BastReqNo;
                            worksheet.Cells[index, j += 1].Value = row.BastNo;
                            worksheet.Cells[index, j += 1].Value = row.TOP;
                            worksheet.Cells[index, j += 1].Value = row.totalValue;
                            worksheet.Cells[index, j += 1].Value = row.CreatedBy;
                            worksheet.Cells[index, j += 1].Value = row.Project;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOne.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOneStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOneDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); 
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwo.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwoStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwoDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); 
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThree.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThreeStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThreeDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFour.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFourStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFourDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            index++;
                        }
                    }
                    else
                    {
                        foreach (var row in Data)
                        {
                            int j = 0;
                            worksheet.Cells[index, j += 1].Value = row.BastReqNo;
                            worksheet.Cells[index, j += 1].Value = row.BastNo;
                            worksheet.Cells[index, j += 1].Value = row.TOP;
                            worksheet.Cells[index, j += 1].Value = row.totalValue;
                            worksheet.Cells[index, j += 1].Value = row.RequestBy;
                            worksheet.Cells[index, j += 1].Value = row.Project;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOne.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOneStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalOneDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwo.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwoStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalTwoDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThree.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThreeStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalThreeDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFour.Name;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFourStatus;
                            worksheet.Cells[index, j += 1].Value = row.ApprovalFourDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                            //worksheet.Cells[index, j += 1].Value = row.ServicePack.Name;
                            //worksheet.Cells[index, j += 1].Value = row.ServicePack.Rate;
                            //worksheet.Cells[index, j += 1].Value = row.Quantity;
                            //worksheet.Cells[index, j += 1].Value = row.NoarmalRate;
                            //worksheet.Cells[index, j += 1].Value = row.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //worksheet.Cells[index, j += 1].Value = row.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            index++;
                        }
                    }
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return Redirect(URL);
        }

        //public override IActionResult lmrtemplate()
        //{
        //    var Profile = _userHelper.GetLoginUser(User);
        //    var ProfileId = Profile.Id;
        //    string sWebRootFolder = _env.WebRootPath;
        //    string sFileName = @"report/LMR.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    if (file.Exists)
        //    {
        //        file.Delete();
        //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    }
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
        //        int i = 0;
        //        int index = 1;
        //        worksheet.Cells[index, i += 1].Value = "Customer Unit Name";
        //        worksheet.Cells[index, i += 1].Value = "Project Name";
        //        worksheet.Cells[index, i += 1].Value = "LMR No";
        //        worksheet.Cells[index, i += 1].Value = "Activity/Services Number";//ah id
        //        worksheet.Cells[index, i += 1].Value = "Qty";
        //        worksheet.Cells[index, i += 1].Value = "Unit Price";
        //        worksheet.Cells[index, i += 1].Value = "Total Amount";
        //        worksheet.Cells[index, i += 1].Value = "Currency";
        //        worksheet.Cells[index, i += 1].Value = "NW ID";
        //        worksheet.Cells[index, i += 1].Value = "NW Activity";
        //        worksheet.Cells[index, i += 1].Value = "Purchasing Grp";
        //        worksheet.Cells[index, i += 1].Value = "Material Group";
        //        worksheet.Cells[index, i += 1].Value = "Vendor Code";
        //        worksheet.Cells[index, i += 1].Value = "Vendor Name";
        //        worksheet.Cells[index, i += 1].Value = "Vendor Payment Terms";
        //        worksheet.Cells[index, i += 1].Value = "Header Text";
        //        worksheet.Cells[index, i += 1].Value = "PR No.";
        //        worksheet.Cells[index, i += 1].Value = "PR Line Item";
        //        worksheet.Cells[index, i += 1].Value = "PO No.";
        //        worksheet.Cells[index, i += 1].Value = "PO Line Item";
        //        worksheet.Cells[index, i += 1].Value = "L1 Approver";
        //        worksheet.Cells[index, i += 1].Value = "L2 Approver";
        //        worksheet.Cells[index, i += 1].Value = "L3 Approver";
        //        worksheet.Cells[index, i += 1].Value = "Requestor Name";
        //        worksheet.Cells[index, i += 1].Value = "Employee Name";
        //        worksheet.Cells[index, i += 1].Value = "Employee NIK";
        //        worksheet.Cells[index, i += 1].Value = "StartDate";
        //        worksheet.Cells[index, i += 1].Value = "EndDate";
        //        worksheet.Cells[index, i += 1].Value = "System Key";



        //        for (int i_temp = 1; i_temp <= i; i_temp++)
        //        {
        //            worksheet.Cells[index, i_temp].Style.Font.Bold = true;
        //            worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
        //            worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
        //        }

        //        index += 1;
        //        Expression<Func<Bast, object>>[] Includes = new Expression<Func<Bast, object>>[10];
        //        Includes[0] = pack => pack.ServicePack;
        //        Includes[1] = pack => pack.ServicePackCategory;
        //        Includes[2] = pack => pack.ApproverOne;
        //        Includes[3] = pack => pack.Candidate;
        //        Includes[4] = pack => pack.Departement;
        //        Includes[5] = pack => pack.DepartementSub;
        //        Includes[6] = pack => pack.Vendor;
        //        Includes[7] = pack => pack.ApproverTwo;
        //        Includes[8] = pack => pack.ApproverThree;
        //        Includes[9] = pack => pack.Network;


        //        var Data = Service.GetAll(Includes).Where(x => x.EndDate > DateTime.Now && x.StatusOne == SrfApproveStatus.Approved && x.StatusTwo == SrfApproveStatus.Approved
        //        && x.StatusThree == SrfApproveStatus.Approved).To();

        //        if (Data != null)
        //        {
        //            foreach (var row in Data)
        //            {
        //                int j = 0;
        //                worksheet.Cells[index, j += 1].Value = row.Departement.Name;
        //                worksheet.Cells[index, j += 1].Value = row.DepartementSub.SubName;
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = row.ServicePack.ServiceCode;
        //                worksheet.Cells[index, j += 1].Value = row.Quantity;
        //                worksheet.Cells[index, j += 1].Value = row.ServicePack.Rate;
        //                worksheet.Cells[index, j += 1].Value = row.NoarmalRate;
        //                worksheet.Cells[index, j += 1].Value = "IDR";
        //                worksheet.Cells[index, j += 1].Value = row.Network.Code;
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "IDC";
        //                worksheet.Cells[index, j += 1].Value = "ZSS0300";
        //                worksheet.Cells[index, j += 1].Value = row.Vendor.AhId;
        //                worksheet.Cells[index, j += 1].Value = row.Vendor.Name;
        //                worksheet.Cells[index, j += 1].Value = "100%";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = row.PONumber;
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "";
        //                worksheet.Cells[index, j += 1].Value = "EIKAIRA";
        //                worksheet.Cells[index, j += 1].Value = row.Name;
        //                worksheet.Cells[index, j += 1].Value = row.NIK;
        //                worksheet.Cells[index, j += 1].Value = row.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); ;
        //                worksheet.Cells[index, j += 1].Value = row.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); ;
        //                worksheet.Cells[index, j += 1].Value = row.Id;
        //               index++;
        //            }
        //        }
        //        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //        package.Save();
        //    }
        //    return Redirect(URL);
        //}

    }
}
