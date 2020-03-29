using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;

namespace App.Domain.Models.Core
{
    public class VacancyList : BaseModel, IEntity
    {

        public VacancyList()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string TerminateNote { get; set; }
        public string TerminateBy { get; set; }

        public string PONumber { get; set; }
        public string POInsertedBy { get; set; }
        public DateTime POInsertDate { get; set; }

        public string Name { get; set; }
        public string NIK { get; set; }

        public string Identifier { get; set; }

        public decimal Quantity { get; set; }

        public SrfApproveStatus BastStatus1 { get; set; }
        public SrfApproveStatus BastStatus2 { get; set; }
        public SrfApproveStatus BastStatus3 { get; set; }

        public SrfApproveStatus BastStatusSL1 { get; set; }
        public SrfApproveStatus BastStatusSL2 { get; set; }
        public SrfApproveStatus BastStatusSL3 { get; set; }

        public SrfApproveStatus StatusOne { get; set; }

        public SrfApproveStatus StatusTwo { get; set; }

        public SrfApproveStatus StatusThree { get; set; }

        public SrfApproveStatus StatusFourth { get; set; }

        public DateTime DateBastApproved1 { get; set; }
        public DateTime DateBastApproved2 { get; set; }
        public DateTime DateBastApproved3 { get; set; }

        public DateTime DateBastApprovedSL1 { get; set; }
        public DateTime DateBastApprovedSL2 { get; set; }
        public DateTime DateBastApprovedSL3 { get; set; }

        public DateTime DateApprovedOne { get; set; }

        public DateTime DateApprovedTwo { get; set; }

        public DateTime DateApprovedThree { get; set; }

        public DateTime DateApprovedFour { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime TerminateDate { get; set; }

        public SrfStatus Status { get; set; }

        public int OtLevel { get; set; }

        public bool isLaptop { get; set; }

        public bool isUsim { get; set; }

        public decimal NoarmalRate { get; set; }

        public bool isManager { get; set; }

        public bool isHrms { get; set; }

        public string Files { get; set; }

        public ApproverStatus VacancyStatus { get; set; }

        public virtual List<CandidateInfo> Candidate { get; set; }

        #region relationship

        public Guid ServicePackCategoryId { get; set; }

        [ForeignKey("ServicePackCategoryId")]
        public virtual ServicePackCategory ServicePackCategory { get; set; }

        public Guid ServicePackId { get; set; }

        [ForeignKey("ServicePackId")]
        public virtual ServicePack ServicePack { get; set; }

        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        public Guid DepartmentSubId { get; set; }

        [ForeignKey("DepartmentSubId")]
        public virtual DepartementSub DepartementSub { get; set; }

        public Guid CostCodeId { get; set; }

        [ForeignKey("CostCodeId")]
        public virtual CostCenter CostCenter { get; set; }

        public Guid NetworkId { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkNumber Network { get; set; }

        public Guid AccountNameId { get; set; }

        [ForeignKey("AccountNameId")]
        public virtual AccountName AccountName { get; set; }

        public Guid JobStageId { get; set; }

        [ForeignKey("JobStageId")]
        public virtual JobStage JobStage { get; set; }

        public int? VendorId { get; set; }

        [ForeignKey("VendorId")]
        public virtual UserProfile Vendor { get; set; }

        public int? ApproverOneId { get; set; }

        [ForeignKey("ApproverOneId")]
        public virtual UserProfile ApproverOne { get; set; }

        [ForeignKey("ApproverTwoId")]
        public int? ApproverTwoId { get; set; }
        public virtual UserProfile ApproverTwo { get; set; }

        [ForeignKey("RpmId")]
        public int? RpmId { get; set; }
        public virtual UserProfile Rpm { get; set; }

        [ForeignKey("ApproverThreeId")]
        public int? ApproverThreeId { get; set; }

        public virtual UserProfile ApproverThree { get; set; }

        [ForeignKey("ApproverFourId")]
        public int? ApproverFourId { get; set; }

        public virtual UserProfile ApproverFour { get; set; }


        [ForeignKey("PackageTypeId")]
        public Guid PackageTypeId { get; set; }

        public virtual PackageType PackageType { get; set; }

        [ForeignKey("RequestById")]
        public int? RequestById { get; set; }

        public virtual UserProfile RequestBy { get; set; }

        [ForeignKey("BastApprover1Id")]
        public int? BastApprover1Id { get; set; }
        public virtual UserProfile BastApprover1 { get; set; }

        [ForeignKey("BastApprover2Id")]
        public int? BastApprover2Id { get; set; }
        public virtual UserProfile BastApprover2 { get; set; }

        [ForeignKey("BastApprover3Id")]
        public int? BastApprover3Id { get; set; }
        public virtual UserProfile BastApprover3 { get; set; }

        [ForeignKey("BastApproverSL1Id")]
        public int? BastApproverSL1Id { get; set; }
        public virtual UserProfile BastApproverSL1 { get; set; }

        [ForeignKey("BastApproverSL2Id")]
        public int? BastApproverSL2Id { get; set; }
        public virtual UserProfile BastApproverSL2 { get; set; }

        [ForeignKey("BastApproverSL3Id")]
        public int? BastApproverSL3Id { get; set; }
        public virtual UserProfile BastApproverSL3 { get; set; }

        #endregion
    }


   

}
