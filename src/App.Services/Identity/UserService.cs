using System.Linq;
using App.Data.Repository;
using App.Domain.Models.Identity;

namespace App.Services.Identity
{
    public class UserService : BaseService<ApplicationUser, IRepository<ApplicationUser>>, IUserService
    {
        private readonly IUserProfileService _userProfileService;

        public UserService(IRepository<ApplicationUser> repository,
            IUserProfileService userProfileService) : base(repository)
        {
            _userProfileService = userProfileService;
        }

        public override ApplicationUser GetById(params object[] ids)
        {
            var user = base.GetById(ids);

            user.UserProfile = _userProfileService.GetByUserId(user.Id);

            return user;
        }
    }
}
