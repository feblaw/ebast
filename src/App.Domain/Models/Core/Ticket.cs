using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class Ticket : BaseModel, IEntity
    {

        public Ticket()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime TicketDate { get; set; }

        public string Token { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public TicketStatus Status { get; set; }

        public bool IsArchive { get; set; }

    }

  

    
}
