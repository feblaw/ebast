using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Services.Core.Interfaces
{
    public interface IDashboardService : IService<SrfRequest>
    {
        int CountTimeSheetByApprover(int Id);
        int CountTravelByApprover(int Id);
        int CountBastByApprover(int Id);
        int CountClaimByApprover(int Id);
        int CountVacancyByApprover(int Id);
        int CountWPByApprover(int Id, ClaimsPrincipal User);
        int CountWPActive(int Id, ClaimsPrincipal User);
        int CountWPEndSoon(int Id, ClaimsPrincipal User);
        int CountWPExpired(int Id, ClaimsPrincipal User);
        int CountSrfByApprover(int Id, ClaimsPrincipal User);
        int CountSrfEscByApprover(int Id, ClaimsPrincipal User);
        String PieChartAccountNameBySrf(int Id,ClaimsPrincipal User);
        String PieChartAccountNameByWP(int Id, ClaimsPrincipal User);
        String AllSrfChart(int Id, ClaimsPrincipal User,Guid? DepartmentId = null, Guid? SubDepartmentId = null);
        String ChartByHeadDepartment(int Id, ClaimsPrincipal User);
        String ChartByLineManager(int Id, ClaimsPrincipal User);
    }
}
