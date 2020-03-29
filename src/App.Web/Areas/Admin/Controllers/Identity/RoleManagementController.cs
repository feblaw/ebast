using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Identity
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class RoleManagementController : BaseController<ApplicationRole, IRoleService, RoleViewModel, RoleForm, string>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleManagementController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            IRoleService service,
            IUserHelper userHelper)
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public override IActionResult Create(RoleForm model)
        {
            if (ModelState.IsValid)
            {
                var role = Mapper.Map<ApplicationRole>(model);

                var result = _roleManager.CreateAsync(role).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddError(result.Errors);
                }
            }

            return View(model);
        }

        [HttpPost]
        public override IActionResult Edit(string id, RoleForm model)
        {
            if (ModelState.IsValid)
            {
                var role = _roleManager.FindByIdAsync(id).Result;

                if(role == null)
                {
                    return NotFound();
                }

                role.Update(model);

                var result = _roleManager.UpdateAsync(role).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("Details", new { id });
                }
                else
                {
                    AddError(result.Errors);
                }
            }

            return View(model);
        }
    }
}
