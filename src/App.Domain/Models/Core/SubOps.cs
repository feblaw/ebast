using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class SubOps : BaseModel, IEntity
    {

        public SubOps()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Token { get; set; }

        public bool Status { get; set; }

    }
}
