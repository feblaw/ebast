using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class AttendanceRecordService : BaseService<AttendanceRecord, IRepository<AttendanceRecord>>, IAttendanceRecordService
    {
        public AttendanceRecordService(IRepository<AttendanceRecord> repository) : base(repository)
        {
        }

        public int GetHours(Guid TimeSheetId, DateTime RecordDate)
        {
            var item = _repository.GetAll().Where(x => x.AttendaceExceptionListId == TimeSheetId && x.AttendanceRecordDate.Date == RecordDate.Date).FirstOrDefault();
            if(item != null) return item.Hours;
            return 0;
        }
    }
}
