using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Identity
{
    public class UserProfile
    {
        #region Props

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(16)]
        public string IdNumber { get; set; }

        [DisplayName("FullName")]
        [MaxLength(200)]
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Birthplace { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public bool? IsActive { get; set; }
        public string Roles { get; set; }
        public string Description { get; set; }
        public string AhId { get; set; }
        public bool? IsTerminate { get; set; }
        public bool? IsBlacklist { get; set; }
        #endregion

        #region relationship
        [Column("UserId")]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public Guid ASPId { get; set; }

        [ForeignKey("ASPId")]
        public virtual ASP ASP{ get; set; }
        #endregion
    }
}
