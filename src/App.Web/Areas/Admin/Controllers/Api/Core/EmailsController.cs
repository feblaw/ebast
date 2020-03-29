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
using System.Collections.Generic;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/emails")]
    [Authorize]
    public class EmailsController : BaseApiController<EmailArchieve, IEmailArchieveService, EmailArchieveDto, Guid>
    {
        private readonly MailingHelper _mailHelper;
        private readonly IUserHelper _userHelper;
        private readonly IEmailArchieveService _email;

        public EmailsController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            IEmailArchieveService service,
            IUserHelper userHelper,
            MailingHelper mailHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _mailHelper = mailHelper;
            _userHelper = userHelper;
            _email = service;
        }

        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            var IdUser = _userHelper.GetUserId(User);
            var CurrentUser = _userHelper.GetUser(IdUser);

            var response = Service.GetDataTablesResponse<EmailArchieveDto>(request,
             Mapper,
             $"Tos.Equals(\"{CurrentUser.Email}\")");

            return new DataTablesJsonResult(response, true);

        }

        [HttpPost]
        [Route("SendEmail/{id}")]
        public IActionResult SendEmail(Guid id)
        {
            var email = Service.GetById(id);
            
            if(email == null)
            {
                return NotFound();
            }

            var result = _mailHelper.SendEmail(email).Result;

            return Ok(result);
        }

        [HttpGet]
        [Route("GetCurrentNotif")]
        public IActionResult GetCurrentNotif()
        {
            Dictionary<string, object> Data = new Dictionary<string, object>();
            var IdUser = _userHelper.GetUserId(User);
            var CurrentUser = _userHelper.GetUser(IdUser);
            if(CurrentUser!=null)
            {
                var result = _email.GetUnRead(CurrentUser.Email);
                Data.Add("Count", result.Count);
                Data.Add("List", result);
                return Ok(Data);
            }
            Data.Add("Count", 0);
            Data.Add("List", null);
            return Ok(Data);
        }

    }
}