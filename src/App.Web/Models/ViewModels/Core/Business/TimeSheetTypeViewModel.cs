using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class TimeSheetTypeViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
    }
    public class TimeSheetTypeModelForm
    {
        public string Type { get; set; }
    }
}
