using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class NetworkNumber : BaseModel, IEntity
    {
        public NetworkNumber()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public String Code { get; set; }

        public String Description { get; set; }

        public string Token { get; set; }

        public NetworkStatus Status { get; set; }

        public bool IsClosed { get; set; }

        public virtual string DisplayName
        {
            get
            {
                return this.Code + " - " + this.Description;
            }
        }

        #region relationship

        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; }

        public Guid DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Departement Departement { get; set; }

        public int LineManagerId { get; set; }

        [ForeignKey("LineManagerId")]
        public virtual UserProfile LineManager { get; set; }

        public int ProjectManagerId { get; set; }

        [ForeignKey("ProjectManagerId")]
        public virtual UserProfile ProjectManager { get; set; }

        public Guid AccountNameId { get; set; }

        [ForeignKey("AccountNameId")]
        public virtual AccountName AccountName { get; set; }

        #endregion

    }

    
}
