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

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class ServicePackController : BaseController<ServicePack, IServicePackService, ServicePackViewModel, ServicePackModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        public ServicePackController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment env,
            IServicePackService service, 
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
        }

        public override IActionResult Index()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/ServicePack.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Type";
                worksheet.Cells[index, i += 1].Value = "Service Pack Category";
                worksheet.Cells[index, i += 1].Value = "SSOW";
                worksheet.Cells[index, i += 1].Value = "Service Code";
                worksheet.Cells[index, i += 1].Value = "Monthly Rate";
                worksheet.Cells[index, i += 1].Value = "Hourly Rate";
                worksheet.Cells[index, i += 1].Value = "OT Package Lump Sum (20)";
                worksheet.Cells[index, i += 1].Value = "OT Package Lump Sum (30)";
                worksheet.Cells[index, i += 1].Value = "OT Package Lump Sum (40)";
                worksheet.Cells[index, i += 1].Value = "Laptop Allowance";
                worksheet.Cells[index, i += 1].Value = "USIM Broadband";
             

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;

                Expression<Func<ServicePack, object>>[] Includes = new Expression<Func<ServicePack, object>>[1];
                Includes[0] = pack => pack.ServicePackCategory;

                var Data = Service.GetAll(Includes).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = Enum.GetName(typeof(PackageTypes), row.Type).ToString();
                        worksheet.Cells[index, j += 1].Value = row.ServicePackCategory.Name;
                        worksheet.Cells[index, j += 1].Value = row.Name;
                        worksheet.Cells[index, j += 1].Value = row.Code;
                        worksheet.Cells[index, j += 1].Value = row.Rate.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Hourly.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Otp20.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Otp30.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Otp40.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Laptop.ToString("#,##0");
                        worksheet.Cells[index, j += 1].Value = row.Usin.ToString("#,##0");
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
