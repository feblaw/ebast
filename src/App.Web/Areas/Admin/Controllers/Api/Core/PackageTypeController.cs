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
    [Route("admin/api/packagetype")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class PackageTypeController : BaseApiController<PackageType, IPackageTypeService, PackageTypeDto, Guid>
    {

        private readonly IVacancyListService _vacancy;

        public PackageTypeController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IPackageTypeService service,
            IVacancyListService vacancy,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {

            _vacancy = vacancy;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckVac = _vacancy.GetAll().Where(x => x.PackageTypeId.Equals(id)).FirstOrDefault();
            if (CheckVac != null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }
    }
}
