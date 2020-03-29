﻿using System;
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
    [Route("admin/api/allowanceforms")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AllowanceFormsController : BaseApiController<AllowanceForm, IAllowanceFormService, AllowanceFormDto, Guid>
    {
        public AllowanceFormsController(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IAllowanceFormService service, IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }
    }
}
