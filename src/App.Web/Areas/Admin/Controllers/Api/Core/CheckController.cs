using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{

    [Area("Admin")]
    [Produces("application/json")]
    [Route("Admin/Api/Check")]
    public class CheckController : BaseApiController<CandidateInfo, ICandidateInfoService, CandidateDto, Guid>
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfile;
        private readonly ISrfRequestService _srf;
        private readonly ICandidateInfoService _candidate;
        private readonly IUserHelper _userHelper;

        public CheckController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IUserProfileService userProfile,
            ISrfRequestService srf,
            ICandidateInfoService candidate,
            ICandidateInfoService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userManager = userManager;
            _userProfile = userProfile;
            _srf = srf;
            _candidate = candidate;
            _userHelper = userHelper;
        }

        [HttpGet]
        [Route("HaveNotVacancy")]
        public IActionResult HaveNotVacancy()
        {
            var Candidate = _candidate.GetAll().Where(x => x.AccountId.HasValue).Select(x => x.AccountId.Value).ToList();
            var UserManager = _userManager.GetUsersInRoleAsync("Contractor").Result;
            var UserProfile = _userProfile.GetAll().Where(x => UserManager.Contains(x.ApplicationUser) && !Candidate.Contains(x.Id)).OrderBy(x=>x.UserName).Select(x => new { x.ApplicationUserId, x.Id, x.Name,x.ApplicationUser.Email, x.ApplicationUser.UserName }).ToList();
            return Ok(UserProfile);
        }

        [HttpGet]
        [Route("HaveNotSrf")]
        public IActionResult HaveNotSrf()
        {
            var CandidateId = _srf.GetAll().Select(x => x.CandidateId).ToList();
            var Candidate = _candidate.GetAll().Where(x => x.AccountId.HasValue && CandidateId.Contains(x.Id)).Select(x => x.AccountId).ToList();
            var UserManager = _userManager.GetUsersInRoleAsync("Contractor").Result;
            var UserProfile = _userProfile.GetAll().Where(x => UserManager.Contains(x.ApplicationUser) && !Candidate.Contains(x.Id)).OrderBy(x => x.UserName).Select(x => new { x.ApplicationUserId, x.Id, x.Name, x.ApplicationUser.Email, x.ApplicationUser.UserName }).ToList();
            return Ok(UserProfile);
        }

        [HttpGet]
        [Route("HaveSrf")]
        public IActionResult HaveSrf()
        {
            var CandidateId = _srf.GetAll().Select(x => x.CandidateId).ToList();
            var Candidate = _candidate.GetAll().Where(x => x.AccountId.HasValue && CandidateId.Contains(x.Id)).Select(x => x.AccountId).ToList();
            var UserManager = _userManager.GetUsersInRoleAsync("Contractor").Result;
            var UserProfile = _userProfile.GetAll().Where(x => UserManager.Contains(x.ApplicationUser) && Candidate.Contains(x.Id)).OrderBy(x => x.UserName).Select(x => new { x.ApplicationUserId, x.Id, x.Name, x.ApplicationUser.Email, x.ApplicationUser.UserName }).ToList();
            return Ok(UserProfile);
        }

        [HttpGet]
        [Route("CheckRole")]
        public IActionResult CheckRole()
        {
            var Role = _userHelper.GetByRoleName("Head Of Sourcing");
            return Ok(Role);
        }

    }
}
