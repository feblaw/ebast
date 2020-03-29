using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Authentication;

namespace App.Domain.Models.Core
{
    public class JobStage : BaseModel, IEntity
    {
        public JobStage()
        {
            Id = Guid.NewGuid();
        }

        
        [Key]
        public Guid Id { get; set; }
        public string Stage { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }
}
