using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;

namespace App.Domain.Models.Core
{
    public class MapAsgBast : BaseModel, IEntity
    {

        public MapAsgBast()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        public string StatusApi { get; set; }


        #region relationship

        public Guid IdAsg { get; set; }
        [ForeignKey("IdAsg")]
        public virtual Assignment Assignment { get; set; }

        public Guid IdBast { get; set; }
        [ForeignKey("IdBast")]
        public virtual Bast Bast { get; set; }

        #endregion
    }




}
