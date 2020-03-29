using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class AllowanceFormViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }

    public class AllowanceFormModelForm
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
