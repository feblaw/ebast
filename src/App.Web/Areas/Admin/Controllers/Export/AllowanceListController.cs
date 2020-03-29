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
using App.Domain.Models.Enum;
using OfficeOpenXml.Style;

namespace App.Web.Areas.Admin.Controllers.Export
{

    [Area("Export")]
    [Authorize]
    public class AllowanceListController : BaseController<AllowanceList, IAllowanceListService, AllowanceListViewModel, AllowanceListModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        public AllowanceListController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment env,
            IAllowanceListService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
        }

        public override IActionResult Index()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/AllowanceList.xlsx";
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
                worksheet.Cells[1, 1].Value = "Type";
                worksheet.Cells[1, 2].Value = "Service Packages Category";
                worksheet.Cells[1, 3].Value = "SSOW";
                worksheet.Cells[1, 4, 1, 5].Merge = true;
                worksheet.Cells[1, 4, 1, 5].Value = "Normal Allowance";
                worksheet.Cells[1, 4, 1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 6, 1, 7].Merge = true;
                worksheet.Cells[1, 6, 1, 7].Value = "Lebaran/Christmas/Granted Holiday";
                worksheet.Cells[1, 6, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 8].Value = "Allowance Note";

                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 2].Value = "";
                worksheet.Cells[2, 3].Value = "";
                worksheet.Cells[2, 4].Value = "On Call Allowance";
                worksheet.Cells[2, 5].Value = "Shift Allowance";
                worksheet.Cells[2, 6].Value = "On Call Allowance";
                worksheet.Cells[2, 7].Value = "Shift Allowance";
                worksheet.Cells[2, 8].Value = "";

             


                for (int ii = 1;ii<=2;ii++)
                {
                    for(int jj = 1;jj<=8;jj++)
                    {
                        worksheet.Cells[ii, jj].Style.Font.Bold = true;
                        worksheet.Cells[ii, jj].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[ii, jj].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                        worksheet.Cells[ii, jj].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                }

                int index = 3;
                Expression<Func<AllowanceList, object>>[] Includes = new Expression<Func<AllowanceList, object>>[2];
                Includes[0] = pack => pack.ServicePack;
                Includes[1] = pack => pack.ServicePack.ServicePackCategory;

                var Data = Service.GetAll(Includes).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(PackageTypes), row.ServicePack.Type).ToString();
                        worksheet.Cells[index, j += 1].Value = row.ServicePack.ServicePackCategory.Name;
                        worksheet.Cells[index, j += 1].Value = row.ServicePack.Name;
                        worksheet.Cells[index, j += 1].Value = row.OnCallNormal.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.ShiftNormal.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.OnCallHoliday.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.ShiftHoliday.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.AllowanceNote;
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
