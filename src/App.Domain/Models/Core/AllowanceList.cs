using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{
    public class AllowanceList : BaseModel, IEntity
    {
        public AllowanceList()
        {
            Id= Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public string AllowanceNote { get; set; }
        public decimal OnCallNormal { get; set; }
        public decimal ShiftNormal { get; set; }
        public decimal OnCallHoliday { get; set; }
        public decimal ShiftHoliday { get; set; }
        public decimal GrantedHoliday14 { get; set; }
        public Status AllowanceStatus { get; set; }
        public string DataToken { get; set; }

        #region relationship

        public Guid ServicePackId { get; set; }

        [ForeignKey("ServicePackId")]
        public virtual ServicePack ServicePack { get; set; }



        #endregion
    }
}
