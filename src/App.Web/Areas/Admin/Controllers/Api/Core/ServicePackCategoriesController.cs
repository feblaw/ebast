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
    /// <summary>
    /// SSOW Category Api Controller
    /// </summary>
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/servicepackcategories")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class ServicePackCategoriesController : BaseApiController<ServicePackCategory, IServicePackCategoryService, ServicePackCategoryDto, Guid>
    {
        private readonly IVacancyListService _vacancy;

        public ServicePackCategoriesController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IVacancyListService vacancy,
            IMapper mapper, 
            IServicePackCategoryService service, IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _vacancy = vacancy;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckVac = _vacancy.GetAll().Where(x => x.ServicePackCategoryId.Equals(id)).FirstOrDefault();
            if (CheckVac != null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }
    }
}
