using App.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Models.ViewModels.Core.Business
{
    public class TicketInfoViewModel
    {
        public Guid Id { get; set; }
        public String Token { get; set; }
        public double Price { get; set; }
        public TicketInfoStatus Status { get; set; }
        public string Note { get; set; }
        public Guid ClaimId { get; set; }
        public String Files { get; set; }
        public String Description { get; set; }
    }

    public class TicketInfoFormModel
    {
        public double Price { get; set; }
        public TicketInfoStatus Status { get; set; }
        public string Note { get; set; }
        public Guid ClaimId { get; set; }
        public String Files { get; set; }
        public String Description { get; set; }
    }
}
