using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.DTO.Core
{
    public class PackageTypeDto
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public PackageStatus Status { get; set; }
    }
}
