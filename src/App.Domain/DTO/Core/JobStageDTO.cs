using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class JobStageDTO : BaseDto
    {
        public Guid Id { get; set; }
        public string Stage { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }
}
