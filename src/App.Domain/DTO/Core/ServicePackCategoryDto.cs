using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class ServicePackCategoryDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Level Level { get; set; }
        public ServicePackStatus Status { get; set; }
        public string Token { get; set; }
    }
}
