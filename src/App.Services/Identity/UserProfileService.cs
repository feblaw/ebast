using App.Data.Repository;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Identity
{
    public class UserProfileService : BaseService<UserProfile, IRepository<UserProfile>>, IUserProfileService
    {

        public UserProfileService(IRepository<UserProfile> repository) : base(repository)
        {

        }

        public UserProfile GetByUserId(string id)
        {
            return _repository.GetAll().Where(x => x.ApplicationUserId.Equals(id)).FirstOrDefault();
        }
    }
}
