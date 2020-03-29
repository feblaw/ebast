using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/activities")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ActivitiesController : BaseApiController<ActivityCode, IActivityCodeService, ActivityCodeDto, Guid>
    {
        private readonly ISrfRequestService _srf;
        private readonly IAttendaceExceptionListService _timeSheet;

        public ActivitiesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IActivityCodeService service,
            IAttendaceExceptionListService timeSheet,
            ISrfRequestService srf,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _srf = srf;
            _timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.ActivityId.Equals(id)).FirstOrDefault();
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.ActivityId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckTimeSheet!=null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }
    }
}
