using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using System;
using System.Collections.Generic;

namespace App.Services.Identity
{
    public interface IUserProfileService : IService<UserProfile>
    {
        UserProfile GetByUserId(string id);
    }
}
