using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class CityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }

    public class CityModelForm
    {
        [Required]
        public string Name { get; set; }
        public Status Status { get; set; }
    }
}
