using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class RequestSpareParts : BaseModel, IEntity
    {
        public RequestSpareParts()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public double Price { get; set; }

        public Guid RepiredId { get; set; }

        [Column(TypeName = "char(1)")]
        public string SpareAs { get; set; }

        [Column(TypeName = "char(1)")]
        public string SpareWdl { get; set; }

        [Column(TypeName = "char(1)")]
        public string Conpensation { get; set; }

        public bool IsSupply { get; set; }

        public double PartAppinsco { get; set; }

        public double PartQty { get; set; }

        #region relationship
        public Guid PanelCategoryId { get; set; }

        [ForeignKey("PanelCategoryId")]
        public virtual PanelCategory PanelCategory { get; set; }

        #endregion
    }
}
