﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using Org.BouncyCastle.Ocsp;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class DepartementViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int HeadId { get; set; }
        public int OperateOrNon { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public Status Status { get; set; }
    }
    public class DepartementModelForm
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int HeadId { get; set; }
        [Required]
        public int OperateOrNon { get; set; }
        public string Description { get; set; }

    }
}
