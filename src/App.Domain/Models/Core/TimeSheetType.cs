using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class TimeSheetType : BaseModel, IEntity
    {

        public TimeSheetType()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Type { get; set; }

        public string Token { get; set; }

        public virtual List<AttendaceExceptionList> ListTimeSheet { get; set; }

    }
}
