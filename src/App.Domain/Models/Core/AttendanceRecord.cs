using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class AttendanceRecord :BaseModel,IEntity
    {
        public AttendanceRecord()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public DateTime AttendanceRecordDate { get; set; }
        public int Hours { get; set; }
        public Guid AttendaceExceptionListId { get; set; }
        [ForeignKey("AttendaceExceptionListId")]
        public virtual AttendaceExceptionList AttendaceExceptionList { get; set; }
    }
}
