using App.Domain.Models.Identity;
using App.Services.Identity;
using App.Web.Models.ViewModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Domain.Models.Core;
using Microsoft.AspNetCore.Hosting;
using App.Web.Helper;
using Microsoft.AspNetCore.Identity;
using App.Helper;

namespace App.Web.Controllers
{
    public class UserProfileController : BaseController<UserProfile, IUserProfileService, UserProfileViewModel, UserProfileFormViewModel, int>
    {
        private const string _userDir = "uploads/user/";

        private readonly IUserService _userService;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserProfileService _service;
        private readonly FileHelper _fileHelper;
        public UserProfileController(IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IHostingEnvironment env,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserProfileService service,
            IUserHelper userHelper,
            FileHelper fileHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userService = userService;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
            _service = service;
            _fileHelper = fileHelper;
        }

        public ActionResult MyProfile()
        {
            if (User.Identity.IsAuthenticated) {
                var userId = CurrentUser.Id;

                var user = _userService.GetAll(x => x.UserProfile).Where(x => x.Id == userId).FirstOrDefault();

                if (user == null)
                {
                    NotFound();
                }

                var userProfile = _service.GetById(user.UserProfile.Id);

                if (userProfile == null)
                {
                    NotFound();
                }

                UserProfileViewModel model = Mapper.Map<UserProfileViewModel>(userProfile);
                return View(model);

            }
            return RedirectToAction("Login", "Account");

        }

        [HttpPost]
        public async Task<ActionResult> MyProfile(int Id, UserProfileViewModel model)
        {
            var userId = CurrentUser.Id;

            var userProfile = Service.GetAll(x => x.ApplicationUser).Where(x => x.Id == Id).FirstOrDefault();

            if (userProfile == null)
            {
                NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.Photo != userProfile.Photo)
                {
                    var picture = JsonConvert.DeserializeObject<Attachment>(model.Photo);

                    var filePath = picture.Path;

                    var filePathCropped = _env.WebRootPath + picture.Path;
                    picture.Path = filePathCropped;

                    picture.CropedPath = _fileHelper.CreateCropped(picture);

                    model.Photo = _fileHelper.FileMove(filePath,
                               _userDir + model.Name.ToSlug() + "_" + userProfile.ApplicationUser.Id, model.Name.ToSlug());

                    model.Photo = _fileHelper.FileMove(picture.CropedPath,
                               _userDir + model.Name.ToSlug() + "_" + userProfile.ApplicationUser.Id, model.Name.ToSlug());
                }

                userProfile.Update(model);

                Service.Update(userProfile);

                var user = _userService.GetAll(x => x.UserProfile).Where(x => x.UserProfile.Id == Id).FirstOrDefault();

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    return RedirectToAction("MyProfile");
                }
            }

            return View(model);
        }
    }
}
