using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel;

namespace App.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        #region Relationships

        [DisplayName("Profile")]
        public virtual UserProfile UserProfile { get; set; }

        #endregion
    }
}
