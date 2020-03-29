using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core;
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
    [Route("admin/api/jobstages")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class JobStagesController : BaseApiController<JobStage, IJobStageService, JobStageDTO, Guid>
    {
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;

        public JobStagesController
            (IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            IJobStageService service,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _srf = srf;
            _vacancy = vacancy;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.AccountId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.AccountNameId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac != null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }

    }
}
