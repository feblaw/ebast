using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class DutySchedule : BaseModel, IEntity
    {
        public DutySchedule()
        {
            Id = Guid.NewGuid();
        }
        
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int OnDutyHour { get; set; }
        public int OnDutyMinute { get; set; }
        public int OffDutyHour { get; set; }
        public int OffDutyMinute { get; set; }
        public bool IsEnabled { get; set; }
    }
}
