using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class TimeSheetPeriod : BaseModel, IEntity
    {

        public TimeSheetPeriod()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime DateActual { get; set; }

        public Guid TimeSheetType { get; set; }

        public string Token { get; set; }

    }
}
