using App.Data.Repository;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Core
{
    public class TacticalResourceService : BaseService<TacticalResource, IRepository<TacticalResource>>, ITacticalResourceService
    {
        public TacticalResourceService(IRepository<TacticalResource> repository) : base(repository)
        {
        }
    }
}
