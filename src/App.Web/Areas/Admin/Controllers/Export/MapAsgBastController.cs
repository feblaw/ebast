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
    public class MapAsgBastController : BaseController<MapAsgBast, IMapAsgBastService, MapAsgBastViewModel, MapAsgBastFormModel, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;
        public MapAsgBastController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IMapAsgBastService service,
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
            string sFileName = @"report/MapAsgBast.xlsx";
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
                    worksheet.Cells[index, i += 1].Value = "PO Number";
                    worksheet.Cells[index, i += 1].Value = "Line Item PO";
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
                //Includes[4] = pack => pack.Asp;
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


                var Data = Service.GetAll(Includes).OrderByDescending(x=> x.Bast.BastReqNo).ToList();

                if (User.IsInRole("ASP Admin"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Assignment.AspId.ToString().Equals(ASP.ToString())).ToList();
                    //Data = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"Assignment.AspId.toString().Equals(\"{ASP.ToString()}\")", Includes);
                    //response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"AspId.toString().Equals(\"{ASP.ToString()}\")", Includes);
                }

                if (User.IsInRole("ASP PM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Bast.ApprovalOneID == ProfileId).ToList();
                    
                }

                if (User.IsInRole("PA"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Bast.ApprovalTwoID == ProfileId && x.Assignment.AssignmentCancel == false).ToList();
                }

                if (User.IsInRole("IM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Bast.ApprovalThreeID == ProfileId && x.Assignment.AssignmentCancel == false).ToList();
                }

                if (User.IsInRole("CPM"))
                {
                    Data = Service.GetAll(Includes).Where(x => x.Bast.ApprovalFourID == ProfileId && x.Assignment.AssignmentCancel == false).ToList();
                }
                

                if (Data != null)
                {
                   
                        foreach (var row in Data)
                        {
                            int j = 0;
                            worksheet.Cells[index, j += 1].Value = row.Bast.BastReqNo;
                            worksheet.Cells[index, j += 1].Value = row.Assignment.AssignmentId;
                            worksheet.Cells[index, j += 1].Value = row.Assignment.PONumber;
                            worksheet.Cells[index, j += 1].Value = row.Assignment.LineItemPO;
                            worksheet.Cells[index, j += 1].Value = row.Assignment.Asp.Name;
                            worksheet.Cells[index, j += 1].Value = row.Bast.BastNo == null ? "-" : "EID/"+row.Bast.OtherInfo+"/" + row.Bast.ApprovalFourDate.ToString("yyyy", CultureInfo.InvariantCulture) + "/:" + row.Bast.BastNo;
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
