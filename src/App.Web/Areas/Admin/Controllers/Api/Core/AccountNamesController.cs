using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/accountnames")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AccountNamesController : BaseApiController<AccountName, IAccountNameService, AccountNameDto, Guid>
    {
        private readonly ExcelHelper excelHelper;
        private readonly CommonHelper helper;
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;
        private readonly IAttendaceExceptionListService _timeSheet;

        public AccountNamesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            IAccountNameService service, 
            IUserHelper userHelper,
            IAttendaceExceptionListService timeSheet,
            ExcelHelper excelHelper, 
            CommonHelper helper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.excelHelper = excelHelper;
            this.helper = helper;
            _srf = srf;
            _vacancy = vacancy;
            _timeSheet = timeSheet;
        }

        [HttpGet]
        [Route("Datatables")]
        public override IActionResult GetDataTables(IDataTablesRequest request)
        {
            Expression<Func<AccountName, Object>>[] includes = { (x => x.Coms) };

            var response = Service.GetDataTablesResponse<AccountNameDto>(request,
                Mapper, Where, includes);

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Expression<Func<AccountName, Object>>[] includes = { (x => x.Coms) };

            var response = Service.GetDataTablesResponse<AccountNameDto>(request,
                Mapper, Where, includes);

            return new DataTablesJsonResult(response, true);
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.AccountId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.AccountNameId.Equals(id)).FirstOrDefault();
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.AccountNameId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac!=null || CheckTimeSheet!=null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }

    }
}
