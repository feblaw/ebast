using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class HolidaysViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateDay { get; set; }

        public String Description { get; set; }

        public DayType DayType { get; set; }
    }

    public class HolidaysFormModel
    {
        [Required]
        public DateTime DateDay { get; set; }

        [Required]
        public String Description { get; set; }

        public DayType DayType { get; set; }
    }
}
