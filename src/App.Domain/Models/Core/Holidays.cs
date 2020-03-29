using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class Holidays : BaseModel, IEntity
    {
        public Holidays()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public DateTime DateDay { get; set; }

        public String Description { get; set; }

        public DayType DayType { get; set; }
    }
}
