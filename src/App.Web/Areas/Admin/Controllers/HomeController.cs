using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Identity;
using App.Services.Core.Interfaces;
using App.Web.Models.ViewModels.Core.Business;
using App.Domain.Models.Enum;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        private readonly IDashboardService _dashboard;
        private readonly IUserHelper _userHelper;
        private readonly IUserProfileService _userProfile;
        private readonly IAccountNameService _account;
        private readonly ISrfRequestService _srf;
        private readonly ICandidateInfoService _candidate;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAttendaceExceptionListService _timeSheet;

        public HomeController(
            IDashboardService dashboard,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper, 
            IUserProfileService userProfile,
            IAccountNameService account, 
            ISrfRequestService srf,
            IAttendaceExceptionListService timeSheet,
            ICandidateInfoService candidate)
        {
            this.userManager = userManager;
            _dashboard = dashboard;
            _userHelper = userHelper;
            _userProfile = userProfile;
            _account = account;
            _srf = srf;
            _candidate = candidate;
            _timeSheet = timeSheet;
        }

        public IActionResult Index()
        {
            if(User.IsInRole("Contractor") || User.IsInRole("HR Agency"))
            {
                return RedirectToAction("Index", "Profile", new { area = "Admin" });
            }
            else
            {
                String UserId = _userHelper.GetUserId(User);
                var UserProfile = _userProfile.GetByUserId(UserId);
                ViewBag.CountTimeSheet = _dashboard.CountTimeSheetByApprover(UserProfile.Id);
                ViewBag.CountTravel = _dashboard.CountTravelByApprover(UserProfile.Id);
                ViewBag.CountBastPending = _dashboard.CountBastByApprover(UserProfile.Id);
                ViewBag.CountClaim = _dashboard.CountClaimByApprover(UserProfile.Id);
                ViewBag.CountVacancy = _dashboard.CountWPByApprover(UserProfile.Id,User);
                ViewBag.CountEndSoon = _dashboard.CountWPEndSoon(UserProfile.Id, User);
                ViewBag.CountActive = _dashboard.CountWPActive(UserProfile.Id, User);
                ViewBag.CountExpired = _dashboard.CountWPExpired(UserProfile.Id, User);
                ViewBag.CountSrf = _dashboard.CountSrfByApprover(UserProfile.Id,User);
                ViewBag.CountSrfEsc = _dashboard.CountSrfEscByApprover(UserProfile.Id, User);
                ViewBag.PieChartAccount = _dashboard.PieChartAccountNameByWP(UserProfile.Id,User);
                return View();
            }
        }

     
    }
}
