using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using App.Services.Identity;
using System.Security.Claims;
using App.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using App.Helper;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Controllers
{
    public class BaseController<TEntity, TService, TViewModel, TModelForm, TId> : Controller
        where TEntity : class
        where TService : IService<TEntity>
        where TModelForm : class
        where TViewModel : class
    {
        protected bool IsAdministrator { get; set; }
        protected bool IsUser { get; set; }
        protected ApplicationUser CurrentUser { get; set; }
        protected IUserService UserService { get; set; }
        protected IUserHelper UserHelper { get; set; }
        protected IMapper Mapper { get; set; }
        protected TService Service { get; set; }

        public BaseController(IHttpContextAccessor httpContextAccessor,
           IUserService userService,
           IMapper mapper,
           TService service,
           IUserHelper userHelper)
        {
            UserService = userService;
            UserHelper = userHelper;
            Mapper = mapper;
            IsAdministrator = false;
            IsUser = false;
            CurrentUser = null;
            Service = service;

            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                //if didn't use SSO use----------------------------------------------------------
                // var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //------------------------------------------------------------------------
                //if using SSO use----------------------------------------------------------
                var userId = UserHelper.GetUserId(httpContextAccessor.HttpContext.User);
                //------------------------------------------------------------------------
                CurrentUser = UserService.GetById(userId);
                IsAdministrator = httpContextAccessor.HttpContext.User.IsInRole(ApplicationRole.Administrator);
                IsUser = httpContextAccessor.HttpContext.User.IsInRole(ApplicationRole.User);
            }
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult lmrtemplate()
        {
            return View();
        }

        public virtual IActionResult Details(TId id)
        {
            var item = Service.GetById(id);
            if (item != null)
            {
                return View(Mapper.Map<TViewModel>(item));
            }

            return NotFound();
        }

        public virtual IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public virtual IActionResult Create(TModelForm model)
        {
            if (ModelState.IsValid)
            {
                var item = Mapper.Map<TEntity>(model);

                Upload(item,model);

                CreateData(item);

                Service.Add(item);

                AfterCreateData(item);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public virtual IActionResult Edit(TId id)
        {
            var item = Service.GetById(id);
            if (item != null)
            {
                return View(Mapper.Map<TModelForm>(item));
            }

            return NotFound();
        }

        public virtual IActionResult BastEditor(TId id)
        {
            var item = Service.GetById(id);
            if (item != null)
            {
                return View(Mapper.Map<TModelForm>(item));
            }

            return NotFound();
        }

        [HttpPost]
        public virtual IActionResult Edit(TId id, TModelForm model)
        {
            if (ModelState.IsValid)
            {
                var item = Service.GetById(id);

                if (item == null) return NotFound();

                var before = item;

                if (ProperUpdateImplemented())
                {
                    var updated = UpdateDataProper(item, model);

                    Service.Detach(item);

                    var result = Service.Update(updated);

                    AfterUpdateData(before, result);
                }
                else
                {
                    UpdateData(item, model);

                    var result = Service.Update(item);

                    AfterUpdateData(before, item);
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        protected virtual void Upload(TEntity item,TModelForm model)
        {
        }

        public virtual IActionResult Import()
        {
            return View();
        }

        protected virtual void CreateData(TEntity item)
        {
        }

        protected virtual void AfterCreateData(TEntity item)
        {
            TempData["Success"] = "Success";
        }

        /// <summary>
        /// Original implementation of entity update, which does not leverage AutoMapper, thus
        /// the actual value change(s) is handled MANUALLY in the override and riddled with missing changes
        /// (new value sent from the form [model] is not applied to the corresponding prop in the entity).
        /// Running out of time to do full fix to all entities (which requires full compliance check between
        /// entity class, form model class, and the edit form for each entities), hence this kludge
        /// (cf. BaseController.Edit(), BaseController.ProperUpdateImplemented(), and BaseController.UpdateDataProper()).
        /// Please check the commit diff/log of this patch.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="model"></param>
        [Obsolete]
        protected virtual void UpdateData(TEntity item, TModelForm model) { }

        protected virtual bool ProperUpdateImplemented() => false;

        protected virtual TEntity UpdateDataProper(TEntity item, TModelForm model) => Mapper.Map<TEntity>(model);

        protected virtual void AfterUpdateData(TEntity before, TEntity after)
        {
            TempData["Success"] = "Success";
        }

        protected virtual void AddError(IEnumerable<IdentityError> errors)
        {
            var errorString = "";
            var first = true;
            var last = errors.Last();
            foreach (var error in errors)
            {
                if (!first)
                {
                    errorString += "<li>";
                }

                errorString += $"{error.Description}";

                if(error.Code != last.Code)
                {
                    errorString += "</li>";
                }

                first = false;
            }
            ModelState.AddModelError("", errorString);
        }
        
    }
}
