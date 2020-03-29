using App.Data.Repository;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Core
{
    public class HolidaysService : BaseService<Holidays, IRepository<Holidays>>, IHolidaysService
    {
        public HolidaysService(IRepository<Holidays> repository) : base(repository)
        {
        }
    }
}
