using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Identity;
using App.Domain.Models.Enum;

namespace App.Domain.DTO.Core
{
    public class DepartementDto :BaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int HeadId { get; set; }
        public int OperateOrNon { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
        public string HeadName { get; set; }
    }
}
