using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class WoItem : BaseModel, IEntity
    {

        public WoItem()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string PartCode { get; set; }

        public WoItemType Type { get; set; }

        public String ItemId { get; set; }

        public string Name { get; set; }

        public int Qty { get; set; }

        public double Price { get; set; }

        public double Disc { get; set; }

        public String WoNumber { get; set; }

    }

}
