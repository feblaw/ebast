using App.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Http;
using App.Helper;
using System.Security.Principal;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using App.Services.Identity;
using App.Web.Models.ViewModels.Core.Business;
using App.Domain.Models.Core;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using App.Web.Controllers;
using AutoMapper;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Authorize]
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/dashboard")]
    public class DashboardController : BaseApiController<SrfRequest, ISrfRequestService, SrfRequest, Guid>
    {
        private readonly IDashboardService _dashboard;
        private readonly IUserHelper _userHelper;
        private readonly IUserProfileService _userProfile;

        public DashboardController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IUserProfileService userProfile,
            IDashboardService dashboard,
            ISrfRequestService service,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _dashboard = dashboard;
            _userHelper = userHelper;
            _userProfile = userProfile;
        }

        [HttpGet]
        [Route("GetBarchart")]
        public IActionResult GetBarchart()
        {
            var UserId = _userHelper.GetUserId(User);
            var Profile = _userProfile.GetByUserId(UserId);
            var AllBallChart = _dashboard.AllSrfChart(Profile.Id,User);
            return Json(AllBallChart);
        }

        [HttpGet]
        [Route("GetHedOperation")]
        public IActionResult GetHedOperation()
        {
            var UserId = _userHelper.GetUserId(User);
            var Profile = _userProfile.GetByUserId(UserId);
            var BarChartOperaion = _dashboard.ChartByHeadDepartment(Profile.Id,User);
            return Json(BarChartOperaion);
        }


        [HttpGet]
        [Route("GetServiceLine")]
        public IActionResult GetServiceLine()
        {
            var UserId = _userHelper.GetUserId(User);
            var Profile = _userProfile.GetByUserId(UserId);
            var BarChartByServiceLine = _dashboard.ChartByLineManager(Profile.Id, User);
            return Json(BarChartByServiceLine);
        }


    }
}
