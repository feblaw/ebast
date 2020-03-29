using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using Newtonsoft.Json;

namespace App.Domain.Models.Core
{
    public class SrfRequest : BaseModel, IEntity
    {
        public SrfRequest()
        {
            Id = Guid.NewGuid();
            SpectValue = 0;
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }

        public SrfType Type { get; set; }

        public SrfApproveStatus ApproveStatusOne { get; set; } // Line Manager

        public SrfApproveStatus ApproveStatusTwo { get; set; }  // Head Of Service Line

        public SrfApproveStatus ApproveStatusThree { get; set; } // Head Of Operation

        public SrfApproveStatus ApproveStatusFour { get; set; } // Head Of Non Operation

        public SrfApproveStatus ApproveStatusFive { get; set; } // Head Of Sourcing

        public SrfApproveStatus ApproveStatusSix { get; set; } // Service Cordinator

        public DateTime? DateApproveStatusOne { get; set; } // Date Remark Line Manager

        public DateTime? DateApproveStatusTwo { get; set; }  // Date Remark Head Of Service Line

        public DateTime? DateApproveStatusThree { get; set; } // Date Remark Head Of Operation

        public DateTime? DateApproveStatusFour { get; set; } // Date Remark Head Of Non Operation

        public DateTime? DateApproveStatusFive { get; set; } // Date Remark Head Of Sourcing

        public DateTime? DateApproveStatusSix { get; set; } // Date Remark Service Cordinator

        public string NotesFirst { get; set; }

        public string NotesLast { get; set; }

        public string RequestBy { get; set; }

        public DateTime? SrfBegin { get; set; }

        public DateTime? SrfEnd { get; set; }

        public int ServiceLevel { get; set; }

        public bool isWorkstation { get; set; }

        public bool isCommunication { get; set; }

        public bool IsHrms { get; set; }

        public bool IsOps { get; set; }

        public bool IsManager { get; set; }

        public RateType RateType { get; set; }

        public string TerimnatedBy { get; set; }

        public DateTime TerminatedDate { get; set; }

        public string TeriminateNote { get; set; }

        public bool IsExtended { get; set; }

        public bool IsLocked { get; set; }

        public SrfStatus Status { get; set; }
        public decimal SpectValue { get; set; }

        public bool IsActive { get; set; }

        public int AnnualLeave { get; set; }

        #region relationship
        public Guid ServicePackId { get; set; }

        [ForeignKey("ServicePackId")]
        public virtual ServicePack ServicePack { get; set; }

        public Guid NetworkId { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkNumber NetworkNumber { get; set; }

        public Guid CostCenterId { get; set; }

        [ForeignKey("CostCenterId")]
        public virtual CostCenter CostCenter { get; set; }

        public int LineManagerId { get; set; }

        [ForeignKey("LineManagerId")]
        public virtual UserProfile LineManager { get; set; }

        public Guid? ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public virtual ActivityCode ActivityCode { get; set; }

        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        public Guid DepartmentSubId { get; set; }

        [ForeignKey("DepartmentSubId")]
        public virtual DepartementSub DepartementSub { get; set; }

        public Guid? ExtendFrom { get; set; }

        [ForeignKey("ExtendFrom")]
        public virtual SrfRequest Extend { get; set; }

        public int ProjectManagerId { get; set; }

        [ForeignKey("ProjectManagerId")]
        public virtual UserProfile ProjectManager { get; set; }


        public int? ApproveOneId { get; set; }

        [ForeignKey("ApproveOneId")]
        public virtual UserProfile ApproveOneBy { get; set; }

        public int? ApproveTwoId { get; set; }

        [ForeignKey("ApproveTwoId")]
        public virtual UserProfile ApproveTwoBy { get; set; }

        public int? ApproveThreeId { get; set; }

        [ForeignKey("ApproveThreeId")]
        public virtual UserProfile ApproveThreeBy { get; set; }

        public int? ApproveFourId { get; set; }

        [ForeignKey("ApproveFourId")]
        public virtual UserProfile ApproveFourBy { get; set; }

        public int? ApproveFiveId { get; set; }

        [ForeignKey("ApproveFiveId")]
        public virtual UserProfile ApproveFiveBy { get; set; }

        public int? ApproveSixId { get; set; }

        [ForeignKey("ApproveSixId")]
        public virtual UserProfile ApproveSixBy { get; set; }

        public Guid CandidateId { get; set; }

        [ForeignKey("CandidateId")]
        public virtual CandidateInfo Candidate { get; set; }

        public Guid? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual AccountName Account { get; set; }

        #endregion

        #region other
        public virtual SrfEscalationRequest Escalation { get; set; }
        public virtual int GetDuration()
        {
            try
            {
                int result = 0;
                if (SrfBegin.HasValue && SrfEnd.HasValue)
                {
                    DateTime lValue = SrfBegin.Value;
                    DateTime rValue = SrfEnd.Value;
                    result = Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
                }
                if (result > 0)
                {
                    return result;
                }
                else
                {
                    return 1;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return 0;
        }
        public virtual bool IsEndSoon()
        {
            try
            {
                if (this.SrfEnd.HasValue && DateTime.Now.Date == this.SrfEnd.Value.AddDays(-45).Date)
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }
        #endregion

    }



}
