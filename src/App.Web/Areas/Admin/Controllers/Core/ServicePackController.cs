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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.Areas.Admin.Controllers.Core
{
    /// <summary>
    /// SSOW
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ServicePackController : BaseController<ServicePack, IServicePackService, ServicePackViewModel, ServicePackModelForm, Guid>
    {
        private readonly IServicePackCategoryService servicePackCategoryService;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public ServicePackController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IServicePackService service,
            IUserHelper userHelper, 
            IHostingEnvironment environment,
            ExcelHelper excel,
            IServicePackCategoryService servicePackCategoryService) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.servicePackCategoryService = servicePackCategoryService;
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        public override IActionResult Create()
        {
            ViewBag.Category = servicePackCategoryService.GetAll().Select(x => new {x.Id, x.Name});
            ViewBag.ListPackageType = Enum.GetValues(typeof(PackageTypes)).Cast<PackageTypes>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.Category = servicePackCategoryService.GetAll().Select(x => new { x.Id, x.Name });
            ViewBag.ListPackageType = Enum.GetValues(typeof(PackageTypes)).Cast<PackageTypes>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var catId = Service.GetById(id).ServicePackCategoryId;
            ViewBag.Category = servicePackCategoryService.GetById(catId);
            return base.Details(id);
        }

        protected override void CreateData(ServicePack item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(ServicePack item, ServicePackModelForm model)
        {
            item.Type = model.Type;
            item.Name = model.Name;
            item.Code = model.Code;
            item.Rate = model.Rate;
            item.Hourly = model.Hourly;
            item.Otp20 = model.Otp20;
            item.Otp30 = model.Otp30;
            item.Otp40 = model.Otp40;
            item.Laptop = model.Laptop;
            item.Usin = model.Usin;
            item.Status = model.Status;
            item.ServicePackCategoryId = model.ServicePackCategoryId;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
            item.ServiceCode = model.ServiceCode;
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
                                if (worksheet.Cells[row, 1].Value != null &&
                                    worksheet.Cells[row, 2].Value != null &&
                                    worksheet.Cells[row, 3].Value != null &&
                                    worksheet.Cells[row, 4].Value != null &&
                                    worksheet.Cells[row, 5].Value != null &&
                                    worksheet.Cells[row, 6].Value != null &&
                                    worksheet.Cells[row, 7].Value != null &&
                                    worksheet.Cells[row, 8].Value != null &&
                                    worksheet.Cells[row, 9].Value != null &&
                                    worksheet.Cells[row, 10].Value != null &&
                                    worksheet.Cells[row, 11].Value != null)
                                {

                                    var Type = worksheet.Cells[row, 1].Value;
                                    var Package = worksheet.Cells[row, 2].Value;
                                    var SSOName = worksheet.Cells[row, 3].Value;
                                    var ServiceCode = worksheet.Cells[row, 4].Value;
                                    var MontlyRate = worksheet.Cells[row, 5].Value;
                                    var HourlyRate = worksheet.Cells[row, 6].Value;
                                    var SumTwo = worksheet.Cells[row, 7].Value;
                                    var SumThree = worksheet.Cells[row, 8].Value;
                                    var SumFour = worksheet.Cells[row, 9].Value;
                                    var LaptopAllowance = worksheet.Cells[row, 10].Value;
                                    var USIM = worksheet.Cells[row, 11].Value;

                                  

                                    ServicePackCategory Category = servicePackCategoryService
                                      .GetAll()
                                      .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(Package.ToString()));

                                    if(Category==null)
                                    {
                                        ServicePackCategory NewCategory = new ServicePackCategory { Name = Package.ToString() };
                                        Category =  servicePackCategoryService.Add(NewCategory);
                                    }

                                    //ServicePack FindSSO = Service
                                    // .GetAll()
                                    // .FirstOrDefault(x =>
                                    //      x.Type == (PackageTypes)Enum.Parse(typeof(PackageTypes), Type.ToString()) &&
                                    //      x.ServicePackCategory == Category &&
                                    //      _excel.TruncateString(x.Name) == _excel.TruncateString(SSOName.ToString()) &&
                                    //      _excel.TruncateString(x.Code) == _excel.TruncateString(ServiceCode.ToString()) &&
                                    //      _excel.TruncateString(x.Rate.ToString()) == _excel.TruncateString(MontlyRate.ToString()) &&
                                    //      _excel.TruncateString(x.Hourly.ToString()) == _excel.TruncateString(HourlyRate.ToString()) &&
                                    //      _excel.TruncateString(x.Otp20.ToString()) == _excel.TruncateString(SumTwo.ToString()) &&
                                    //      _excel.TruncateString(x.Otp30.ToString()) == _excel.TruncateString(SumThree.ToString()) &&
                                    //      _excel.TruncateString(x.Otp40.ToString()) == _excel.TruncateString(SumFour.ToString()) &&
                                    //      _excel.TruncateString(x.Laptop.ToString()) == _excel.TruncateString(LaptopAllowance.ToString()) &&
                                    //      _excel.TruncateString(x.Usin.ToString()) == _excel.TruncateString(USIM.ToString()));
                                        
                                    //if (FindSSO == null)
                                    //{
                                    //    ServicePack Sp = new ServicePack
                                    //    {
                                    //        Type = (PackageTypes)Enum.Parse(typeof(PackageTypes), Type.ToString()),
                                    //        ServicePackCategory = Category,
                                    //        Name = SSOName.ToString(),
                                    //        Code = ServiceCode.ToString(),
                                    //        Rate = decimal.Parse(MontlyRate.ToString()),
                                    //        Hourly = decimal.Parse(HourlyRate.ToString()),
                                    //        Otp20 = decimal.Parse(SumTwo.ToString()),
                                    //        Otp30 = decimal.Parse(SumThree.ToString()),
                                    //        Otp40 = decimal.Parse(SumFour.ToString()),
                                    //        Laptop = decimal.Parse(LaptopAllowance.ToString()),
                                    //        Usin = decimal.Parse(USIM.ToString()),
                                    //    };
                                    //    Service.Add(Sp);
                                    //    TOTAL_INSERT++;
                                    //}

                                    ServicePack Sp = new ServicePack
                                    {
                                        Type = (PackageTypes)Enum.Parse(typeof(PackageTypes), Type.ToString()),
                                        ServicePackCategory = Category,
                                        Name = SSOName.ToString(),
                                        Code = ServiceCode.ToString(),
                                        Rate = decimal.Parse(MontlyRate.ToString()),
                                        Hourly = decimal.Parse(HourlyRate.ToString()),
                                        Otp20 = decimal.Parse(SumTwo.ToString()),
                                        Otp30 = decimal.Parse(SumThree.ToString()),
                                        Otp40 = decimal.Parse(SumFour.ToString()),
                                        Laptop = decimal.Parse(LaptopAllowance.ToString()),
                                        Usin = decimal.Parse(USIM.ToString()),
                                    };
                                    Service.Add(Sp);
                                    TOTAL_INSERT++;

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
