using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class ClaimCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string Category { get; set; }
    }
    public class ClaimCategoryModelForm
    {
        public string Name { get; set; }
    }
}
