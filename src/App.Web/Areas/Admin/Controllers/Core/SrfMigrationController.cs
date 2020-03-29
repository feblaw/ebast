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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace App.Web.Areas.Admin.Controllers.Core
{

    [Authorize]
    [Area("Admin")]
    public class SrfMigrationController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {
        private readonly IUserHelper _userHelper;
        private readonly IHostingEnvironment _environment;
        private readonly ISrfMigrationService _migration;
        private string _connectionString { get; set; }
        private Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; }

        public SrfMigrationController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IHostingEnvironment environment,
            ISrfRequestService service,
            ISrfMigrationService migration,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userHelper = userHelper;
            _environment = environment;
            _migration = migration;
            Configuration = configuration;
            _connectionString = Configuration["DataAccessPostgreSqlProvider:ConnectionString"];
        }

        public override IActionResult Index()
        {
            _migration.SetActiveSrf(_connectionString);
            return Content("Set Actived SRF OK");
        }

        public IActionResult GenerateRunningNumber()
        {
            //var Srf = Service.GetAll().Where(x => x.SrfBegin.HasValue && x.SrfEnd.HasValue).OrderBy(x => x.SrfBegin).ToList();
            var Srf = Service.GetAll().Where(x => x.CreatedAt.Value == new DateTime(2011, 11, 11) || x.CreatedAt.Value == DateTime.Now).OrderBy(x => x.SrfBegin).ToList();
            if (Srf != null)
            {
                int Index = 1944;
                foreach (var row in Srf)
                {
                    var Temp = Service.GetById(row.Id);
                    Temp.Number = Service.GenerateNumnberByCustom(Index);
                    Service.Update(Temp);
                    Index++;
                }
            }
            return Content("Set Running Number SRF OK");

        }

        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
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
                    var ConnectionString = Configuration["DataAccessPostgreSqlProvider:ConnectionString"];
                    var Deleted = _migration.DeleteAllContractor(ConnectionString);
                    if(Deleted==true)
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
            System.IO.File.Delete(System.IO.Path.Combine(uploads, file.FileName));
            return RedirectToAction("Index", "Srf", new { area = "Admin" });
        }


    }
}
