using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Domain.Models.Core
{
    public class WotList : BaseModel, IEntity
    {

        public WotList()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime WotDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }

        public bool ApproveOne { get; set; }

        public bool ApproveTwo { get; set; }

        public string Token { get; set; }

        public WotStatus StatusOne { get; set; }

        public WotStatus StatusTwo { get; set; }

        public DateTime AddDate { get; set; }

        public WotStatusFinal Status { get; set; }
    }

   
}
