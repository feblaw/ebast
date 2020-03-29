using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using App.Services.Core.Interfaces;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class WebSettingController : BaseController<WebSetting, IWebSettingService, WebSettingViewModel, WebSettingForm, Guid>
    {
        private readonly ISrfRequestService _srf;

        public WebSettingController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IMapper mapper,
            ISrfRequestService srf,
            IWebSettingService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _srf = srf;
        }

        protected override void CreateData(WebSetting item)
        {
            item.CreatedAt = DateTime.Now;
            item.CreatedBy = CurrentUser?.Id;
        }

        protected override void UpdateData(WebSetting item, WebSettingForm model)
        {
            item.Update(model);
            item.LastUpdateTime = DateTime.Now;
            item.LastEditedBy = CurrentUser?.Id;
        }

        public IActionResult ResetNumber()
        {
            var All = _srf.GetAll().OrderBy(x=>x.CreatedAt).ToList();
            foreach(var row in All)
            {
                var temp = _srf.GetById(row.Id);
                temp.Number = null;
                _srf.Update(temp);
            }

            var After = _srf.GetAll().OrderBy(x => x.CreatedAt).ToList();

            foreach (var af in After)
            {
                var temp = _srf.GetById(af.Id);
                temp.Number = _srf.GenerateNumnber();
                _srf.Update(temp);
            }

            return Content("OK");
        }

        public IActionResult Test(string id)
        {
            string Current = int.Parse(id).ToString();
            int index = int.Parse(Current);
            int newIndex = index + 1;
            int i_number = newIndex.ToString().Length;
            string number = newIndex.ToString();
            for (int i = 4; i > i_number; i--)
            {
                number = "0" + number;
            }
            return Content(number);
        }


    }
}
