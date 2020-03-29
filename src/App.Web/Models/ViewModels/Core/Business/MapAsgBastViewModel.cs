using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class MapAsgBastViewModel
    {
        public Guid Id { get; set; }

        public string Asg { get; set; }
        public string Bast { get; set; }
        
    }

    public class MapAsgBastFormModel
    {
        public Guid Id { get; set; }

        public string Asg { get; set; }
        public string Bast { get; set; }

    }

}
