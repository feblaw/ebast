using App.Domain.Models.Identity;
using App.Web.Models.ViewModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace App.Web.Helper
{
    public static class UserHelper
    {
        public static void Update(this UserProfile user, UserProfileViewModel model)
        {
            user.Name = model.Name;
            user.Address = model.Address;
            user.Photo = model.Photo;
            user.Birthdate = model.Birthdate;
            user.Birthplace = model.Birthplace;
        }

        public static bool IsAdministrator(this IPrincipal user)
        {

            if (user.IsInRole(ApplicationRole.Administrator))
            {
                return true;
            }

            return false;
        }

        public static bool IsUser(this IPrincipal user)
        {
            if (user.IsInRole(ApplicationRole.User))
            {
                return true;
            }

            return false;
        }
    }
}
