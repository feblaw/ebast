using App.Data.Repository;
using App.Domain.Models.Identity;

namespace App.Services.Identity
{
    public class RoleService : BaseService<ApplicationRole, IRepository<ApplicationRole>>, IRoleService
    {
        public RoleService(IRepository<ApplicationRole> repository) : base(repository)
        {
        }
    }
}
