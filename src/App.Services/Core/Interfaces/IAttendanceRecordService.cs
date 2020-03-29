using App.Domain.Models.Core;
using System;

namespace App.Services.Core.Interfaces
{
    public interface IAttendanceRecordService : IService<AttendanceRecord>
    {
        int GetHours(Guid TimeSheetId, DateTime RecordDate);
    }
}
