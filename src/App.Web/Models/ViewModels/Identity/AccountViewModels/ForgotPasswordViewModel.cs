using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Identity.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
