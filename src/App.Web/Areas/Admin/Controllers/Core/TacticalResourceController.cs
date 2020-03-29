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

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class TacticalResourceController : BaseController<TacticalResource, ITacticalResourceService, TacticalResourceViewModel, TacticalResourceFormModel, Guid>
    {
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly ConfigHelper _config;
        private readonly IDepartementService _dept;
        private readonly IDepartementSubService _deptSub;
        private readonly ISrfRequestService _srf;
        private readonly IUserHelper _userHelper;

        public TacticalResourceController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ITacticalResourceService service,
            IHostingEnvironment environment,
            IDepartementService dept,
            IDepartementSubService deptSub,
            ExcelHelper excel,
            ConfigHelper config,
            ISrfRequestService srf,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _environment = environment;
            _excel = excel;
            _config = config;
            _dept = dept;
            _deptSub = deptSub;
            _srf = srf;
            _userHelper = userHelper;
        }

        public override IActionResult Index()
        {
            var FirstRecord = _srf.GetAll().Where(x => x.SrfBegin.HasValue).OrderBy(x => x.SrfBegin).FirstOrDefault();
            var FirstYears = FirstRecord.SrfBegin.Value.Year;
            var LastRecord = _srf.GetAll().Where(x => x.SrfEnd.HasValue).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            var LastYears = LastRecord.SrfEnd.Value.Year;
            var OptionYears = "<option selected disabled>-- Select Years--</option>";
            for (int i = LastYears; i >= FirstYears; i--)
            {
                OptionYears += "<option value='" + LastYears + "'>" + LastYears + "</option>";
                LastYears += -1;
            }
            var OptionsMonths = "<option selected disabled>-- Select Months--</option>";
            for (int j = 1; j <= 12; j++)
            {
                var Value = (j > 9) ? j.ToString() : "0" + j;
                OptionsMonths += "<option value='" + Value + "'>" + Value + "</option>";
            }
            ViewBag.MonthsOption = OptionsMonths;
            ViewBag.YearsOption = OptionYears;
            return base.Index();
        }


        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
        
            var uploads = System.IO.Path.Combine(_environment.WebRootPath, "temp");
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
                                    worksheet.Cells[row, 2].Value != null &&
                                    worksheet.Cells[row, 3].Value != null &&
                                    worksheet.Cells[row, 4].Value != null)
                                {

                                    var Dept = worksheet.Cells[row, 1].Value.ToString();
                                    var Approved = worksheet.Cells[row, 2].Value.ToString();
                                    var Forecast = worksheet.Cells[row, 3].Value.ToString();
                                    var DateSrf = worksheet.Cells[row, 4].Value.ToString();

                                    #region Department
                                    var Department = _dept.GetAll().Where(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Dept)).FirstOrDefault();
                                    if(Department != null)
                                    {
                                        var TcDept = Service.GetAll().Where(x => x.DepartmentId.Equals(Department.Id)).FirstOrDefault();
                                        if(TcDept == null)
                                        {
                                            TacticalResource tp = new TacticalResource();
                                            tp.Approved = int.Parse(Approved);
                                            tp.Forecast = int.Parse(Forecast);
                                            tp.DateSrf = DateTime.Parse(DateSrf);
                                            tp.DepartmentId = Department.Id;
                                            tp.CreatedAt = DateTime.Now;
                                            tp.CreatedBy = _userHelper.GetUserId(User);
                                            tp.CountSrf = 0;
                                            Service.Add(tp);
                                        }
                                        else
                                        {
                                            TcDept.Approved = int.Parse(Approved);
                                            TcDept.Forecast = int.Parse(Forecast);
                                            TcDept.DateSrf = DateTime.Parse(DateSrf);
                                            TcDept.LastUpdateTime = DateTime.Now;
                                            TcDept.LastEditedBy = _userHelper.GetUserId(User);
                                            TcDept.CountSrf = 0;
                                            Service.Update(TcDept);
                                        }
                                    }
                                    #endregion



                                    #region DepartmentSub
                                    var DepartmentSub = _deptSub.GetAll().Where(x => _excel.TruncateString(x.SubName) == _excel.TruncateString(Dept)).FirstOrDefault();
                                    if (DepartmentSub != null)
                                    {
                                        var TcDept = Service.GetAll().Where(x => x.DepartmentSubId.Equals(DepartmentSub.Id)).FirstOrDefault();
                                        if (TcDept == null)
                                        {
                                            TacticalResource tp = new TacticalResource();
                                            tp.Approved = int.Parse(Approved);
                                            tp.Forecast = int.Parse(Forecast);
                                            tp.DateSrf = DateTime.Parse(DateSrf);
                                            tp.DepartmentId = DepartmentSub.DepartmentId;
                                            tp.DepartmentSubId = DepartmentSub.Id;
                                            tp.CountSrf = 0;
                                            Service.Add(tp);
                                        }
                                        else
                                        {
                                            TcDept.Approved = int.Parse(Approved);
                                            TcDept.Forecast = int.Parse(Forecast);
                                            TcDept.DateSrf = DateTime.Parse(DateSrf);
                                            TcDept.CountSrf = 0;
                                            Service.Update(TcDept);
                                        }
                                    }
                                    #endregion


                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Messages"] = ex.ToString();
                }
            }

            TempData["Messages"] = "Data has been updated";
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");

        }


        public IActionResult Template()
        {
            var Check = Service.GetAll().ToList();
            if(Check.Any())
            {
                var Template = CurrentTemplate();
                return Redirect(Template);
            }
            else
            {
                var Template = NewTemplate();
                return Redirect(Template);
            }
        }

        public string CurrentTemplate()
        {
            string sWebRootFolder = _environment.WebRootPath;
            string sFileName = @"report/TRCP_TEMPLATE.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Organization / Sub Organization";
                worksheet.Cells[index, i += 1].Value = "TRCP Approved";
                worksheet.Cells[index, i += 1].Value = "TRCP Forecast";
                worksheet.Cells[index, i += 1].Value = "Date SRF";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }


                index += 1;
                var Data = Service.GetAll().ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        if (row.DepartmentId.HasValue && row.DepartmentSubId==null)
                        {
                            var Temp = _dept.GetById(row.DepartmentId);
                            worksheet.Cells[index, 1].Value = Temp.Name;
                        }
                        else
                        {
                            var Temp = _deptSub.GetById(row.DepartmentSubId);
                            worksheet.Cells[index, 1].Value = Temp.SubName;
                        }
                        worksheet.Cells[index, 2].Value = row.Approved;
                        worksheet.Cells[index, 3].Value = row.Forecast;
                        if (row.DateSrf.HasValue)
                        {
                            worksheet.Cells[index, 4].Value = row.DateSrf.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            worksheet.Cells[index, 4].Value = "";
                        }
                        index++;
                    }

                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return URL;
        }

        public string NewTemplate()
        {
          
            string sWebRootFolder = _environment.WebRootPath;
            string sFileName = @"report/TRCP_TEMPLATE.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Organization / Sub Organization";
                worksheet.Cells[index, i += 1].Value = "TRCP Approved";
                worksheet.Cells[index, i += 1].Value = "TRCP Forecast";
                worksheet.Cells[index, i += 1].Value = "Date SRF";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;
                var Data = _dept.GetAll().OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.OperateOrNon }).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {

                      
                        worksheet.Cells[index, 1].Value = row.Name.ToUpper();
                        worksheet.Cells[index, 2].Value = "0";
                        worksheet.Cells[index, 3].Value = "0";
                        worksheet.Cells[index, 4].Value = "";
                        index++;

                        var Sub = _deptSub.GetAll().Where(x => x.DepartmentId == row.Id).ToList();
                        if (Sub.Any())
                        {
                            foreach (var sb in Sub)
                            {

                                worksheet.Cells[index, 1].Value = sb.SubName.ToUpper();
                                worksheet.Cells[index, 2].Value = "0";
                                worksheet.Cells[index, 3].Value = "0";
                                worksheet.Cells[index, 4].Value = "";
                                index++;

                            }
                        }

                    }

                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return URL;
        }

    }
}
