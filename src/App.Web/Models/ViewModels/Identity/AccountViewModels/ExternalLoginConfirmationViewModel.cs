using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Identity.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
