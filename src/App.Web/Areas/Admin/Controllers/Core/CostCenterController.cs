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

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    public class CostCenterController : BaseController<CostCenter, ICostCenterService, CostCenterViewModel, CostCenterModelForm, Guid>
    {
        private readonly IDepartementService departement;
        private readonly IHostingEnvironment _environment;
        private readonly IUserHelper _userHelper;
        private readonly ExcelHelper _excel;


        public CostCenterController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ICostCenterService service,
            IUserHelper userHelper,
            IHostingEnvironment environment,
            ExcelHelper excel,
            IDepartementService departement) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.departement = departement;
            _environment = environment;
            _excel = excel;
            _userHelper = userHelper;
        }

        public override IActionResult Create()
        {
            ViewBag.Departements = departement.GetAll().ToList();
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.Departements = departement.GetAll().ToList();
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var costCenter = Service.GetById(id);
            ViewBag.Departements = departement.GetAll().Single(x=>x.Id == costCenter.DepartmentId);
            return base.Details(id);
        }

        protected override void CreateData(CostCenter item)
        {
            item.Status = item.Status;
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
        }

        protected override void UpdateData(CostCenter item, CostCenterModelForm model)
        {
            item.Code = model.Code;
            item.Description = model.Description;
            item.Status = model.Status;
            item.DepartmentId = model.DepartmentId;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
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
                                if (worksheet.Cells[row, 1].Value != null &&
                                    worksheet.Cells[row, 2].Value != null && 
                                    worksheet.Cells[row, 3].Value != null)
                                {
                                    var CostCenterCode = worksheet.Cells[row, 1].Value;
                                    var OrganizationUnit = worksheet.Cells[row, 2].Value;
                                    var Description = worksheet.Cells[row, 3].Value;

                                    CostCenter CostFind = Service
                                     .GetAll()
                                     .FirstOrDefault(x => _excel.TruncateString(x.Code) == _excel.TruncateString(CostCenterCode.ToString()));

                                    Departement DepartmentFind = departement
                                     .GetAll()
                                     .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(OrganizationUnit.ToString()));

                                    if(DepartmentFind!=null)
                                    {
                                        if (CostFind == null)
                                        {
                                            CostCenter cs = new CostCenter { Code = CostCenterCode.ToString(), Departement = DepartmentFind, Description = Description.ToString() };
                                            Service.Add(cs);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {
                                            CostCenter cd = Service.GetById(CostFind.Id);
                                            cd.Departement = DepartmentFind;
                                            cd.Description = Description.ToString();
                                            Service.Update(cd);
                                            TOTAL_UPDATE++;
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
