using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class ASPDto : BaseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }
}
