using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Identity;
using App.Domain.Models.Core;

namespace App.Domain.DTO.Core
{
    public class DepartementSubDto : BaseDto
    {
        public Guid Id { get; set; }
        public string SubName { get; set; }
        public int DsStatus { get; set; }
        public Guid DepartmentId { get; set; }
        public int LineManagerid { get; set; }
        public DepartementDto Departement { get; set; }
        public UserProfileDto LineManager { get; set; }

    }
}
