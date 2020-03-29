using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Token { get; set; }

        public ProjectStatus Status { get; set; }
    }

    public class ProjectModelForm
    {

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
