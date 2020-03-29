using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AllowanceListController : BaseController<AllowanceList, IAllowanceListService, AllowanceListViewModel, AllowanceListModelForm, Guid>
    {
        protected readonly IServicePackService servicePack;
        protected readonly IServicePackCategoryService ServicePackCategory;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly IUserHelper _userHelper;

        public AllowanceListController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IAllowanceListService service, 
            IUserHelper userHelper, 
            IServicePackService servicePack,
            IHostingEnvironment environment,
            ExcelHelper excel,
            IServicePackCategoryService ServicePackCategory) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.servicePack = servicePack;
            this.ServicePackCategory = ServicePackCategory;
             _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        public override IActionResult Create()
        {
            ViewBag.ServicePacks = servicePack.GetAll().Where(x=>x.Type == PackageTypes.A || x.Type == PackageTypes.B);
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            var servicePackId = Service.GetById(id).ServicePackId;
            var servicePacks = servicePack.GetAll(x => x.ServicePackCategory).Single(x => x.Id.Equals(servicePackId));
            var servicePacksCats = ServicePackCategory.GetAll().Single(x => x.Id == servicePacks.ServicePackCategoryId);
            ViewBag.ServicePacks = servicePacks;
            ViewBag.ServicePackCategories = servicePacksCats;
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var servicePackId = Service.GetById(id).ServicePackId;
            var servicePacks = servicePack.GetAll(x=>x.ServicePackCategory).Single(x => x.Id.Equals(servicePackId));
            var servicePacksCats = ServicePackCategory.GetAll().Single(x => x.Id == servicePacks.ServicePackCategoryId);
            ViewBag.ServicePacks = servicePacks;
            ViewBag.ServicePackCategories = servicePacksCats;
            return base.Details(id);
        }

        protected override void CreateData(AllowanceList item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }


        protected override void UpdateData(AllowanceList item, AllowanceListModelForm model)
        {
            item.AllowanceStatus = model.AllowanceStatus;
            item.DataToken = model.DataToken;
            item.ServicePackId = model.ServicePackId;
            item.OnCallHoliday = model.OnCallHoliday;
            item.ShiftHoliday = model.ShiftHoliday;
            item.OnCallNormal = model.OnCallNormal;
            item.ShiftNormal = model.ShiftNormal;
            item.GrantedHoliday14 = model.GrantedHoliday14;
            item.AllowanceNote = model.AllowanceNote;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
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
                                    worksheet.Cells[row, 7].Value != null)
                                {

                                    var Type = worksheet.Cells[row, 1].Value.ToString();
                                    var ServicePakcCategory = worksheet.Cells[row, 2].Value.ToString();
                                    var SSOW = worksheet.Cells[row, 3].Value.ToString();
                                    var OnCallNormal = worksheet.Cells[row, 4].Value != null ? worksheet.Cells[row, 4].Value.ToString() : "0";
                                    var OnShiftNormal = worksheet.Cells[row, 5].Value != null ? worksheet.Cells[row, 5].Value.ToString() : "0";
                                    var OnCallHoliday = worksheet.Cells[row, 6].Value != null ? worksheet.Cells[row, 6].Value.ToString() : "0";
                                    var OnShiftHoliday = worksheet.Cells[row, 7].Value != null ? worksheet.Cells[row, 7].Value.ToString() : "0";
                                    var Remark = worksheet.Cells[row, 8].Value.ToString();


                                    if(Type!=null)
                                    {
                                       
                                        if (_excel.TruncateString(Type).Equals(_excel.TruncateString("A")) ||
                                            _excel.TruncateString(Type).Equals(_excel.TruncateString("B")) ||
                                            _excel.TruncateString(Type).Equals(_excel.TruncateString("FSO")))
                                        {
                                          
                                            ServicePackCategory Category = ServicePackCategory
                                               .GetAll()
                                               .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(ServicePakcCategory));

                                            ServicePack SerPack = servicePack.GetAll()
                                              .Where(x => _excel.TruncateString(x.Name) == _excel.TruncateString(SSOW) && x.ServicePackCategory == Category)
                                              .FirstOrDefault();

                                            if(SerPack!=null)
                                            {
                                                AllowanceList inAllowanceList = new AllowanceList
                                                {
                                                    ServicePack = SerPack,
                                                    OnCallNormal = decimal.Parse(OnCallNormal),
                                                    OnCallHoliday = decimal.Parse(OnCallHoliday),
                                                    ShiftNormal = decimal.Parse(OnShiftNormal),
                                                    ShiftHoliday = decimal.Parse(OnShiftHoliday),
                                                    OtherInfo = Remark
                                                };
                                                Service.Add(inAllowanceList);
                                                TOTAL_INSERT++;
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

            TempData["Messages"] = "Total Inserted   " + TOTAL_INSERT;
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index");
        }
    }

}
