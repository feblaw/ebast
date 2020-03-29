using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class AllowanceListService : BaseService<AllowanceList, IRepository<AllowanceList>>, IAllowanceListService
    {
        public AllowanceListService(IRepository<AllowanceList> repository) : base(repository)
        {
        }
    }
}
