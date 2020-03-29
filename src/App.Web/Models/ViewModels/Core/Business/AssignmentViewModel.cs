using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class AssignmentViewModel
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string SiteID { get; set; }
        public string SiteName { get; set; }
        public string AssignmentId { get; set; }
        public DateTime AssignmentCreateDate { get; set; }
        public DateTime AssignmentAcceptedDate { get; set; }
        public string PRNo { get; set; }
        public DateTime PRDateCreated { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string LineItemPO { get; set; }
        public bool AssignmentReady4Bast { get; set; }
        public string ShortTextPO { get; set; }
        public decimal ValueAssignment { get; set; }
        public string SHID { get; set; }
        public string TOP { get; set; }
        public string AssignmentCreateBy { get; set; }
        public bool AssignmentCancel { get; set; }

        public string Asp { get; set; }
        public int AspId { get; set; }

        //public string TerminateBy { get; set; }
        //public string TerminateNote { get; set; }
        //public DateTime TerminateDate { get; set; }
        //public string Description { get; set; }
        //public string Token { get; set; }
        //public SrfApproveStatus StatusOne { get; set; }
        //public SrfApproveStatus StatusTwo { get; set; }
        //public SrfApproveStatus StatusThree { get; set; }
        //public SrfApproveStatus StatusFourth { get; set; }
        //public DateTime DateApprovedOne { get; set; }
        //public DateTime DateApprovedTwo { get; set; }
        //public DateTime DateApprovedThree { get; set; }
        //public DateTime DateApprovedFour { get; set; }
        //public DateTime JoinDate { get; set; }
        //public SrfStatus Status { get; set; }
        //public int OtLevel { get; set; }
        //public bool isLaptop { get; set; }
        //public bool isUsim { get; set; }
        //public decimal NoarmalRate { get; set; }
        //public bool isManager { get; set; }
        //public bool isHrms { get; set; }
        //public int RequestById { get; set; }
        public string Files { get; set; }
        //public Guid ServicePackId { get; set; }
        //public string ServicePackName { get; set; }
        //public Guid ServicePackCategoryId { get; set; }
        //public string ServicePackCategoryName { get; set; }
        //public Guid DepartmentId { get; set; }
        //public string DepartmentName { get; set; }
        //public Guid DepartmentSubId { get; set; }
        //public string DepartmentSubName { get; set; }
        //public Guid CostCodeId { get; set; }
        //public string CostCodeName { get; set; }
        //public Guid NetworkId { get; set; }
        //public string NetworkName{ get; set; }
        //public Guid AccountNameId { get; set; }
        //public string AccountName { get; set; }
        //public Guid JobStageId { get; set; }
        //public string JobStageName { get; set; }
        //public int ApproverOneId { get; set; }
        //public string ApproverOne { get; set; }
        //public int ApproverTwoId { get; set; }
        //public string ApproverTwo { get; set; }
        //public int RpmId { get; set; }
        //public string Rpm { get; set; }
        //public int ApproverThreeId { get; set; }
        //public string ApproverThree { get; set; }
        //public int ApproverFourId { get; set; }
        //public string ApproverFour { get; set; }
        //public Guid PackageTypeId { get; set; }
        //public string PackageTypeName { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public ApproverStatus VacancyStatus { get; set; }
        //public DateTime StartDate { get; set; }

        //public DateTime EndDate { get; set; }
        //public string Name { get; set; }
        //public string NIK { get; set; }

        //public string Identifier { get; set; }

        //public decimal Quantity { get; set; }
        //public int? VendorId { get; set; }
        //public string Vendor { get; set; }
        //public string PONumber { get; set; }
        //public string POInsertedBy { get; set; }
        //public DateTime POInsertDate { get; set; }
    }

    public class AssignmentFormModel
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string SiteID { get; set; }
        public string SiteName { get; set; }
        public string AssignmentId { get; set; }
        public DateTime AssignmentCreateDate { get; set; }
        public DateTime AssignmentAcceptedDate { get; set; }
        public string PRNo { get; set; }
        public DateTime PRDateCreated { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string LineItemPO { get; set; }
        public bool AssignmentReady4Bast { get; set; }
        public string ShortTextPO { get; set; }
        public decimal ValueAssignment { get; set; }
        public string SHID { get; set; }
        public string TOP { get; set; }
        public string AssignmentCreateBy { get; set; }
        public bool AssignmentCancel { get; set; }
        public string Files { get; set; }

    }

}
