using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class CandidateInfoViewModel
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public string NickName { get; set; }
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
        public AgencyType AgencyType { get; set; }
        public Guid VacancyId { get; set; }
        public Guid HomeBaseId { get; set; }
    }

    public class CandidateInfoModelForm
    {
        [Required]
        public String Name { get; set; }
        public string NickName { get; set; }
        [Required]
        [EmailAddress]
        public String Email { get; set; }
        [Display(Name = "ID Number (KTP)")]
        [StringLength(16, ErrorMessage = "The {0} must be at  {2} characters long.", MinimumLength = 16)]
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
        public AgencyType AgencyType { get; set; }
        public Guid VacancyId { get; set; }
        public Guid HomeBaseId { get; set; }
    }
}
