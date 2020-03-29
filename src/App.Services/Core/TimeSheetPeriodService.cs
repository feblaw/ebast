using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class TimeSheetPeriodService : BaseService<TimeSheetPeriod, IRepository<TimeSheetPeriod>>, ITimeSheetPeriodService
    {
        public TimeSheetPeriodService(IRepository<TimeSheetPeriod> repository) : base(repository)
        {
        }
    }
}
