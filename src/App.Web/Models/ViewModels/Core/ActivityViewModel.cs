using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core
{
    public class ActivityViewModel
    {

        public UserProfile User { get; set; }

        public string Subject { get; set; }

        public string CallbackUrl { get; set; }

        public string Description { get; set; }
    }
}
