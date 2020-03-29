using App.Domain.DTO.Localization;
using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using App.Helper;
using App.Services.Identity;
using App.Services.Localization;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Localization;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/localeresources")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class LocaleResourcesController : BaseApiController<LocaleResource, ILocaleResourceService, LocaleResourceDto, Guid>
    {
        public LocaleResourcesController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ILocaleResourceService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }

        #region Methods

        [HttpPost]
        [Route("PostDatatables/{id}")]
        public IActionResult PostDataTables(int id, IDataTablesRequest request)
        {
            var response = Service.GetDataTablesResponse<LocaleResourceDto>(request,
                Mapper,
                where: $"LanguageId.Equals({id})");

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        public IActionResult Post(LocaleResourceForm model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                foreach (var value in model.ResourceValues)
                {
                    var resource = new LocaleResource()
                    {
                        LanguageId = value.LanguageId,
                        ResourceName = model.ResourceName,
                        ResourceValue = string.IsNullOrWhiteSpace(value.Value) ? "" : value.Value,
                        CreatedAt = DateTime.Now,
                        CreatedBy = CurrentUser?.Id
                    };
                    Service.Add(resource);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public IActionResult Put(LocaleResourceForm model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                foreach (var item in model.ResourceValues)
                {
                    var source = new LocaleResource()
                    {
                        LanguageId = item.LanguageId,
                        ResourceName = model.ResourceName,
                        ResourceValue = string.IsNullOrWhiteSpace(item.Value) ? "" : item.Value,
                        CreatedAt = DateTime.Now,
                        CreatedBy = CurrentUser?.Id
                    };

                    if (item.LocaleResourceId.HasValue)
                    {
                        var resource = Service.GetById(item.LocaleResourceId);
                        resource.Update(source);
                        resource.LastEditedBy = CurrentUser?.Id;
                        resource.LastUpdateTime = DateTime.Now;
                        Service.Update(resource);
                    }
                    else
                    {
                        Service.Add(source);
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
    }
}