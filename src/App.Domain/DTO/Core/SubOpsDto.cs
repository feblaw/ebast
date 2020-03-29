using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class SubOpsDto : BaseDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Token { get; set; }

        public bool Status { get; set; }
    }
}
