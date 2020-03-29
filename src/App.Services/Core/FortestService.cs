using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class FortestService : BaseService<Fortest, IRepository<Fortest>>, IFortestService
    {
        public FortestService(IRepository<Fortest> repository) : base(repository)
        {
        }
    }
}
