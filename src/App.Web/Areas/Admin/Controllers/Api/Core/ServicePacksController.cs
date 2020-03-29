using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    /// <summary>
    /// SSOW Api Controller
    /// </summary>
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/servicepacks")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ServicePacksController : BaseApiController<ServicePack, IServicePackService, ServicePackDto, Guid>
    {
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;

        public ServicePacksController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IServicePackService service,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            Includes = new Expression<Func<ServicePack, object>>[1];
            Includes[0] = pack => pack.ServicePackCategory;
            _srf = srf;
            _vacancy = vacancy;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.ServicePackId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.ServicePackId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac != null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }
    }
}
