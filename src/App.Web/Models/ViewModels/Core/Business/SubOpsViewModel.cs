using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class SubOpsViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Token { get; set; }

        public bool Status { get; set; }
    }

    public class SubOpsModelForm
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }

        public bool Status { get; set; }
    }
}
