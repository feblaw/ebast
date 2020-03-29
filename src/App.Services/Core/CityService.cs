using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services.Core.Interfaces;

namespace App.Services.Core
{
    public class CityService : BaseService<City, IRepository<City>>, ICityService
    {
       
        public CityService(IRepository<City> repository) : base(repository)
        {
        }
    }
}
