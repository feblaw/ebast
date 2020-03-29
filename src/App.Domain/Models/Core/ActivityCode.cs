using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class ActivityCode : BaseModel, IEntity
    {
        public ActivityCode()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public string Token { get; set; }

        public virtual string DisplayName
        {
            get
            {
                return this.Code + " - " + this.Description;
            }
        }
    }
}
