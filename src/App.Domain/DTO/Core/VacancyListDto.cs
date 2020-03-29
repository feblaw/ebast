using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class VacancyListDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

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

        public DateTime CreatedAt { get; set; }

        public DateTime StartDate { get; set; }
        public Guid AccountNameId { get; set; }

        public DateTime EndDate { get; set; }
        public string TerminateBy { get; set; }
        public string TerminateNote { get; set; }

        public string PONumber { get; set; }
        public string POInsertedBy { get; set; }
        public DateTime POInsertDate { get; set; }

        public string Name { get; set; }
        public string NIK { get; set; }

        public string Identifier { get; set; }

        public decimal Quantity { get; set; }

        #region additional
        public String RequestBy { get; set; }
        public String ApproverTwo { get; set; }
        public String ApproverThree { get; set; }
        public String Vendor { get; set; }
        public String Rpm { get; set; }
        public String BastApprover1 { get; set; }
        public String BastApprover2 { get; set; }
        public String BastApprover3 { get; set; }
        public String ServicePackCategory { get; set; }
        public String ServicePack { get; set; }
        public String Account { get; set; }
        public String Project { get; set; }
        public int CountCandidate { get; set; }

        public int? ApproverOneId { get; set; }
        public int? ApproverTwoId { get; set; }
        public int? ApproverThreeId { get; set; }
        public int? RpmId { get; set; }
        public int? BastApprover1Id { get; set; }
        public int? BastApprover2Id { get; set; }
        public int? BastApprover3Id { get; set; }
        public int? BastApproverSL1Id { get; set; }
        public int? BastApproverSL2Id { get; set; }
        public int? BastApproverSL3Id { get; set; }
        #endregion

    }
}
