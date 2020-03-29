using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class AllowanceListDto : BaseDto
    {
        public Guid Id { get; set; }
        public string AllowanceNote { get; set; }
        public decimal OnCallNormal { get; set; }
        public decimal ShiftNormal { get; set; }
        public decimal OnCallHoliday { get; set; }
        public decimal ShiftHoliday { get; set; }
        public decimal GrantedHoliday14 { get; set; }
        public Status AllowanceStatus { get; set; }
        public string DataToken { get; set; }
        public Guid ServicePackId { get; set; }
        public ServicePackDto ServicePacks { get; set; }
    }
}
