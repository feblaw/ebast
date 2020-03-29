using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class ASP : BaseModel, IEntity
    {
        public ASP()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }

    }
}
