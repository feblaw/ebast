using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class CostCenterDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartementName { get; set; }
    }
}
