using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{
    public class AccountName : BaseModel, IEntity
    {
        public AccountName()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

       
        public string Name { get; set; }

        
        public bool Status { get; set; }

        public string Token { get; set; }

        #region relationship

        public int Com { get; set; }

        [ForeignKey("Com")]
        public virtual UserProfile Coms { get; set; }
        #endregion

    }
}
