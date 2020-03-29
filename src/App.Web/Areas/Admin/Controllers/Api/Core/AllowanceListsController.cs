using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
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
    [Route("admin/api/allowancelists")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AllowanceListsController : BaseApiController<AllowanceList, IAllowanceListService,AllowanceListDto, Guid>
    {
        private readonly IServicePackService servicePack;
        public AllowanceListsController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IAllowanceListService service, IUserHelper userHelper, IServicePackService servicePack) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            this.servicePack = servicePack;
            Includes = new Expression<Func<AllowanceList, object>>[2]
            {
                list => list.ServicePack,
                list => list.ServicePack.ServicePackCategory
            };
        }

        [HttpGet]
        [Route("GetByType")]
        public IActionResult GetByType(PackageTypes? types = null)
        {
            var result = servicePack.GetByType(types);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetByCategory")]
        public IActionResult GetByCategory(string categoryId = null)
        {
            var result = servicePack.GetByCategory(categoryId);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetByTypeAndCategory")]
        public IActionResult GetByTypeAndCategory(PackageTypes? types = null, string categoryId = null)
        {
            var result = servicePack.GetByTypeAndCategory(types, categoryId);
            return Ok(result);
        }
    }
}
