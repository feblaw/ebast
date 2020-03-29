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
    [Route("admin/api/subopses")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class SubOpsesController : BaseApiController<SubOps, ISubOpsService, SubOpsDto, Guid>
    {
        private readonly IAttendaceExceptionListService _timeSheet;

        public SubOpsesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISubOpsService service,
            IAttendaceExceptionListService timeSheet,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.SubOpsId.Equals(id)).FirstOrDefault();
            if(CheckTimeSheet!=null)
            {
                return BadRequest();
            }
            return base.Delete(id);
        }
    }
}
