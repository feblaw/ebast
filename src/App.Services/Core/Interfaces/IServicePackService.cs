using System.Linq;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;

namespace App.Services.Core.Interfaces
{
    public interface IServicePackService : IService<ServicePack>
    {
        IQueryable GetByType(PackageTypes? types = null);
        IQueryable GetByCategory(string categoryId = null);
        IQueryable GetByTypeAndCategory(PackageTypes? type = null, string categoryId = null);
    }
}
 