using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class ServicePackCategory : BaseModel, IEntity
    {
        public ServicePackCategory()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public String Name { get; set; }

        public Level Level { get; set; }

        public ServicePackStatus Status { get; set; }

        public string Token { get; set; }
    }

   
}
