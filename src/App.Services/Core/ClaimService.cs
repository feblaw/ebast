using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class ClaimService : BaseService<Claim, IRepository<Claim>>, IClaimService
    {
        public ClaimService(IRepository<Claim> repository) : base(repository)
        {
        }
    }
}
