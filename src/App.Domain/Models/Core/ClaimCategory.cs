using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class ClaimCategory : BaseModel, IEntity
    {
        public ClaimCategory()
        {
            Id = Guid.NewGuid();
            Status = Status.Active;
        }
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Status Status { get; set; }
        public string Category { get; set; }
    }
}
