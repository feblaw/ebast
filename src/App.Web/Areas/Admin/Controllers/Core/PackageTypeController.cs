using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class PackageTypeController : BaseController<PackageType, IPackageTypeService, PackageTypeViewModel, PackageTypeyModelForm, Guid>
    {
        private readonly IPackageTypeService service;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;

        public PackageTypeController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IHostingEnvironment environment,
            IPackageTypeService service,
            ExcelHelper excel,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.service = service;
            _environment = environment;
            _excel = excel;
        }

        protected override void CreateData(PackageType item)
        {
            item.Name = item.Name.ToUpper();
        }

        protected override void UpdateData(PackageType item, PackageTypeyModelForm model)
        {
            item.Name = model.Name.ToUpper();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
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
                                if (worksheet.Cells[row, 1].Value != null)
                                {
                                    var PackageName = worksheet.Cells[row, 1].Value;
                                    var Check = service.GetAll().FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(PackageName.ToString()));
                                    if (Check == null)
                                    {
                                        PackageType p = new PackageType { Name = PackageName.ToString().ToUpper() };
                                        service.Add(p);
                                        TOTAL_INSERT++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }
    }
}
