using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Models.Identity;

namespace App.Domain.Models.Core
{
    public class Departement :BaseModel,IEntity
    {
        public Departement()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int OperateOrNon { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }

        #region  Relationship
        public int HeadId { get; set; }

        [ForeignKey("HeadId")]
        public virtual UserProfile Head { get; set; }
        #endregion

       

    }
}
