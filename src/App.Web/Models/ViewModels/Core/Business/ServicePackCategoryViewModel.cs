using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    /// <summary>
    /// SSOW Category View Model
    /// </summary>
    public class ServicePackCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public ServicePackStatus Status { get; set; }
        public string Token { get; set; }
    }
    /// <summary>
    /// SSOW Category Model Form
    /// </summary>
    public class ServicePackCategoryModelForm
    {
        [Required]
        public string Name { get; set; }
        public Level Level { get; set; }
        public ServicePackStatus Status { get; set; }
    }
}
