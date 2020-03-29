using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Domain.DTO.Identity;

namespace App.Domain.DTO.Core
{
    public class SrfRequestDto
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public SrfType Type { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; }

        public SrfApproveStatus ApproveStatusTwo { get; set; }

        public SrfApproveStatus ApproveStatusThree { get; set; }

        public SrfApproveStatus ApproveStatusFour { get; set; }

        public SrfApproveStatus ApproveStatusFive { get; set; }

        public SrfApproveStatus ApproveStatusSix { get; set; }

        public DateTime DateApproveStatusOne { get; set; } // Date Remark Line Manager

        public DateTime DateApproveStatusTwo { get; set; }  // Date Remark Head Of Service Line

        public DateTime DateApproveStatusThree { get; set; } // Date Remark Head Of Operation

        public DateTime DateApproveStatusFour { get; set; } // Date Remark Head Of Non Operation

        public DateTime DateApproveStatusFive { get; set; } // Date Remark Head Of Sourcing

        public DateTime DateApproveStatusSix { get; set; } // Date Remark Service Cordinator

        public string NotesFirst { get; set; }

        public string NotesLast { get; set; }

        public string RequestBy { get; set; }

        public DateTime SrfBegin { get; set; }

        public DateTime SrfEnd { get; set; }

        public int ServiceLevel { get; set; }

        public bool isWorkstation { get; set; }

        public bool isCommunication { get; set; }

        public bool IsHrms { get; set; }

        public bool IsOps { get; set; }

        public bool IsManager { get; set; }

        public RateType RateType { get; set; }

        public string TerimnatedBy { get; set; }

        public DateTime TerminatedDate { get; set; }

        public string TeriminateNote { get; set; }

        public bool IsExtended { get; set; }

        public bool IsLocked { get; set; }
        public SrfStatus Status { get; set; }
        public decimal SpectValue { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public int AnnualLeave { get; set; }
        public int ApproveOneId { get; set; }
        public int ApproveTwoId { get; set; }
        public int ApproveThreeId { get; set; }
        public int ApproveFourId { get; set; }
        public int ApproveFiveId { get; set; }
        public int ApproveSixId { get; set; }


        #region relationship
        public String PackageType { get; set; }
        public String EmployeeName { get; set; }
        public String LineManager { get; set; }
        public String ServiceCoordinator { get; set; }
        public String DepartmentSub { get; set; }
        public String Position { get; set; }
        public bool IsEndSoon { get; set; }
        public bool IsOperation { get; set; }
        public bool IsEscalation { get; set; }
        public StatusEscalation StatusEscalationLineManager { get; set; } // Main Status Line Manager
        public SrfApproveStatus StatusEscalationServiceLine { get; set; } // Head Of Service Line
        public SrfApproveStatus StatusEscalationHeadOperation { get; set; } // Head Of Operation or None
        public SrfApproveStatus StatusEscalationHeadSourcing { get; set; } // Head Of Sourcing
        public SrfApproveStatus StatusEscalationServiceCoordinator { get; set; } // Service Coordinator
        public decimal EscalationRate { get; set; } // Additional Rate
        #endregion
    }
}
