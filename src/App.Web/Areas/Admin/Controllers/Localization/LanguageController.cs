using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using App.Helper;
using App.Services.Identity;
using App.Services.Localization;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core;
using App.Web.Models.ViewModels.Localization;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Localization
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class LanguageController : BaseController<Language, ILanguageService, LanguageViewModel, LanguageForm, int>
    {
        private readonly FileHelper _fileHelper;

        public LanguageController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IMapper mapper,
            ILanguageService service,
            FileHelper fileHelper,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _fileHelper = fileHelper;
        }

        public override IActionResult Create()
        {
            ViewBag.FlagImages = _fileHelper.LoadFlags();

            return base.Create();
        }

        [HttpPost]
        public override IActionResult Create(LanguageForm model)
        {
            ViewBag.FlagImages = _fileHelper.LoadFlags();

            return base.Create(model);
        }

        public override IActionResult Edit(int id)
        {
            ViewBag.FlagImages = _fileHelper.LoadFlags();

            return base.Edit(id);
        }

        [HttpPost]
        public override IActionResult Edit(int id, LanguageForm model)
        {
            ViewBag.FlagImages = _fileHelper.LoadFlags();

            return base.Edit(id, model);
        }

        protected override void CreateData(Language item)
        {
            item.CreatedAt = DateTime.Now;
            item.CreatedBy = CurrentUser?.Id;
        }

        protected override void UpdateData(Language item, LanguageForm model)
        {
            item.Update(model);
            item.LastUpdateTime = DateTime.Now;
            item.LastEditedBy = CurrentUser?.Id;
        }
    }
}
