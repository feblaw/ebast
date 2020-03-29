using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class TravelRequestViewModel
    {
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
        
        public string Token { get; set; }
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
        #region relationship
        public Guid DepartureId { get; set; }
        public Guid DestinationId { get; set; }
        public Guid CostCenterId { get; set; }
        public Guid NetworkNumberId { get; set; }
        public Guid ActivityCodeId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid VacancyId { get; set; }
        public int ClaimApproverOneId { get; set; }
        public int ClaimApproverTwoId { get; set; }
        public int ClaimById { get; set; }
        public int ClaimForId { get; set; }
        public int RedeemForId { get; set; }
        public Guid ClaimCategoryId { get; set; }
        #endregion
    }

    public class TravelRequestModelForm
    {

        public Guid CostCenterId { get; set; }
        public Guid NetworkNumberId { get; set; }
        public Guid ActivityCodeId { get; set; }
        public Guid VacancyId { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid DepartureId { get; set; }
        public Guid DestinationId { get; set; }
        public Schedule Schedule { get; set; }
        public string Description { get; set; }
        public int ClaimApproverOneId { get; set; }
        public int ClaimApproverTwoId { get; set; }
        //public int TravelReqNo { get; set; }
        public DayType DayType { get; set; }
    }

}
