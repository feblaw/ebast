using App.Data.Repository;
using App.Domain.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.Models.Enum;
using App.Services.Core.Interfaces;
using MoreLinq;

namespace App.Services.Core
{
    public class ServicePackService : BaseService<ServicePack, IRepository<ServicePack>>, IServicePackService
    {
        public ServicePackService(IRepository<ServicePack> repository) : base(repository)
        {
        }

        public IQueryable GetByType(PackageTypes? types = null)
        {
            var result =  GetAllQ(x=>x.ServicePackCategory)
                .Where(x => !types.HasValue || x.Type == types /*&& x.Status == SpacateStatus.Active*/)
                .Select(x => new
                {
                    x.Id,
                    x.Type,
                    x.Name,
                    x.ServicePackCategoryId,
                    ServicePackCategoryName = x.ServicePackCategory.Name
                })
                //.OrderBy(x => x.Name)
                //.ThenBy(x => x.ServicePackCategoryName)
                //.Distinct()
                .AsQueryable();
            return result;
        }

        public IQueryable GetByCategory(string categoryId = null)
        {
            var result = GetAllQ(x=>x.ServicePackCategory)
                .Where(x => x.ServicePackCategoryId == Guid.Parse(categoryId) /*&& x.Status == SpacateStatus.Active*/)
                //.OrderBy(x => x.Name)
                .Distinct()
                .AsQueryable();
            return result;
        }

        public IQueryable GetByTypeAndCategory(PackageTypes? type = null, string categoryId = null)
        {
            var result = GetAllQ(x => x.ServicePackCategory)
                .Where(x => !type.HasValue || x.Type == type &&
                    categoryId == null || categoryId.Trim() == string.Empty ||
                    x.ServicePackCategoryId == Guid.Parse(categoryId) 
                    /*&& x.Status == SpacateStatus.Active*/)
                .Select(x=>new
                {
                    x.Id,
                    x.Type,
                    x.Name,
                    x.ServicePackCategoryId,
                    ServicePackCategoryName = x.ServicePackCategory.Name
                })
                //.OrderBy(x=>x.Name)
                //.ThenBy(x=>x.ServicePackCategoryName)
                //.DistinctBy(x=>x.ServicePackCategoryName)
                //.DistinctBy(x=>x.Name)
                ;
            
            return result.AsQueryable();
        }
    }
}
