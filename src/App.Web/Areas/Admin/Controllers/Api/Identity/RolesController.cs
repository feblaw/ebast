using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Web.Controllers;
using App.Domain.Models.Identity;
using App.Services.Identity;
using App.Domain.DTO.Identity;
using AutoMapper;
using App.Helper;
using Microsoft.AspNetCore.Authorization;

namespace App.Web.Areas.Admin.Controllers.Api.Identity
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/roles")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class RolesController : BaseApiController<ApplicationRole, IRoleService, ApplicationRoleDto, string>
    {
        public RolesController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IRoleService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }
    }
}