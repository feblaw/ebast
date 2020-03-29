using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class SubdivisionService : BaseService<Subdivision, IRepository<Subdivision>>, ISubdivisionService
    {
        public SubdivisionService(IRepository<Subdivision> repository) : base(repository)
        {
        }
    }
}
