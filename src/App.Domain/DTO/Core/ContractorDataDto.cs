using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class ContractorDataDto
    {
        #region SrfInfo
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public SrfType Type { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; } // Line Manager

        public SrfApproveStatus ApproveStatusTwo { get; set; }  // Head Of Service Line

        public SrfApproveStatus ApproveStatusThree { get; set; } // Head Of Operation

        public SrfApproveStatus ApproveStatusFour { get; set; } // Head Of Non Operation

        public SrfApproveStatus ApproveStatusFive { get; set; } // Head Of Sourcing

        public SrfApproveStatus ApproveStatusSix { get; set; } // Service Cordinator

        public DateTime DateApproveStatusOne { get; set; } // Date Remark Line Manager

        public DateTime DateApproveStatusTwo { get; set; }  // Date Remark Head Of Service Line

        public DateTime DateApproveStatusThree { get; set; } // Date Remark Head Of Operation

        public DateTime DateApproveStatusFour { get; set; } // Date Remark Head Of Non Operation

        public DateTime DateApproveStatusFive { get; set; } // Date Remark Head Of Sourcing

        public DateTime DateApproveStatusSix { get; set; } // Date Remark Service Cordinator

        public string NotesFirst { get; set; }

        public string NotesLast { get; set; }

        public string RequestBy { get; set; }

        public DateTime? SrfBegin { get; set; }

        public DateTime? SrfEnd { get; set; }

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

        public bool IsActive { get; set; }

        public int AnnualLeave { get; set; }

        public DateTime CreatedAt { get; set; }

        #endregion

        #region Additional
        public string ServiceType { get; set; }
        public string ContractorAhID { get; set; } // CandidateInfo.Name 
        public string ContractorName { get; set; } // CandidateInfo.Name 
        public string ContractorEmail { get; set; } // CandidateInfo.Email
        public string Position { get; set; } // ServicePack.Name
        public string OrganizationUnit { get; set; } // Department.Name
        public string SubOrganizationUnit { get; set; } // DepartmentSub.SubName
        public string LineManager { get; set; } // ApproverOneBy.Name
        public string ProjectManager { get; set; } // NetworkNumber.ProjectManage.Name
        public string Supplier { get; set; } // CandidateInfo.Agency.Name

        #endregion

    }
}
