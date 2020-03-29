using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class BastViewModel
    {
        public Guid Id { get; set; }
        public string BastNo { get; set; }
        public int ApprovalOneID { get; set; }
        public int ApprovalTwoID { get; set; }
        public int ApprovalThreeID { get; set; }
        public int ApprovalFourID { get; set; }
        public int ApprovalOneStatus { get; set; }
        public int ApprovalTwoStatus { get; set; }
        public int ApprovalThreeStatus { get; set; }
        public int ApprovalFourStatus { get; set; }
        public DateTime ApprovalOneDate { get; set; }
        public DateTime ApprovalTwoDate { get; set; }
        public DateTime ApprovalThreeDate { get; set; }
        public DateTime ApprovalFourDate { get; set; }
        public string RejectionReason { get; set; }
        public decimal totalValue { get; set; }
        public string BastReqNo { get; set; }
        public int TotalDelay { get; set; }
        public string Files { get; set; }
        public string ApproverOne { get; set; }
        public string ApproverTwo { get; set; }
        public string ApproverThree { get; set; }
        public string ApproverFour { get; set; }
        public string RequestBy { get; set; }
        public string Sow { get; set; }
        public string TOP { get; set; }
        public string Asp { get; set; }
        public int AspId { get; set; }
        
    }

    public class BastFormModel
    {
        public Guid Id { get; set; }
        public string BastNo { get; set; }
        public int ApprovalOneID { get; set; }
        public int ApprovalTwoID { get; set; }
        public int ApprovalThreeID { get; set; }
        public int ApprovalFourID { get; set; }
        public int ApprovalOneStatus { get; set; }
        public int ApprovalTwoStatus { get; set; }
        public int ApprovalThreeStatus { get; set; }
        public int ApprovalFourStatus { get; set; }
        public DateTime ApprovalOneDate { get; set; }
        public DateTime ApprovalTwoDate { get; set; }
        public DateTime ApprovalThreeDate { get; set; }
        public DateTime ApprovalFourDate { get; set; }
        public string RejectionReason { get; set; }
        public decimal totalValue { get; set; }
        public string BastReqNo { get; set; }
        public int TotalDelay { get; set; }
        public string Project { get; set; }
        public string Sow { get; set; }
        public string TOP { get; set; }
        public string Files { get; set; }

    }

}
