using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class SrfEscalationRequest : BaseModel, IEntity
    {
        public SrfEscalationRequest()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public int OtLevel { get; set; } // BasiC Service Level

        public bool IsWorkstation { get; set; } // Is Workstation

        public bool IsCommnunication { get; set; } // Is Communication

        public decimal SparateValue { get; set; } // Additional Rate

        public string Files { get; set; }

        public StatusEscalation Status { get; set; } // Main Status

        public SrfApproveStatus ApproveStatusOne { get; set; } // Head Of Service Line

        public SrfApproveStatus ApproveStatusTwo { get; set; } // Head Of Operation or None

        public SrfApproveStatus ApproveStatusThree { get; set; } // Head Of Sourcing

        public SrfApproveStatus ApproveStatusFour { get; set; } // Service Coordinator

        public string Note { get; set; }

        #region relationship

        public Guid ServicePackId { get; set; }

        [ForeignKey("ServicePackId")]
        public virtual ServicePack ServicePack { get; set; }

        public Guid SrfId { get; set; }

        [ForeignKey("SrfId")]
        public virtual SrfRequest SrfRequest { get; set; }
        #endregion
    }

    
}
