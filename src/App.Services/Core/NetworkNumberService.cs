using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class NetworkNumberService : BaseService<NetworkNumber, IRepository<NetworkNumber>>, INetworkNumberService
    {
        public NetworkNumberService(IRepository<NetworkNumber> repository) : base(repository)
        {
        }
    }
}
