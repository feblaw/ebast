using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class PackageType : BaseModel, IEntity
    {
        public PackageType()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public String Name { get; set; }

        public PackageStatus Status { get; set; }
    }

   
}
