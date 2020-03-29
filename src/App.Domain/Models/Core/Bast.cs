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
    public class Bast : BaseModel, IEntity
    {

        public Bast()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        public string BastNo { get; set; }
        
        public BastApproveStatus ApprovalOneStatus { get; set; }
        public BastApproveStatus ApprovalTwoStatus { get; set; }
        public BastApproveStatus ApprovalThreeStatus { get; set; }
        public BastApproveStatus ApprovalFourStatus { get; set; }
        public DateTime ApprovalOneDate { get; set; }
        public DateTime ApprovalTwoDate { get; set; }
        public DateTime ApprovalThreeDate { get; set; }
        public DateTime ApprovalFourDate { get; set; }
        public string RejectionReason  { get; set; }
        public string Project { get; set; }
        public string Sow { get; set; }
        public string TOP { get; set; }
        public string Files { get; set; }
        public Boolean BastFinal { get; set; }
        public decimal totalValue { get; set; }
        public string BastReqNo { get; set; }
        public int TotalDelay { get; set; }


        #region relationship

        public Guid AspId { get; set; }
        [ForeignKey("AspId")]
        public virtual ASP Asp { get; set; }

        public int RequestById { get; set; }

        [ForeignKey("RequestById")]
        public virtual UserProfile RequestBy { get; set; }


        public int ApprovalOneID { get; set; }
        
        [ForeignKey("ApprovalOneID")]
        public virtual UserProfile ApprovalOne { get; set; }

        public int ApprovalTwoID { get; set; }
        [ForeignKey("ApprovalTwoID")]
        public virtual UserProfile ApprovalTwo { get; set; }

        public int ApprovalThreeID { get; set; }
        [ForeignKey("ApprovalThreeID")]
        public virtual UserProfile ApprovalThree { get; set; }

        public int ApprovalFourID { get; set; }
        [ForeignKey("ApprovalFourID")]
        public virtual UserProfile ApprovalFour { get; set; }
        #endregion
    }


   

}
