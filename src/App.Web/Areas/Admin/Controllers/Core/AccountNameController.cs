using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AccountNameController : BaseController<AccountName, IAccountNameService, AccountNameViewModel, AccountNameModelForm, Guid>
    {
        private readonly IUserProfileService _ownerName;
        private readonly IHostingEnvironment _environment;
        private readonly ExcelHelper _excel;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;

        public AccountNameController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IAccountNameService service,
            IHostingEnvironment environment,
            ExcelHelper excel,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper, IUserProfileService ownerName) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _ownerName = ownerName;
            _environment = environment;
            _excel = excel;
            _userManager = userManager;
            _userHelper = userHelper;
        }

        public override IActionResult Create()
        {
            ViewBag.Owner = _ownerName.GetAll().OrderBy(x => x.Name).ToList();
            return base.Create();
        }

        public override IActionResult Edit(Guid id)
        {
            ViewBag.Owner = _ownerName.GetAll().OrderBy(x => x.Name).ToList();
            return base.Edit(id);
        }

        public override IActionResult Details(Guid id)
        {
            var accountName = Service.GetById(id);
            ViewBag.Owner = _ownerName.GetAll().Single(x => x.Id == accountName.Com);
            return base.Details(id);
        }

        protected override void UpdateData(AccountName item, AccountNameModelForm model)
        {
            item.Name = model.Name;
            item.Status = model.Status;
            item.Com = model.Com;
            item.LastEditedBy = _userHelper.GetUserId(User);
            item.LastUpdateTime = DateTime.Now;
        }

        protected override void CreateData(AccountName item)
        {
            item.CreatedBy = _userHelper.GetUserId(User);
            item.CreatedAt = DateTime.Now;
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
                                   
                                    var AccountName = worksheet.Cells[row, 1].Value;
                                    var Status = worksheet.Cells[row, 2].Value.ToString();
                                    var Owner = worksheet.Cells[row, 3].Value;

                                    bool sStatus = false;
                                    if(_excel.TruncateString(Status.ToString()) == _excel.TruncateString("Active"))
                                    {
                                        sStatus = true;
                                    }

                                    AccountName AccountFind = Service
                                        .GetAll()
                                        .FirstOrDefault(x => _excel.TruncateString(x.Name) == _excel.TruncateString(AccountName.ToString()));

                                    ApplicationUser AppUser = await _userManager.FindByEmailAsync(Owner.ToString());

                                    if (AppUser!=null)
                                    {
                                        var OwnerName = _ownerName.GetByUserId(AppUser.Id);

                                        if (AccountFind == null)
                                        {
                                            AccountName ac = new AccountName { Name = AccountName.ToString(), Status = sStatus, Coms = OwnerName };
                                            Service.Add(ac);
                                            TOTAL_INSERT++;
                                        }
                                        else
                                        {
                                           
                                            AccountName ac = Service.GetById(AccountFind.Id);
                                            ac.Name = AccountName.ToString();
                                            ac.Status = sStatus;
                                            ac.Coms = OwnerName;
                                            Service.Update(ac);
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
