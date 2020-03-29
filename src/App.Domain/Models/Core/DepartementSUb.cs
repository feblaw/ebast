using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class DepartementSub:BaseModel,IEntity
    {
        public DepartementSub()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public string SubName { get; set; }

        //public int ImId { get; set; } <== ApplicationUserId
        public int DsStatus { get; set; }

        #region relationship
        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        public int LineManagerid { get; set; }

        [ForeignKey("LineManagerid")]
        public virtual UserProfile LineManager { get; set; }

        #endregion
    }
}
