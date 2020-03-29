using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class PackageTypeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
    }

    public class PackageTypeyModelForm
    {
        [Required]
        public string Name { get; set; }
        public Status Status { get; set; }
    }
}
