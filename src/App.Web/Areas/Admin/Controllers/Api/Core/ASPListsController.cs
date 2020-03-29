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
    [Route("admin/api/asplists")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ASPListsController : BaseApiController<ASP, IASPService, ASPDto, Guid>
    {
        //private readonly IAttendaceExceptionListService _timeSheet;

        public ASPListsController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IASPService service,
            //IAttendaceExceptionListService timeSheet,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            //_timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            //var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.DepartmentId.Equals(id)).FirstOrDefault();
            //if(CheckTimeSheet!=null)
            //{
            //    return NotFound();
            //}
            return base.Delete(id);
        }
    }
}
