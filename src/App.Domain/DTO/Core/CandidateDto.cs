using App.Domain.DTO.Identity;
using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class CandidateDto
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
        public Martial Martial { get; set; }
        public bool IsFormerEricsson { get; set; }
        public string Attachments { get; set; }
        public ApproverStatus ApproveOneStatus { get; set; }
        public ApproverStatus ApproveTwoStatus { get; set; }
        public bool IsContractor { get; set; }
        public AgencyType AgencyType { get; set; }
        public VacancyListDto Vacancy { get; set; }
        public UserProfileDto RequestBy { get; set; }
        public UserProfileDto Account { get; set; }
        public UserProfileDto Agency { get; set; }
        public  CityDto HomeBase { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
