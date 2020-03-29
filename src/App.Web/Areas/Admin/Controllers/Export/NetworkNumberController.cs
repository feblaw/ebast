﻿using App.Domain.Models.Core;
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
using System.IO;
using OfficeOpenXml;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace App.Web.Areas.Admin.Controllers.Export
{
    [Area("Export")]
    [Authorize]
    public class NetworkNumberController : BaseController<NetworkNumber, INetworkNumberService, NetworkNumberViewModel, NetworkNumberModelForm, Guid>
    {
        private readonly IHostingEnvironment _env;

        public NetworkNumberController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            INetworkNumberService service,
            IHostingEnvironment env,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _env = env;
        }

        public override IActionResult Index()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"report/NetworkNumber.xlsx";
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
                worksheet.Cells[index, i += 1].Value = "Network Number";
                worksheet.Cells[index, i += 1].Value = "NN Description";
                worksheet.Cells[index, i += 1].Value = "Organization Unit";
                worksheet.Cells[index, i += 1].Value = "Project Name";
                worksheet.Cells[index, i += 1].Value = "Total Project Manager";
                worksheet.Cells[index, i += 1].Value = "Project Manager";
                worksheet.Cells[index, i += 1].Value = "Account Name";
                worksheet.Cells[index, i += 1].Value = "Status";

                for (int i_temp = 1; i_temp <= i; i_temp++)
                {
                    worksheet.Cells[index, i_temp].Style.Font.Bold = true;
                    worksheet.Cells[index, i_temp].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[index, i_temp].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    worksheet.Cells[index, i_temp].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                index += 1;

                Expression<Func<NetworkNumber, object>>[] Includes = new Expression<Func<NetworkNumber, object>>[5];
                Includes[0] = pack => pack.Departement;
                Includes[1] = pack => pack.Project;
                Includes[2] = pack => pack.AccountName;
                Includes[3] = pack => pack.LineManager;
                Includes[4] = pack => pack.ProjectManager;

                var Data = Service.GetAll(Includes).ToList();
                if (Data != null)
                {
                    foreach (var row in Data)
                    {
                        int j = 0;
                        worksheet.Cells[index, j += 1].Value = row.Code;
                        worksheet.Cells[index, j += 1].Value = row.Description;
                        worksheet.Cells[index, j += 1].Value = row.Departement.Name;
                        worksheet.Cells[index, j += 1].Value = row.Project.Name;
                        worksheet.Cells[index, j += 1].Value = row.LineManager.Name;
                        worksheet.Cells[index, j += 1].Value = row.Project.Name;
                        worksheet.Cells[index, j += 1].Value = row.AccountName.Name;
                        worksheet.Cells[index, j += 1].Value = row.IsClosed == true ? "Close" : "Active";
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
