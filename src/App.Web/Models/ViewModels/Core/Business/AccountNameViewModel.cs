using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class AccountNameViewModel
    {
        public Guid Id { get; set; }


        public string Name { get; set; }

        public string Com { get; set; }

        public bool Status { get; set; }

        public string Token { get; set; }
    }

    public class AccountNameModelForm
    {
        [Required]
        public string Name { get; set; }
        public int Com { get; set; }
        public bool Status { get; set; }


    }
}
