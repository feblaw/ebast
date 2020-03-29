using App.Domain.Models.Identity;
using App.Services.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using App.Services.Core.Interfaces;
using System.Linq;
using App.Domain.Models.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace App.Helper
{
    public class UserHelper : IUserHelper
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private IUserService _userService;
        private ConfigHelper _configHelper;
        private readonly IUserProfileService _userProfile;
        private readonly IEmailArchieveService _email;
        private readonly ISrfRequestService _srf;
        private readonly ICandidateInfoService _candidate;

        public UserHelper(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserService userService,
            IUserProfileService userProfile,
            IEmailArchieveService mail,
            ISrfRequestService srf,
            ICandidateInfoService candidate,
            ConfigHelper configHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _configHelper = configHelper;
            _userProfile = userProfile;
            _email = mail;
            _srf = srf;
            _candidate = candidate;
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return "";
            }

            //var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = user.FindFirstValue("sub");
            return userId;
        }

        public ApplicationUser GetUser(string id)
        {
            var user = _userService.GetById(id);
            return user;
        }

        public ApplicationUser GetUser(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);

            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return GetUser(userId);
        }

        public UserProfile GetLoginUser(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            return _userProfile.GetByUserId(userId);
        }

        public string GetAvatar(string id)
        {
            var user = GetUser(id);
            var attachment = user.UserProfile.Photo.ConvertToAttachment();
            if (attachment == null)
                return _configHelper.GetConfig("user.no.avatar.url");

            return "/" + attachment.CropedPath ?? attachment.Path;
        }

        public string GetAvatar(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);

            if (string.IsNullOrWhiteSpace(userId))
                return _configHelper.GetConfig("user.no.avatar.url");

            return GetAvatar(userId);
        }

        public UserProfile GetUserProfile(int id)
        {
            return _userProfile.GetById(id);
        }


        public List<UserProfile> GetByRoleName(string role)
        {
           
            if (!string.IsNullOrEmpty(role))
            {
                var User = _userManager.GetUsersInRoleAsync(role).Result.Select(x=>x.Id).ToList();
                return _userProfile.GetAll().Where(x=>User.Contains(x.ApplicationUserId)
                    && x.IsActive == true
                    ).ToList();
            }
            return null;
        }

        public SrfRequest GetCurrentSrfByLogin(ClaimsPrincipal user)
        {
            var UserProfile = GetLoginUser(user);
            var Candidate = _candidate.GetAll().Where(x => x.AccountId.Equals(UserProfile.Id)).FirstOrDefault();
            var GetSrf = _srf.GetAll().Where(x => x.CandidateId.Equals(Candidate.Id) && x.IsActive == true).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            if (GetSrf == null)
            {
                GetSrf = _srf.GetAll().Where(x => x.CandidateId.Equals(Candidate.Id)).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            }
            return GetSrf;
        }

        public SrfRequest GetCurrentSrfByCandidate(Guid CandidateId)
        {
            var GetSrf = _srf.GetAll().Where(x => x.CandidateId.Equals(CandidateId) && x.IsActive == true).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            if (GetSrf == null)
            {
                GetSrf = _srf.GetAll().Where(x => x.CandidateId.Equals(CandidateId)).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            }
            return GetSrf;
        }

        public SrfRequest GetCurrentSrfByUserProfile(int id)
        {
            var Canidate = _candidate.GetAll().Where(x => x.AccountId.Equals(id)).FirstOrDefault();
            return GetCurrentSrfByCandidate(Canidate.Id);
        }
    }
}
