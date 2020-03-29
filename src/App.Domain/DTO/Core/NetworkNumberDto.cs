using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Identity;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;

namespace App.Domain.DTO.Core
{
    public class NetworkNumberDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public bool IsClosed { get; set; }
        public NetworkStatus Status { get; set; }
        public Guid ProjectId { get; set; }
        public Guid DepartmentId { get; set; }
        public string LineManagerId { get; set; }
        public string ProjectManagerId { get; set; }
        public Guid AccountNameId { get; set; }
        public DepartementDto Departement { get; set; }
        public ProjectDto Project { get; set; }
        public AccountNameDto AccountName { get; set; }
        public UserProfileDto LineManager { get; set; }
        public UserProfileDto ProjectManager { get; set; }
    }
}
