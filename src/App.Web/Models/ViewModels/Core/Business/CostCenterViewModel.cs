using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class CostCenterViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
        public Guid DepartmentId { get; set; }
    }
    public class CostCenterModelForm
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        public Status Status { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
