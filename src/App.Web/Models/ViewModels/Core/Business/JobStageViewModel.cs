using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class JobStageViewModel
    {
        public Guid Id { get; set; }
        public string Stage { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }

    public class JobStageModelForm
    {
        [Required]
        public string Stage { get; set; }

        [Required]
        public Status Status { get; set; }
        
        public string Description { get; set; }
    }
}
