using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class TacticalResource : BaseModel, IEntity
    {
        public TacticalResource()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        public int Approved { get; set; }

        public int Forecast { get; set; }

        public int CountSrf { get; set; }

        public DateTime? DateSrf { get; set; }

        [Key]
        public Guid Id { get; set; }

        public Guid? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        public Guid? DepartmentSubId { get; set; }

        [ForeignKey("DepartmentSubId")]
        public virtual DepartementSub DepartementSub { get; set; }
    }
}
