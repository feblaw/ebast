using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class TicketReply : BaseModel, IEntity
    {

        public TicketReply()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime ReplyDate { get; set; }

        public string Description { get; set; }

        public string Token { get; set; }


        #region relationship
        public Guid TicketId { get; set; }

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }
        #endregion

    }
}
