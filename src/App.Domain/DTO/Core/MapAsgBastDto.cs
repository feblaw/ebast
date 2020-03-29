using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class MapAsgBastDto
    {
        public Guid Id { get; set; }
        public String Assignment { get; set; }
        public String Bast { get; set; }
        public string SiteName { get; set; }
        public string AssignmentId { get; set; }
        public string PONumber { get; set; }
        public string LineItemPO { get; set; }
        public string TOP { get; set; }
        public string BastReqNo { get; set; }
        public string BastNo { get; set; }
        public string BastStatus { get; set; }
        public string ASPName { get; set; }
        public string ReqBy { get; set; }
        public string Approver1 { get; set; }
        public string Approver2 { get; set; }
        public string Approver3 { get; set; }
        public string Approver4 { get; set; }
        public int Approver1Status { get; set; }
        public int Approver2Status { get; set; }
        public int Approver3Status { get; set; }
        public int Approver4Status { get; set; }
        public DateTime ApprovalOneDate { get; set; }
        public DateTime ApprovalTwoDate { get; set; }
        public DateTime ApprovalThreeDate { get; set; }
        public DateTime ApprovalFourDate { get; set; }
        public decimal ValueAssignment { get; set; }
        public string createdBy { get; set; }
        
    }
}
