using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class SrfEscalationRequestDto
    {
        public Guid Id { get; set; }

        public int OtLevel { get; set; }

        public bool IsWorkstation { get; set; }

        public bool IsCommnunication { get; set; }

        public decimal SparateValue { get; set; }

        public string Files { get; set; }

        public StatusEscalation Status { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; }

        public SrfApproveStatus ApproveStatusTwo { get; set; }

        public SrfApproveStatus ApproveStatusThree { get; set; }

        public SrfApproveStatus ApproveStatusFour { get; set; }

        public string Note { get; set; }

        #region relationshi
        public  ServicePackDto ServicePack { get; set; }
        public  SrfRequestDto SrfRequest { get; set; }
        #endregion
    }
}
