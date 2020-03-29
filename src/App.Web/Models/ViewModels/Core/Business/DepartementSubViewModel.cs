using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Core;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class DepartementSubViewModel
    {
        public Guid Id { get; set; }
        public string SubName { get; set; }
        public int DsStatus { get; set; }
        public Guid DepartmentId { get; set; }
        public int LineManagerid { get; set; }
        public string DepartementName { get; set; }
        public string LineManagerName { get; set; }

    }
    public class DepartementSubModelForm
    {
        public string SubName { get; set; }
        public int DsStatus { get; set; }
        public Guid DepartmentId { get; set; }
        public  int LineManagerid { get; set; }
    }
}
