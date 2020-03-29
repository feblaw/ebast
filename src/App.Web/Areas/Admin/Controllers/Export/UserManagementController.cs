using App.Domain.Models.Identity;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Linq.Expressions;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class UserManagementController : BaseController<ApplicationUser, IUserService, ApplicationUserViewModel, ApplicationUserForm, string>
    {
        private readonly IHostingEnvironment _env;
        private readonly IUserProfileService _profile;

        public UserManagementController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment env,
            IUserProfileService profile,
            IUserService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
            _profile = profile;
        }

        public override IActionResult Index()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/UserManagement.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "User Name";
                worksheet.Cells[index, i += 1].Value = "Access Level";
                worksheet.Cells[index, i += 1].Value = "Status";
                worksheet.Cells[index, i += 1].Value = "Company";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;

                var Data = _profile.GetAll().Where(x=>x.Roles.Trim().ToLower() != "Contractor".Trim().ToLower()).OrderBy(x=>x.Name).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        var AppUsers = Service.GetById(row.ApplicationUserId);
                        if(AppUsers != null)
                        {
                            worksheet.Cells[index, j += 1].Value = row.Name;
                            worksheet.Cells[index, j += 1].Value = AppUsers.UserName;
                            worksheet.Cells[index, j += 1].Value = row.Roles;
                            worksheet.Cells[index, j += 1].Value = row.IsActive == true ? "Active" : "Close";
                            worksheet.Cells[index, j += 1].Value = row.ASP.Name;
                            index++;
                        }
                    }
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }
            return Redirect(URL);
        }
    }
}
