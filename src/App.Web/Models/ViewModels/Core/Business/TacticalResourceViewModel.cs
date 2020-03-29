using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class TacticalResourceViewModel
    {
        public Guid Id { get; set; }
        public int Approved { get; set; }
        public int Forecast { get; set; }
        public int CountSrf { get; set; }
        public DateTime DateSrf { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentSubName { get; set; }

        public int SrfOnProses { get; set; }
    }

    public class TacticalResourceFormModel
    {
        public int Approved { get; set; }
        public int Forecast { get; set; }
        public int CountSrf { get; set; }
        public DateTime DateSrf { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid DepartmentSubId { get; set; }
    }
}
