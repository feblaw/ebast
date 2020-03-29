using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Services;
using App.Services.Identity;
using AutoMapper;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using App.Domain.Models.Identity;
using System.Security.Claims;
using System.Linq.Expressions;
using System;
using App.Helper;

namespace App.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/BaseApi")]
    public class BaseApiController<TEntity, TService, TDto, TId> : Controller
        where TEntity : class
        where TService : IService<TEntity>
        where TDto : class
    {
        protected bool IsAdministrator { get; set; }
        protected bool IsUser { get; set; }
        protected ApplicationUser CurrentUser { get; set; }
        protected IUserService UserService { get; set; }
        protected IMapper Mapper { get; set; }
        protected TService Service { get; set; }
        protected IUserHelper UserHelper { get; set; }
        protected Expression<Func<TEntity, object>>[] Includes { get; set; }
        protected string Where { get; set; }

        public BaseApiController(IHttpContextAccessor httpContextAccessor,
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
            Where = null;
            Includes = new Expression<Func<TEntity, object>>[0];

            if (httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                //var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userId = userHelper.GetUserId(httpContextAccessor.HttpContext.User);
                CurrentUser = UserService.GetById(userId);
                IsAdministrator = httpContextAccessor.HttpContext.User.IsInRole(ApplicationRole.Administrator);
                IsUser = httpContextAccessor.HttpContext.User.IsInRole(ApplicationRole.User);
            }
        }

        [HttpGet]
        [Route("Datatables")]
        public virtual IActionResult GetDataTables(IDataTablesRequest request)
        {
            var response = Service.GetDataTablesResponse<TDto>(request,
                Mapper, Where, Includes);

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("PostDatatables")]
        public virtual IActionResult PostDataTables(IDataTablesRequest request)
        {
            var response = Service.GetDataTablesResponse<TDto>(request,
                Mapper, Where, Includes);

            return new DataTablesJsonResult(response, true);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(TId id)
        {
            TEntity item = Service.GetById(id);
            if (null == item)
            {
                return Json(BadRequest());
            }

            Service.Delete(item);

            return Ok(item);
        }
    }
}