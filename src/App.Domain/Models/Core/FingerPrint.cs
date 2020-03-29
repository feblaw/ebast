using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class FingerPrint : BaseModel, IEntity
    {
        public FingerPrint()
        {
            Id =Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Ip { get; set; }
        public string Key { get; set; }
        public bool IsEnabled { get; set; }

    }
}
