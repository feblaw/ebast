using App.Domain.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain
{
    public class BaseModel
    {
        #region Props

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string LastEditedBy { get; set; }

        public string OtherInfo { get; set; }

        public string CustomField1 { get; set; }

        public string CustomField2 { get; set; }

        public string CustomField3 { get; set; }

        #endregion

        #region Relationships

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Creator { get; set; }

        [ForeignKey("LastEditedBy")]
        public virtual ApplicationUser Editor { get; set; }

        #endregion
    }
}
