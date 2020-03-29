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
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/departements")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class DepartementsController : BaseApiController<Departement, IDepartementService, DepartementDto, Guid>
    {

        private readonly IUserHelper _userHelper;
        private readonly ISrfRequestService _srf;
        private readonly IVacancyListService _vacancy;
        private readonly IAttendaceExceptionListService _timeSheet;

        public DepartementsController(
            IUserProfileService userProfile,
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            ISrfRequestService srf,
            IVacancyListService vacancy,
            IMapper mapper,
            IAttendaceExceptionListService timeSheet,
            IDepartementService service, IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            Includes = new Expression<Func<Departement, object>>[1]
            {
                departement => departement.Head,
                //departement => departement.Head.UserProfile
            };
            _userHelper = UserHelper;
            _srf = srf;
            _vacancy = vacancy;
            _timeSheet = timeSheet;
        }

        [Route("GetManager")]
        [HttpGet]
        public IActionResult GetManager(int type)
        {
            var item = _userHelper.GetByRoleName("Head Of Operation").ToList();
            if (type == 0)
            {
                item = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
            }
            if(item!=null)
            {
                string option = "<option disabled selected>-- Select Head Of Departement --</option>";
                foreach(var row in item)
                {
                    if (item.Count() > 0)
                    {
                        option += "<option value='" + row.Id + "'>" + row.Name + "</option>";
                    }
                    else
                    {
                        option += "<option value='" + row.Id + "' selected>" + row.Name + "</option>";
                    }
                }
                return Json(new
                {
                    data = option
                });
            }
            return Json(new BadRequestResult());
        }

        public override IActionResult Delete(Guid id)
        {
            var CheckSrf = _srf.GetAll().Where(x => x.DepartmentId.Equals(id)).FirstOrDefault();
            var CheckVac = _vacancy.GetAll().Where(x => x.DepartmentId.Equals(id)).FirstOrDefault();
            var CheckTimeSheet = _timeSheet.GetAll().Where(x => x.DepartmentId.Equals(id)).FirstOrDefault();
            if (CheckSrf != null || CheckVac != null || CheckTimeSheet!=null)
            {
                return Json(BadRequest());
            }
            return base.Delete(id);
        }

    }
}
