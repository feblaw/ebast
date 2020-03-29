using App.Domain.DTO.Identity;
using App.Domain.Models.Identity;
using App.Services.Identity;
using App.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using System.Linq.Expressions;

namespace App.Web.Areas.Admin.Controllers.Api.Identity
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/userprofile")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class UserProfileControllers : BaseApiController<UserProfile, IUserProfileService, UserProfileDto, int>
    {
        public UserProfileControllers(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IUserProfileService service, IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }

        public override IActionResult PostDataTables(IDataTablesRequest request)
        {

            Includes = new Expression<Func<UserProfile, object>>[1];
            Includes[0] = pack => pack.ASP;


            var response = Service.GetDataTablesResponse<UserProfileDto>(request, Mapper, null, Includes);
              //Mapper,
              //where: $"Roles!=\"Contractor\"", Includes);

            return new DataTablesJsonResult(response, true);
        }
    }
}
