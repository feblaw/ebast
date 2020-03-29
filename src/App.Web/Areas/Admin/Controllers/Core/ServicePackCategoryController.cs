using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace App.Web.Areas.Admin.Controllers.Core
{
    /// <summary>
    /// SSOW category
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ServicePackCategoryController : BaseController<ServicePackCategory, IServicePackCategoryService, ServicePackCategoryViewModel, ServicePackCategoryModelForm, Guid>
    {
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;

        public ServicePackCategoryController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IServicePackCategoryService service,
            IHostingEnvironment environment,
            ExcelHelper excel,
            IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _environment = environment;
            _excel = excel;
        }

        protected override void CreateData(ServicePackCategory item)
        {
            item.Name = item.Name.ToUpper();
        }

        protected override void UpdateData(ServicePackCategory item, ServicePackCategoryModelForm model)
        {
            item.Name = model.Name.ToUpper();
            item.Level = model.Level;
            item.Status = model.Status;
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            int TOTAL_INSERT = 0;
            int TOTAL_UPDATE = 0;
    
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
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                                {
                                    int number;
                                    bool CheckLevel = Int32.TryParse(worksheet.Cells[row, 2].Value.ToString(), out number);
                                    if(CheckLevel)
                                    {
                                        var CategoryName = worksheet.Cells[row, 1].Value;
                                        var CategoryLevel = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                                        if(CategoryLevel==0 || CategoryLevel==1)
                                        {
                                            Level lv = (Level)Enum.ToObject(typeof(Level), CategoryLevel);
                                            ServicePackCategory SpFind = Service
                                                .GetAll()
                                                .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(CategoryName.ToString()));
                                            if (SpFind == null)
                                            {
                                                ServicePackCategory sp = new ServicePackCategory { Name = CategoryName.ToString(), Level = lv };
                                                Service.Add(sp);
                                                TOTAL_INSERT++;
                                            }
                                            else
                                            {
                                              
                                                ServicePackCategory sp = Service.GetById(SpFind.Id);
                                                sp.Name = CategoryName.ToString();
                                                sp.Level = lv;
                                                Service.Update(sp);
                                                TOTAL_UPDATE++;
                                            }
                                        }
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

            TempData["Messages"] = "Total Inserted = " + TOTAL_INSERT + " , Total Updated = " + TOTAL_UPDATE;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }

    }
}
