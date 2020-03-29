using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class ActivityCodeViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
    }

    public class ActivityCodeModelForm
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
    }
}
