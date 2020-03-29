using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Models.Identity;

namespace App.Domain.Models.Core
{
    public class CandidateInfo : BaseModel, IEntity
    {
        public CandidateInfo()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public String Name { get; set; }
        public string NickName { get; set; }
        [Required]
        public String Email { get; set; }
        public String IdNumber { get; set; }
        public String Nationality { get; set; }
        public String PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }
        public String Address { get; set; }
        public Gender Gender { get; set; }
        public String HomePhoneNumber { get; set; }
        public String MobilePhoneNumber { get; set; }
        public String Description { get; set; }
        public string ApproveOneNotes { get; set; }
        public DateTime ApproveOneDate { get; set; }
        public string ApproveTwoNotes { get; set; }
        public DateTime ApproveTwoeDate { get; set; }
        public bool IsUser { get; set; }
        public bool IsCandidate { get; set; }
        public Martial? Martial { get; set; }
        public bool? IsFormerEricsson { get; set; }
        public string Attachments { get; set; }
        public ApproverStatus ApproveOneStatus { get; set; }
        public ApproverStatus ApproveTwoStatus { get; set; }

        public bool IsContractor { get; set; }

        #region relationshiop
        public AgencyType AgencyType { get; set; }

        public Guid VacancyId { get; set; }

        [ForeignKey("VacancyId")]
        public virtual VacancyList Vacancy { get; set; }

        [ForeignKey("RequestById")]
        public int? RequestById { get; set; }

        public virtual UserProfile RequestBy { get; set; }

        [ForeignKey("AccountId")]
        public int? AccountId { get; set; }

        public virtual UserProfile Account { get; set; }

        [ForeignKey("AgencyId")]
        public int? AgencyId { get; set; }

        public virtual UserProfile Agency { get; set; }

        public Guid? HomeBaseId { get; set; }

        [ForeignKey("HomeBaseId")]
        public virtual City HomeBase { get; set; }

        #endregion

    }
}
