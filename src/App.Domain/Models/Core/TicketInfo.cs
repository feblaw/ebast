using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class TicketInfo : BaseModel, IEntity
    {

        public TicketInfo()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public String Token { get; set; }

        public double Price { get; set; }

        public TicketInfoStatus Status { get; set; }

        public string Note { get; set; }

        public String Files { get; set; }

        public String Description { get; set; }

        #region relationship
        public Guid ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public virtual Claim Claim { get; set; }
        #endregion

    }

}
