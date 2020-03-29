using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Identity
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nama")]
        public string Name { get; set; }

        [DisplayName("Tempat Lahir")]
        public string Birthplace { get; set; }

        [DisplayName("Tanggal Lahir")]
        public DateTime? Birthdate { get; set; }

        [DisplayName("Alamat")]
        public string Address { get; set; }

        [DisplayName("Foto")]
        public string Photo { get; set; }

        public string Roles { get; set; }

        public string ApplicationUserId { get; set; }

        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class UserProfileFormViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nama")]
        public string Name { get; set; }

        [DisplayName("Tempat Lahir")]
        public string Birthplace { get; set; }

        [DisplayName("Tanggal Lahir")]
        public DateTime? Birthdate { get; set; }

        [DisplayName("Alamat")]
        public string Address { get; set; }

        [DisplayName("Foto")]
        public string Photo { get; set; }

        public string Roles { get; set; }

        public string ApplicationUserId { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}
