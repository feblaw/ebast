using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
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

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("Admin/Api/SrfDelete")]
    [Authorize]
    public class SrfDeleteController : BaseApiController<SrfRequest, ISrfRequestService, SrfRequestDto, Guid>
    {
        public SrfDeleteController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, ISrfRequestService service, IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }

        public override IActionResult Delete(Guid id)
        {
            return Ok(Service.DeleteContractor(id));
        }
    }
}
