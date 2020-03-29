using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;
using App.Domain.Models.Enum;

namespace App.Services.Core
{
    public class CandidateInfoService : BaseService<CandidateInfo, IRepository<CandidateInfo>>, ICandidateInfoService
    {
        public CandidateInfoService(IRepository<CandidateInfo> repository) : base(repository)
        {
        }

    }
}
