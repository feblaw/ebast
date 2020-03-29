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
    [Route("admin/api/projects")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ProjectsController: BaseApiController<Projects, IProjectsService, ProjectDto, Guid>
    {
        private readonly IAttendaceExceptionListService _timeSheet;

        public ProjectsController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IAttendaceExceptionListService timeSheet,
            IMapper mapper, 
            IProjectsService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.ProjectId.Equals(id)).FirstOrDefault();
            if(CheckTimeSheet!=null)
            {
                return BadRequest();
            }
            return base.Delete(id);
        }
    }
}
