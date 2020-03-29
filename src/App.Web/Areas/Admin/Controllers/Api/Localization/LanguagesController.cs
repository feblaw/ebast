using App.Domain.DTO.Localization;
using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using App.Helper;
using App.Services.Identity;
using App.Services.Localization;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/languages")]
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class LanguagesController : BaseApiController<Language, ILanguageService, LanguageDto, int>
    {
        public LanguagesController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper,
            ILanguageService service,
            IUserHelper userHelper) 
            : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }
    }
}