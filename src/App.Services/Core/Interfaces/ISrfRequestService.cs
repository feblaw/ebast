using App.Domain.Models.Core;
using System;
using System.Security.Claims;

namespace App.Services.Core.Interfaces
{
    public interface ISrfRequestService : IService<SrfRequest>
    {
        string GenerateNumnber();
        string GenerateNumnberByCustom(int index);
        void SetActive(Guid SrfId, Guid CandidateId,int UserProfileId);
        bool DeleteSrf(Guid Id);
        bool DeleteContractor(Guid Id);
        int ActualSrf(int UserId,DateTime DateSrf, Boolean IsActive, Guid? DepartmentId = null, Guid? DepartementSubId = null,ClaimsPrincipal User = null);
    }
}
