using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class AttendaceExceptionListViewModel
    {
        public Guid Id { get; set; }
        public AttendanceExceptionListType Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public DateTime AddDate { get; set; }
        public int CreateBy { get; set; }
        public StatusOne StatusOne { get; set; }
        public StatusTwo StatusTwo { get; set; }
        public DateTime ApprovedOneDate { get; set; }
        public DateTime ApprovedTwoDate { get; set; }
        public ActiveStatus RequestStatus { get; set; }
        public string Files { get; set; }
        public Guid NetworkId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid CostId { get; set; }
        public Guid LocationId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AccountNameId { get; set; }
        public Guid SubOpsId { get; set; }
        public Guid VacancyId { get; set; }
        public int ApproverOneId { get; set; }
        public int ApproverTwoId { get; set; }
        public int RequestById { get; set; }
        public Guid TimeSheetTypeId { get; set; }
        public int ContractorId { get; set; }
        public int AgencyId { get; set; }

        public int RemainingDays { get; set; }
    }

    public class AttendaceExceptionListModelForm
    {
        public AttendanceExceptionListType Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public DateTime AddDate { get; set; }
        public int CreateBy { get; set; }
        public StatusOne StatusOne { get; set; }
        public StatusTwo StatusTwo { get; set; }
        public DateTime ApprovedOneDate { get; set; }
        public DateTime ApprovedTwoDate { get; set; }
        public ActiveStatus RequestStatus { get; set; }
        public string Files { get; set; }
        public Guid NetworkId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid CostId { get; set; }
        public Guid LocationId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AccountNameId { get; set; }
        public Guid SubOpsId { get; set; }
        public Guid VacancyId { get; set; }
        public int? ApproverOneId { get; set; }
        public int ApproverTwoId { get; set; }
        public int RequestById { get; set; }
        public Guid TimeSheetTypeId { get; set; }
        public int ContractorId { get; set; }
        public int AgencyId { get; set; }
        public String Hours { get; set; }
        public String AttendanceRecordDate { get; set; }

        public int RemainingDays { get; set; }
    }

    public class WeeksList
    {
        public string Weeks { get; set; }
        public string SelectDates { get; set; }
    }
    public class DateList
    {
        public int Weeks { get; set; }
        public DateTime Date { get; set; }
        public string DateName { get; set; }
    }
}
