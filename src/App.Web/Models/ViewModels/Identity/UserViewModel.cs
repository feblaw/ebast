using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Identity
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public List<string> Roles { get; set; }

        public UserProfileViewModel UserProfile { get; set; }

        public string ASP { get; set; }
    }

    public class ApplicationUserForm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        public Guid ASPId { get; set; }

        public string Birthplace { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        [UIHint("SingleFileUploadCropper")]
        public string Photo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Status { get; set; }

        public string Password { get; set; }

        public string ASP { get; set; }

        [Required]
        public List<string> Roles { get; set; }
        public List<string> Company { get; set; }
    }

    public class ApplicationUserChangePassword
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Old Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string NewConfirmPassword { get; set; }
    }

    public class ChangePasswordForm
    {
        [Required]
        [Display(Name = "Old Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string NewConfirmPassword { get; set; }
    }

    public class UpdateProfileForm
    {
        [Required]
        public string Name { get; set; }

        public string Birthplace { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        [UIHint("SingleFileUploadCropper")]
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string AhId { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string ASP { get; set; }
    }

    public class VerifyEmailViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }


    public static class UserExtension
    {
        public static void Update(this ApplicationUser user, ApplicationUserForm model)
        {
            user.UserName = model.Username;
            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            user.UserProfile.Name = model.Name;
            user.UserProfile.Birthdate = model.Birthdate;
            user.UserProfile.Birthplace = model.Birthplace;
            user.UserProfile.Address = model.Address;
        }
    }
}
