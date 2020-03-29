using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace App.Services.Core.Interfaces
{
    public interface IAttendaceExceptionListService : IService<AttendaceExceptionList>
    {
        List<ExportExcelsDto> ExportExcel(int UserId, ClaimsPrincipal role, DateTime FirstDate, DateTime LastDate);
    }
}
