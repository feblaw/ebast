using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Identity;
using App.Domain.Models.Identity;

namespace App.Domain.DTO.Core
{
    public class AccountNameDto : BaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Com { get; set; }
        public bool Status { get; set; }
        public string Token { get; set; }
        public int ComId { get; set; }
        public string ComName { get; set; }
    }
}
