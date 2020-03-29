using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Models.Core
{
    public class CostCenter: BaseModel, IEntity
    {
        public CostCenter()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }

        public virtual string DisplayName
        {
            get
            {
                return this.Code+" - "+this.Description;
            }
        }

        #region relationship

        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        #endregion
    }
}
