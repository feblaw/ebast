using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class HolidaysDto
    {
        public Guid Id { get; set; }

        public DateTime DateDay { get; set; }

        public String Description { get; set; }

        public DayType DayType { get; set; }

        public String TypeName { get; set; }
    }
}
