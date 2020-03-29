using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
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
    [Route("admin/api/holidays")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class HolidaysController : BaseApiController<Holidays, IHolidaysService, HolidaysDto, Guid>
    {
        public HolidaysController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IHolidaysService service, IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }
    }
}
