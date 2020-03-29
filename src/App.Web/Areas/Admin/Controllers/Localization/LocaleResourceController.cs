using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using App.Helper;
using App.Services.Identity;
using App.Services.Localization;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Localization;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Localization
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class LocaleResourceController : BaseController<LocaleResource, ILocaleResourceService, LocaleResourceViewModel, LocaleResourceForm, Guid>
    {
        private readonly ILanguageService _languageService;

        public LocaleResourceController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IMapper mapper,
            ILocaleResourceService service,
            ILanguageService languageService,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _languageService = languageService;
        }

        public override IActionResult Index()
        {
            var languages = _languageService
                .GetAll()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

            ViewBag.Languages = languages;

            return base.Index();
        }

        public override IActionResult Create()
        {
            var model = new LocaleResourceForm()
            {
                ResourceValues = _languageService
                    .GetAll()
                    .Select(x => new ResourceValue()
                    {
                        LanguageId = x.Id,
                        LanguageName = x.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public override IActionResult Create(LocaleResourceForm model)
        {
            return NotFound();
        }

        public IActionResult EditByName(string name)
        {
            var resources = Service
                .GetResources(name);

            if (resources == null || !resources.Any())
                return NotFound();

            var model = new LocaleResourceForm()
            {
                ResourceName = name,
                ResourceValues = _languageService
                    .GetAll()
                    .Select(x => new ResourceValue()
                    {
                        LanguageId = x.Id,
                        LanguageName = x.Name
                    })
                    .ToList()
            };

            model.ResourceValues
                .ForEach(x =>
                {
                    var resource = resources
                        .FirstOrDefault(y => y.LanguageId == x.LanguageId);
                    if (resource != null)
                    {
                        x.LocaleResourceId = resource.Id;
                        x.Value = resource.ResourceValue;
                    }
                });

            return View("Edit", model);
        }

        [HttpPost]
        public override IActionResult Edit(Guid id, LocaleResourceForm model)
        {
            return NotFound();
        }
    }
}
