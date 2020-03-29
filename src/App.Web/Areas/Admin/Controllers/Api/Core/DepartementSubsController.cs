using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/departementsubs")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class DepartementSubsController : BaseApiController<DepartementSub, IDepartementSubService, DepartementSubDto, Guid>
    {
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;
        private readonly IAttendaceExceptionListService _timeSheet;

        public DepartementSubsController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService, 
            IMapper mapper,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            IAttendaceExceptionListService timeSheet,
            IDepartementSubService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            Includes = new Expression<Func<DepartementSub, object>>[2]
            {
                sub => sub.Departement,
                sub => sub.LineManager,
                //sub => sub.LineManager.UserProfile
            };
            _srf = srf;
            _vacancy = vacancy;
            _timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.DepartmentSubId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.DepartmentSubId.Equals(id)).FirstOrDefault();
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.DepartmentSubId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac != null || CheckTimeSheet!=null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }
    }
}
