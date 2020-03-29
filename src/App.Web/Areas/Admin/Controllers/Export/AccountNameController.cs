using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using OfficeOpenXml;
using App.Domain.Models.Core;
using System.Linq.Expressions;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class AccountNameController : BaseController<AccountName, IAccountNameService, AccountNameViewModel, AccountNameModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;
        public AccountNameController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper, 
            IAccountNameService service, IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
        }

        public override IActionResult Index()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/AccuntName.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Name";
                worksheet.Cells[index, i += 1].Value = "Owner";
                worksheet.Cells[index, i += 1].Value = "Status";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                Expression<Func<AccountName, object>>[] Includes = new Expression<Func<AccountName, object>>[1];
                Includes[0] = pack => pack.Coms;

                index += 1;
                var Data = Service.GetAll(Includes).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.Name;
                        worksheet.Cells[index, j += 1].Value = row.Coms.Name;
                        worksheet.Cells[index, j += 1].Value = row.Status == true ? "Active" : "Close";
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
