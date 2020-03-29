using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class TicketInfoService : BaseService<TicketInfo, IRepository<TicketInfo>>, ITicketInfoService
    {
        public TicketInfoService(IRepository<TicketInfo> repository) : base(repository)
        {
        }
    }
}
