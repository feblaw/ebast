using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Authorize]
    [Area("Export")]
    public class TimeSheetActivityController : BaseController<AttendaceExceptionList, IAttendaceExceptionListService, AttendaceExceptionListViewModel, AttendaceExceptionListModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserHelper _userHelper;
        private readonly IAttendanceRecordService _record;
        private readonly IJobStageService _jobstage;

        public TimeSheetActivityController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IHostingEnvironment env,
            IAttendaceExceptionListService service,
            IAttendanceRecordService record,
            IJobStageService jobstage,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _userHelper = userHelper;
            _record = record;
            _jobstage = jobstage;
        }


        public IActionResult Download(string ValueDate)
        {
            var valueDate = ValueDate.Split('-');
            var valueMonth = valueDate[0];
            var valueYears = valueDate[1];

            var dateMonth = int.Parse(valueMonth);
            var dateYears = int.Parse(valueYears);
            int days = DateTime.DaysInMonth(dateYears, dateMonth);

            var FirstDate = new DateTime(dateYears, dateMonth, 1);
            var LastDate = new DateTime(dateYears, dateMonth, days);

            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/TimeSheetActivity.xlsx";
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
                int max = 31;
                int dist = max - days;

                // Header 1
                int numVal = 0;
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = dateYears.ToString();
                worksheet.Cells[index, numVal += 1].Value = "MONTH";
                worksheet.Cells[index, numVal += 1].Value = dateMonth.ToString();
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";

                for (int a = 1; a <= days; a++)
                {
                    var _date = a > 9 ? a.ToString() : "0" + a;
                    var _month = dateMonth > 9 ? dateMonth.ToString() : "0" + dateMonth;
                    var _dateHeader = String.Format("{0}-{1}-{2}",dateYears, _month,_date);
                    DateTime dateHeader = DateTime.Parse(_dateHeader);
                    worksheet.Cells[index, numVal += 1].Value = dateHeader.ToString("ddd").ToUpper();
                }

                for (int ds = 1; ds <= dist; ds++)
                {
                    worksheet.Cells[index, numVal += 1].Value = "";
                }

                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";

                // Header 2
                index += 1;
                worksheet.Cells[index, i += 1].Value = "Personal Number";
                worksheet.Cells[index, i += 1].Value = "Location";
                worksheet.Cells[index, i += 1].Value = "Activity Type";
                worksheet.Cells[index, i += 1].Value = "Order";
                worksheet.Cells[index, i += 1].Value = "Network";
                worksheet.Cells[index, i += 1].Value = "Operation";
                worksheet.Cells[index, i += 1].Value = "Sub-Operation";
                worksheet.Cells[index, i += 1].Value = "Att/Abs";

                for (int a = 1;a <= days; a++)
                {
                    worksheet.Cells[index, i += 1].Value = a.ToString();
                }

                worksheet.Cells[index, i += 1].Value = "Short Text";
                worksheet.Cells[index, i += 1].Value = "Remarks";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Row Data
                index += 1;
               
                var CurrentUser = _userHelper.GetLoginUser(User);
                var Data = Service.ExportExcel(CurrentUser.Id, User, FirstDate, LastDate);
                foreach (var row in Data ?? new List<ExportExcelsDto>())
                {
                    if (row._firstDate > LastDate || row._lasttDate < FirstDate) continue;

                    int j = 0;

                    var JobStage = _jobstage.GetByActiveUser(row.ContractorId);

                    worksheet.Cells[index, j += 1].Value = row.ResourceId; // Personal Number
                    worksheet.Cells[index, j += 1].Value = row.Location; // Location
                    worksheet.Cells[index, j += 1].Value = JobStage == null ? "" : JobStage.Stage;// Activity Type ( jobstage)
                    worksheet.Cells[index, j += 1].Value = ""; // Order
                    worksheet.Cells[index, j += 1].Value = row.NetworkCode; // Network
                    worksheet.Cells[index, j += 1].Value = row.ActCode;  // Operation
                    worksheet.Cells[index, j += 1].Value = row.SubOpsCode; // Sub-Operation
                    worksheet.Cells[index, j += 1].Value = ""; // Att/Abs

                    DateTime start = new DateTime(FirstDate.Year, FirstDate.Month, FirstDate.Day)
                        .AddDays(-1);
                    while (start < LastDate)
                    {
                        start = start.AddDays(1);
                        if (start < FirstDate) continue;

                        //var date = day > 9 ? day.ToString() : "0" + day;                                              //goblog
                        //var month = dateMonth > 9 ? dateMonth.ToString() : "0" + dateMonth;                           //goblog
                        //var realDate = String.Format("{0}-{1}-{2}", dateYears, month, date);                          //goblog
                        //var Hours = _record.GetHours(row.Id, DateTime.Parse(realDate));                               //goblog
                        //var value = Hours == 0 ? "" : Hours.ToString();                                               //goblog
                        //worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(value) ? value + ".0" : ""; //goblog

                        var hours = _record.GetHours(row.Id, start);
                        worksheet.Cells[index, j += 1].Value = hours == 0 ? "" : hours.ToString("F1");
                    }

                    worksheet.Cells[index, j += 1].Value = ""; // Short Text
                    worksheet.Cells[index, j += 1].Value = ""; // Remarks
                    index++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();

            }
            return Redirect(URL);
        }

        public IActionResult Download2(string ValueDate)
        {
            var valueDate = ValueDate.Split('-');
            var valueMonth = valueDate[0];
            var valueYears = valueDate[1];

            var dateMonth = int.Parse(valueMonth);
            var dateYears = int.Parse(valueYears);
            int days = DateTime.DaysInMonth(dateYears, dateMonth);

            var FirstDate = new DateTime(dateYears, dateMonth, 1);
            var LastDate = new DateTime(dateYears, dateMonth, days);

            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/TimeSheetActivity.xlsx";
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
                int max = 31;
                int dist = max - days;

                // Header 1
                int numVal = 0;
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = dateYears.ToString();
                worksheet.Cells[index, numVal += 1].Value = "MONTH";
                worksheet.Cells[index, numVal += 1].Value = dateMonth.ToString();
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";

                for (int a = 1; a <= days; a++)
                {
                    var _date = a > 9 ? a.ToString() : "0" + a;
                    var _month = dateMonth > 9 ? dateMonth.ToString() : "0" + dateMonth;
                    var _dateHeader = String.Format("{0}-{1}-{2}", dateYears, _month, _date);
                    DateTime dateHeader = DateTime.Parse(_dateHeader);
                    worksheet.Cells[index, numVal += 1].Value = dateHeader.ToString("ddd").ToUpper();
                }

                for (int ds = 1; ds <= dist; ds++)
                {
                    worksheet.Cells[index, numVal += 1].Value = "";
                }

                worksheet.Cells[index, numVal += 1].Value = "";
                worksheet.Cells[index, numVal += 1].Value = "";

                // Header 2
                index += 1;
                worksheet.Cells[index, i += 1].Value = "Personal Number";
                worksheet.Cells[index, i += 1].Value = "Contractor Name";
                worksheet.Cells[index, i += 1].Value = "Location";
                worksheet.Cells[index, i += 1].Value = "Timesheet Type";
                //worksheet.Cells[index, i += 1].Value = "Order";
                worksheet.Cells[index, i += 1].Value = "Cost Center";
                worksheet.Cells[index, i += 1].Value = "Network";
                worksheet.Cells[index, i += 1].Value = "Operation";
                worksheet.Cells[index, i += 1].Value = "Sub-Operation";
                worksheet.Cells[index, i += 1].Value = "PM";
                worksheet.Cells[index, i += 1].Value = "PM Status";
                worksheet.Cells[index, i += 1].Value = "PM Approved Date";
                worksheet.Cells[index, i += 1].Value = "LM";
                worksheet.Cells[index, i += 1].Value = "LM Status";
                worksheet.Cells[index, i += 1].Value = "LM Approved Date";

                for (int a = 1; a <= days; a++)
                {
                    worksheet.Cells[index, i += 1].Value = a.ToString();
                }

                worksheet.Cells[index, i += 1].Value = "Short Text";
                worksheet.Cells[index, i += 1].Value = "Remarks";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Row Data
                index += 1;

                var CurrentUser = _userHelper.GetLoginUser(User);
                var Data = Service.ExportExcel(CurrentUser.Id, User, FirstDate, LastDate);
                foreach (var row in Data ?? new List<ExportExcelsDto>())
                {
                    if (row._firstDate > LastDate || row._lasttDate < FirstDate) continue;

                    int j = 0;

                    var JobStage = _jobstage.GetByActiveUser(row.ContractorId);

                    worksheet.Cells[index, j += 1].Value = row.ResourceId; // Personal Number
                    worksheet.Cells[index, j += 1].Value = row.ResourceName; //Name
                    worksheet.Cells[index, j += 1].Value = row.Location; // Location
                    worksheet.Cells[index, j += 1].Value = row.TimeSheetType; //TimeSheetType
                        //JobStage == null ? "" : JobStage.Stage;// Activity Type ( jobstage)
                    worksheet.Cells[index, j += 1].Value = row.CosCenterCode; // Costcenter
                    worksheet.Cells[index, j += 1].Value = row.NetworkCode; // Network
                    worksheet.Cells[index, j += 1].Value = row.ActCode;  // Operation
                    worksheet.Cells[index, j += 1].Value = row.SubOpsCode; // Sub-Operation
                    worksheet.Cells[index, j += 1].Value = row.ApprooveOneBy;
                    worksheet.Cells[index, j += 1].Value = row.PMStatus;
                    worksheet.Cells[index, j += 1].Value = row.ApprooveOneDateRemark;
                    worksheet.Cells[index, j += 1].Value = row.ApprooveTwoBy;
                    worksheet.Cells[index, j += 1].Value = row.LMStatus;
                    worksheet.Cells[index, j += 1].Value = row.ApprooveTwoDateRemark;

                    DateTime start = new DateTime(FirstDate.Year, FirstDate.Month, FirstDate.Day)
                        .AddDays(-1);
                    while (start < LastDate)
                    {
                        start = start.AddDays(1);
                        if (start < FirstDate) continue;

                        //var date = day > 9 ? day.ToString() : "0" + day;                                              //goblog
                        //var month = dateMonth > 9 ? dateMonth.ToString() : "0" + dateMonth;                           //goblog
                        //var realDate = String.Format("{0}-{1}-{2}", dateYears, month, date);                          //goblog
                        //var Hours = _record.GetHours(row.Id, DateTime.Parse(realDate));                               //goblog
                        //var value = Hours == 0 ? "" : Hours.ToString();                                               //goblog
                        //worksheet.Cells[index, j += 1].Value = !string.IsNullOrWhiteSpace(value) ? value + ".0" : ""; //goblog

                        var hours = _record.GetHours(row.Id, start);
                        worksheet.Cells[index, j += 1].Value = hours == 0 ? "" : hours.ToString("F1");
                    }

                    worksheet.Cells[index, j += 1].Value = ""; // Short Text
                    worksheet.Cells[index, j += 1].Value = ""; // Remarks
                    index++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();

            }
            return Redirect(URL);
        }
    }

}