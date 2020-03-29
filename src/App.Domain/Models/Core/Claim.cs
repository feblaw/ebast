using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{
    public class Claim : BaseModel, IEntity
    {
        public Claim()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
        public Guid Id { get; set; }
        public ClaimType ClaimType { get; set; }
        public DateTime ClaimDate { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Schedule Schedule { get; set; }
        public string Description { get; set; }
        public int ApproverOne { get; set; }
        public int ApproverTwo { get; set; }
        public int TravelReqNo { get; set; }
        public DateTime AddDate { get; set; }
        public StatusOne StatusOne { get; set; }
        public StatusTwo StatusTwo { get; set; }
        public DateTime ApprovedDateOne { get; set; }
        public DateTime ApprovedDateTwo { get; set; }
        public ActiveStatus ClaimStatus { get; set; }
        public TripType TripType { get; set; }
        public AllowanceOptions AllowanceOption { get; set; }
        public EmployeeLevel EmployeeLevel { get; set; }
        public DayType DayType { get; set; }
        public decimal OnCallShift { get; set; }
        public decimal Domallo1 { get; set; }
        public decimal Domallo2 { get; set; }
        public decimal Domallo3 { get; set; }
        public decimal Domallo4 { get; set; }
        public decimal Domallo5 { get; set; }
        public decimal Domallo6 { get; set; }
        public decimal Intallo1 { get; set; }
        public decimal Intallo2 { get; set; }
        public decimal Intallo3 { get; set; }
        public decimal Intallo4 { get; set; }
        public decimal Intallo5 { get; set; }
        public decimal Intallo6 { get; set; }
        public string Files { get; set; }
        public string ApproverOneNotes { get; set; }
        public string ApproverTwoNotes { get; set; }

        #region relationship

        public Guid? DepartureId { get; set; }

        [ForeignKey("DepartureId")]
        public virtual City Departure { get; set; }

        public Guid? DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public virtual City Destination { get; set; }

        public Guid? CostCenterId { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }

        public Guid? VacancyId { get; set; }

        [ForeignKey("VacancyId")]
        public virtual VacancyList Vacancy { get; set; }

        public Guid? NetworkNumberId { get; set; }

        [ForeignKey("NetworkNumberId")]
        public virtual NetworkNumber NetworkNumber { get; set; }

        public Guid? ActivityCodeId { get; set; }

        [ForeignKey("ActivityCodeId")]
        public virtual ActivityCode ActivityCode { get; set; }

        public Guid? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; }

        public int? ClaimApproverOneId { get; set; }

        [ForeignKey("ClaimApproverOneId")]
        public virtual UserProfile ClaimApproverOne { get; set; }

        public int? ClaimApproverTwoId { get; set; }

        [ForeignKey("ClaimApproverTwoId")]
        public virtual UserProfile ClaimApproverTwo { get; set; }

        public int? ClaimForId { get; set; }

        [ForeignKey("ClaimForId")]
        public virtual UserProfile ClaimFor { get; set; }

        public int? RedeemForId { get; set; }

        [ForeignKey("RedeemForId")]
        public virtual UserProfile RedeemFor { get; set; }

        public Guid? ClaimCategoryId { get; set; }

        [ForeignKey("ClaimCategoryId")]
        public virtual ClaimCategory ClaimCategory { get; set; }

        public int? ContractorId { get; set; }

        [ForeignKey("ContractorId")]
        public virtual UserProfile Contractor { get; set; }

        public int? AgencyId { get; set; }

        [ForeignKey("AgencyId")]
        public virtual UserProfile Agency { get; set; }

        public Guid? ContractorProfileId { get; set; }

        [ForeignKey("ContractorProfileId")]
        public virtual CandidateInfo ContractorProfile { get; set; }
        public virtual TicketInfo Ticket { get; set; }

        #endregion
    }
}
