﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class Fortest : BaseModel, IEntity
    {
        public Fortest()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
