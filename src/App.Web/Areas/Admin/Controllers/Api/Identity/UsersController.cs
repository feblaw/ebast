using App.Domain.DTO.Identity;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace App.Web.Areas.Admin.Controllers.Api.Identity
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/users")]
    public class UsersController : BaseApiController<ApplicationUser, IUserService, ApplicationUserDto, string>
    {
        private readonly IUserService _service;
        private readonly IUserProfileService _profile;
        private readonly IUserHelper _userHelper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IUserService service,
            IUserProfileService profile,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _service = service;
            _profile = profile;
            _userHelper = userHelper;
            _userManager = userManager;
            Includes = new Expression<Func<ApplicationUser, object>>[1];
            Includes[0] = (x => x.UserProfile);
        }

      

        [Authorize(Roles = ApplicationRole.Administrator)]
        [HttpDelete("{id}")]
        public override IActionResult Delete(string id)
        {
            //var pr = _profile.GetByUserId(id);
            //var deleted =  _profile.Delete(pr);

            //ApplicationUser item = _service.GetById(id);
            //if (null == item)
            //{
            //    return Json(BadRequest());
            //}

            //_service.Delete(item);
            //return Ok(item);
            var profile = _profile.GetByUserId(id);
            profile.IsActive = false;
            _profile.Update(profile);
            return Ok(profile);
        }


    }
}
