using App.Domain.DTO.Identity;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class AttendaceExceptionListDto
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
        public DateTime CreatedAt { get; set; }
        public int? ApproverOneId { get; set; }
        public int? ApproverTwoId { get; set; }

        #region Additional
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String PONumber { get; set; }
        public String Vacancy { get; set; }
        public String TimeSheetType { get; set; }
        public String Contractor { get; set; }
        public String Supplier { get; set; }
        public String ProjectManager { get; set; }
        public String LineManager { get; set; }
        public String CostCenter { get; set; }
        public String NetworkNumber { get; set; }
        public String Activiy { get; set; }
        public String Department { get; set; }
        public String DepartmentSub { get; set; }
        public String TotalHours { get; set; }
        public String DetailTimeSheet { get; set; }
        public bool IsAnnual { get; set; }
        public String Location { get; set; }
        public String OtherInfo { get; set; }
        #endregion
    }

    public class ExportExcelsDto
    {
        public Guid Id { get; set; }
        public int ContractorId { get; set; }
        public int LineManagerId { get; set; }
        public int ProjectManagerId { get; set; }
        public int AgencyId { get; set; }
        public DateTime _firstDate { get; set; }
        public DateTime _lasttDate { get; set; }
        public string NetworkCode { get; set; }
        public string NetworkDescription { get; set; }
        public string SubOpsCode { get; set; }
        public string SubOpsName { get; set; }
        public string ActCode { get; set; }
        public string ActName { get; set; }
        public string CosCenterCode { get; set; }
        public string CosCenterName { get; set; }
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ProjectName { get; set; }
        public string Organization { get; set; }
        public string Suborganization { get; set; }
        public string Location { get; set; }
        public string PMStatus { get; set; }
        public string ApprooveOneBy { get; set; }
        public string ApprooveOneDateRemark { get; set; }
        public string LMStatus { get; set; }
        public string ApprooveTwoBy { get; set; }
        public string ApprooveTwoDateRemark { get; set; }
        public string PeriodeStart { get; set; }
        public string PeriodeEnd { get; set; }
        public int RegularHours { get; set; }
        public int AnnualLeave { get; set; }
        public int NationalHoliday { get; set; }
        public int OverTime { get; set; }
        public int PPA { get; set; }
        public int Total { get; set; }
        public String TimeSheetType { get; set; }
        public string PONumber { get; set; }
    }


}
