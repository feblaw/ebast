using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class NetworkNumberViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public NetworkStatus Status { get; set; }
        public Guid ProjectId { get; set; }
        public Guid DepartmentId { get; set; }
        public int LineManagerId { get; set; }
        public int ProjectManagerId { get; set; }
        public Guid AccountNameId { get; set; }
        public bool IsClosed { get; set; }
    }
    public class NetworkNumberModelForm
    { 
        [Required]
        public string Code { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public Guid DepartmentId { get; set; }
        [Required]
        public int LineManagerId { get; set; }
        [Required]
        public int ProjectManagerId { get; set; }
        [Required]
        public Guid AccountNameId { get; set; }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
    }
}
