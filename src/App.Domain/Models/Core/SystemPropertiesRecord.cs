using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class SystemPropertiesRecord : BaseModel, IEntity
    {

        public SystemPropertiesRecord()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string PropertyName { get; set; }

        public string PropertyValue { get; set; }

    }
}
