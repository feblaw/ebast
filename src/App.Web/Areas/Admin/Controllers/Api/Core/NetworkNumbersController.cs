using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
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
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/networknumbers")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class NetworkNumbersController : BaseApiController<NetworkNumber, INetworkNumberService, NetworkNumberDto, Guid>
    {
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;
        private readonly IAttendaceExceptionListService _timeSheet;

        public NetworkNumbersController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            INetworkNumberService service,
            IAttendaceExceptionListService timeSheet,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            Includes = new Expression<Func<NetworkNumber, object>>[5]
            {
                number => number.Departement,
                number => number.LineManager,
                number => number.Project,
                number => number.AccountName,
                number => number.ProjectManager,

            };
            _srf = srf;
            _vacancy = vacancy;
            _timeSheet = timeSheet;
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.NetworkId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.NetworkId.Equals(id)).FirstOrDefault();
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.NetworkId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac != null || CheckTimeSheet!=null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }

    }
}
