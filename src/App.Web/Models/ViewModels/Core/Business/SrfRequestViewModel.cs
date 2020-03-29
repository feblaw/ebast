using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class SrfRequestViewModel
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

        public DateTime? DateApproveStatusOne { get; set; } // Date Remark Line Manager

        public DateTime? DateApproveStatusTwo { get; set; }  // Date Remark Head Of Service Line

        public DateTime? DateApproveStatusThree { get; set; } // Date Remark Head Of Operation

        public DateTime? DateApproveStatusFour { get; set; } // Date Remark Head Of Non Operation

        public DateTime? DateApproveStatusFive { get; set; } // Date Remark Head Of Sourcing

        public DateTime? DateApproveStatusSix { get; set; } // Date Remark Service Cordinator

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

        public int AnnualLeave { get; set; }

        #region relationship
        public Guid ServicePackId { get; set; }
        public Guid NetworkId { get; set; }
        public Guid CostCenterId { get; set; }
        public int LineManagerId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public Guid ExtendFrom { get; set; }
        public Guid AccountId { get; set; }
        public int ProjectManagerId { get; set; }
        public int ApproveOneId { get; set; }
        public int ApproveTwoId { get; set; }
        public int ApproveThreeId { get; set; }
        public int ApproveFourId { get; set; }
        public int ApproveFiveId { get; set; }
        public int ApproveSixId { get; set; }
        public Guid CandidateId { get; set; }

        public bool IsActive { get; set; }
        #endregion
    }
    public class SrfRequestModelForm
    {
        public string Number { get; set; }

        public string Description { get; set; }

        public SrfType Type { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; }

        public SrfApproveStatus ApproveStatusTwo { get; set; }

        public SrfApproveStatus ApproveStatusThree { get; set; }

        public SrfApproveStatus ApproveStatusFour { get; set; }

        public SrfApproveStatus ApproveStatusFive { get; set; }

        public SrfApproveStatus ApproveStatusSix { get; set; }

        public DateTime? DateApproveStatusOne { get; set; } // Date Remark Line Manager

        public DateTime? DateApproveStatusTwo { get; set; }  // Date Remark Head Of Service Line

        public DateTime? DateApproveStatusThree { get; set; } // Date Remark Head Of Operation

        public DateTime? DateApproveStatusFour { get; set; } // Date Remark Head Of Non Operation

        public DateTime? DateApproveStatusFive { get; set; } // Date Remark Head Of Sourcing

        public DateTime? DateApproveStatusSix { get; set; } // Date Remark Service Cordinator

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

        public bool IsActive { get; set; }

        public int AnnualLeave { get; set; }
        public VacancyListFormModel FormVacancy { get; set; }

        #region relationship
        public Guid ServicePackId { get; set; }
        public Guid NetworkId { get; set; }
        public Guid CostCenterId { get; set; }
        public int LineManagerId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public Guid? ExtendFrom { get; set; }

        public Guid AccountId { get; set; }
        public int ProjectManagerId { get; set; }
        public int ApproveOneId { get; set; }
        public int ApproveTwoId { get; set; }
        public int ApproveThreeId { get; set; }
        public int ApproveFourId { get; set; }
        public int ApproveFiveId { get; set; }
        public int ApproveSixId { get; set; }
        public Guid CandidateId { get; set; }
        public int AgencyId { get; set; }
        #endregion
    }

    public class EscalationViewModel
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

        public Guid ServicePackId { get; set; }
        public Guid SrfId { get; set; }
    }

    public class EscalationModelForm
    {
        public int OtLevel { get; set; }

        public bool IsWorkstation { get; set; }

        public bool IsCommnunication { get; set; }

        public bool? IsManager { get; set; }

        public decimal SparateValue { get; set; }

        public string Files { get; set; }

        public StatusEscalation Status { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; }

        public SrfApproveStatus ApproveStatusTwo { get; set; }

        public SrfApproveStatus ApproveStatusThree { get; set; }

        public SrfApproveStatus ApproveStatusFour { get; set; }

        public string Note { get; set; }

        public Guid? PackageTypeId { get; set; }

        public Guid? ServicePackCategoryId { get; set; }

        public Guid ServicePackId { get; set; }

        public Guid SrfId { get; set; }

        public int HeadSourcingId { get; set; }

        public SrfRequestModelForm Srf { get; set; }

    }

    public class ContractorModelForm
    {
        [Required]
        public Guid SrfId { get; set; }

        public String AhID { get; set; }

        public Guid HomeBaseId { get; set; }

        public string SrfNumber { get; set; }

        [Required]
        public String ContrctorName { get; set; }

        public PackageTypes PricelistType { get; set; }

        [Required]
        public Guid ServicePackCategoryId { get; set; }

        [Required]
        public Guid ServicePackId { get; set; }

        [Required]
        public DateTime SrfBegin { get; set; }

        [Required]
        public DateTime SrfEnd { get; set; }

        [Required]
        public int LineManagerId { get; set; }

        [Required]
        public int ProjectManagerId { get; set; }

        public String HomePhoneNumber { get; set; }
        public String MobilePhoneNumber { get; set; }

        [Required]
        public string IdNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public String Nationality { get; set; }

        public String PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }

        public String Address { get; set; }
        public Gender Gender { get; set; }

        public String ApplicationUserId { get; set; }

        [Required]
        public String Username { get; set; }
        public String Password { get; set; }

        public string Notes { get; set; }

        public SrfStatus Status { get; set; }

        public Martial Martial { get; set; }
    }

    public class TrcPViewModel
    {
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public string Department { get; set; }
        public int Srf { get; set; }
        public int Approved { get; set; }
        public int Forecast { get; set; }
        public int Gap { get; set; }
        public String DateSrf { get; set; }
    } 

    public class RecoveryForm
    {
        public Guid DepartmentId { get; set; }

        public Guid DepartmentSubId { get; set; }

        public Guid CostCodeId { get; set; }

        public Guid AccountNameId { get; set; }

        public DateTime JoinDate { get; set; }

        public Guid PackageTypeId { get; set; }

        public Guid ServicePackCategoryId { get; set; }

        public Guid ServicePackId { get; set; }

        public Guid NetworkId { get; set; }

        public int OtLevel { get; set; } // Basic Service Level (OT)

        public int isLaptop { get; set; } // Workstation Services

        public bool isUsim { get; set; } // Communication Services

        public decimal NoarmalRate { get; set; }

        public bool isHrms { get; set; } // Signum

        public bool isManager { get; set; }

        public Guid JobStageId { get; set; }

        public string Description { get; set; }

        public int ApproverOneId { get; set; } // Line Manager

        public int SourcingId { get; set; } // Sourcing

        public string Files { get; set; }

        public ApproverStatus VacancyStatus { get; set; }

        public DateTime SrfBegin { get; set; }

        public DateTime SrfEnd { get; set; }

        public int AgencyId { get; set; }
        public int ApproveOneId { get; set; }
        public int ApproveTwoId { get; set; }
        public int ApproveThreeId { get; set; }
        public int ApproveFourId { get; set; }
        public int ApproveFiveId { get; set; }
        public int ApproveSixId { get; set; }

    }

}
