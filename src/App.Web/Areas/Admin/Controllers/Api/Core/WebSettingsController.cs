using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using App.Services.Core.Interfaces;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/websettings")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class WebSettingsController : BaseApiController<WebSetting, IWebSettingService, WebSettingDto, Guid>
    {
        public WebSettingsController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IWebSettingService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }
    }
}