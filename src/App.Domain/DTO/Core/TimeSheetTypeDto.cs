using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class TimeSheetTypeDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

        public int CountTimeSheet { get; set; }
    }
}
