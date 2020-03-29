using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core;
using App.Web.Models.ViewModels.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using App.Services.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class SeedingController : Controller
    {
        private readonly IWebSettingService _webSettingService;
        private readonly FileHelper _fileHelper;

        public SeedingController(IWebSettingService webSettingService,
            FileHelper fileHelper)
        {
            _webSettingService = webSettingService;
            _fileHelper = fileHelper;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Seed(SeedingViewModel model)
        {
            if(model.WebSetting)
                SeedWebSetting();

            return RedirectToAction("Index");
        }

        private void SeedWebSetting()
        {
            var webSettingJson = _fileHelper.LoadJson("/seeds/WebSetting.json");
            var data = JsonConvert.DeserializeObject<List<WebSetting>>(webSettingJson);

            foreach (var item in data)
            {
                var exist = _webSettingService.GetByName(item.Name);
                if(exist == null)
                    _webSettingService.Add(item);
            }
        }
    }
}
