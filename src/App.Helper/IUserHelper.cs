using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Helper
{
    public interface IUserHelper
    {
        string GetUserId(ClaimsPrincipal user);
        ApplicationUser GetUser(string id);
        ApplicationUser GetUser(ClaimsPrincipal user);
        string GetAvatar(string id);
        string GetAvatar(ClaimsPrincipal user);
        UserProfile GetLoginUser(ClaimsPrincipal user);
        UserProfile GetUserProfile(int id);
        SrfRequest GetCurrentSrfByLogin(ClaimsPrincipal user);
        SrfRequest GetCurrentSrfByCandidate(Guid CandidateId);
        SrfRequest GetCurrentSrfByUserProfile(int id);
        List<UserProfile> GetByRoleName(string role);
    }
}
