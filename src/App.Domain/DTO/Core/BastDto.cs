using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class BastDto
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
        public string Project { get; set; }
        public int TotalDelay { get; set; }

        public String ASP { get; set; }
        public String TOP { get; set; }
        public String ApprovalOne { get; set; }
        public String ApprovalTwo { get; set; }
        public String ApprovalThree { get; set; }
        public String ApprovalFour { get; set; }
        public String RequestBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
