using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Identity
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string IdNumber { get; set; }
        public string Name { get; set; }
        public string Ahid { get; set; }
        public string UserName { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string Birthplace { get; set; }
        public DateTime Birthdate { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public bool IsActive { get; set; }
        public bool IsTerminate { get; set; }
        public bool IsBlacklist { get; set; }
        public string Roles { get; set; }
        public string Description { get; set; }
        public bool? IsFormerEricsson { get; set; }
        public string Attachments { get; set; }
        public Martial Martial { get; set; }
        public bool IsCandidate { get; set; }
        public bool Status { get; set; }
        public string NickName { get; set; }
        public string ASPName { get; set; }

        public Guid? ASPId { get; set; }
    }
}
