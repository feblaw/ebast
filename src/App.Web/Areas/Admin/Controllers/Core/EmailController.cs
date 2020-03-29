using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using App.Services.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class EmailController : BaseController<EmailArchieve, IEmailArchieveService, EmailArchieveDto, EmailArchieveDto, Guid>
    {
        public EmailController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IMapper mapper,
            IEmailArchieveService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }

        public override IActionResult Details(Guid id)
        {
            var item = Service.GetById(id);
            item.IsRead = true;
            Service.Update(item);
            return Redirect(item.LinkTo);
        }

        public override IActionResult Create()
        {
            return NotFound();
        }

        [HttpPost]
        public override IActionResult Create(EmailArchieveDto model)
        {
            return NotFound();
        }

        public override IActionResult Edit(Guid id)
        {
            return NotFound();
        }

        [HttpPost]
        public override IActionResult Edit(Guid id, EmailArchieveDto model)
        {
            return NotFound();
        }
    }
}
