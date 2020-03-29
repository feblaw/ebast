using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{

    public class AutoGenerateVariable : BaseModel, IEntity
    {
        public AutoGenerateVariable()
        {
            Id = Guid.NewGuid();
            
        }
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "char(2)")]
        public string GenerateDate { get; set; }

        [Column(TypeName = "char(2)")]
        public string GenerateHour { get; set; }

        [Column(TypeName = "char(2)")]
        public string GenerateMinute { get; set; }

        [Column(TypeName = "char(2)")]
        public string GenerateSecond { get; set; }

        [Column(TypeName = "char(2)")]
        public string DayNight { get; set; }
        public DateTime SetDate { get; set; }

        [Column(TypeName = "char(4)")]
        public string SetBy { get; set; }
        public string function { get; set; } // Billing as "Bill" & Backup as "backup"
        public bool AutoStatus { get; set; }
        public Cycle Cycle { get; set; }


    }
}
