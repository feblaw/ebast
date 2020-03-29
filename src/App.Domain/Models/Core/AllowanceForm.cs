﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class AllowanceForm : BaseModel, IEntity
    {

        public AllowanceForm()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Decimal Value { get; set; }
    }
}