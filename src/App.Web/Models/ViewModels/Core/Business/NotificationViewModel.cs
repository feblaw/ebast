using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }

        public string LinkTo { get; set; }

        public bool IsRead { get; set; }

        public NotificationInboxStatus Status { get; set; }

        public Activities Activity { get; set; }

        public Guid EmailArviceId { get; set; }
    }

    public class NotificationInboxModelForm
    {
        public string LinkTo { get; set; }

        public bool IsRead { get; set; }

        public NotificationInboxStatus Status { get; set; }

        public Activities Activity { get; set; }

        public Guid EmailArviceId { get; set; }
    }

   
}
