using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using DataTables.AspNet.Core;
using System.Linq.Expressions;
using DataTables.AspNet.AspNetCore;
using App.Web.Models.ViewModels.Core.Business;
using App.Services.Utils;
using System.Linq.Dynamic.Core;
using App.Domain.Models.Enum;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/tacticalresource")]
    [Authorize]
    public class TacticalResourceController : BaseApiController<TacticalResource, ITacticalResourceService, TacticalResourceDto, Guid>
    {
        private readonly IUserHelper _userHelper;
        private readonly ISrfRequestService _srf;

        public TacticalResourceController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            ITacticalResourceService service,
            ISrfRequestService srf,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userHelper = userHelper;
            _srf = srf;
        }



        [Route("GetData")]
        [HttpPost]
        public IActionResult GetData(IDataTablesRequest request, string month = null, string year = null)
        {
            Expression<Func<TacticalResource, Object>>[] includes = { (x => x.Departement), (x => x.DepartementSub) };

            var FilterDate = "";
            if (!string.IsNullOrWhiteSpace(month) && !string.IsNullOrWhiteSpace(year))
            {
                int Month = int.Parse(month);
                int Year = int.Parse(year);
                var lastDayOfMonth = DateTime.DaysInMonth(Year, Month);
                var First = Year + "-" + Month + "-01";
                var Last = Year + "-" + Month + "-" + lastDayOfMonth;
                FilterDate = string.Format(@" DateSrf >= Convert.ToDateTime(""{0}"") &&  DateSrf <= Convert.ToDateTime(""{1}"")   ", DateTime.Parse(First), DateTime.Parse(Last));
            }

            var response = Service.GetDataTablesResponse<TacticalResourceDto>(request,
                Mapper, FilterDate, includes);

            return new DataTablesJsonResult(response, true);
        }

        [Route("GetByRole")]
        [HttpPost]
        public IActionResult GetByRole(IDataTablesRequest request)
        {

            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            Expression<Func<TacticalResource, object>>[] Includes = new Expression<Func<TacticalResource, object>>[2];
            Includes[0] = pack => pack.Departement;
            Includes[1] = pack => pack.DepartementSub;


            #region FilterByApprover
           
            if (User.IsInRole("Head Of Operation") || User.IsInRole("Head Of Non Operation"))
            {
                var headop = Service
                   .GetAll(Includes)
                   .Where(x =>  x.DateSrf.HasValue && !x.DepartmentSubId.HasValue && x.Departement.HeadId == PreofileId )
                   // tadinya pake yang bawah, kemudian diganti karena error di Head Ops 22/5/2019
                   //.Where(x => x.DateSrf.HasValue && (!x.DepartmentSubId.HasValue && x.Departement.HeadId == PreofileId) || (x.Departement.HeadId == PreofileId && x.DepartmentSubId.HasValue && x.DepartementSub.DepartmentId == x.DepartmentId))
                   .OrderByDescending(x=>x.DateSrf)
                   .Select(x => new
                   {
                       CountSrf = _srf.ActualSrf(PreofileId,x.DateSrf.Value, true,(x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null), User),
                       Approved = x.Approved,
                       Progress = _srf.ActualSrf(PreofileId, x.DateSrf.Value, false,(x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null), User),
                       Forecast = x.Forecast,
                       DateSrf = x.DateSrf,
                       Department = x.Departement.Name,
                       DepartementSub = x.DepartmentSubId.HasValue ? x.DepartementSub.SubName : null,
                   });
                return CreateData(request, headop.AsQueryable());
            }

            if (User.IsInRole("Line Manager"))
            {
                var serline = Service
                   .GetAll(Includes)
                   .Where(x => x.DateSrf.HasValue && x.DepartmentSubId.HasValue && x.DepartementSub.LineManagerid == PreofileId)
                   .OrderByDescending(x => x.DateSrf)
                   .Select(x => new
                   {
                       CountSrf = _srf.ActualSrf(PreofileId, x.DateSrf.Value,true, (x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null),  User),
                       Approved = x.Approved,
                       Progress = _srf.ActualSrf(PreofileId, x.DateSrf.Value,false, (x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null), User),
                       Forecast = x.Forecast,
                       DateSrf = x.DateSrf,
                       Department = x.Departement.Name,
                       DepartementSub = x.DepartmentSubId.HasValue ? x.DepartementSub.SubName : null,
                   });
                return CreateData(request, serline.AsQueryable());
            }

            var data = Service
                .GetAll(Includes)
                .Where(x => x.DateSrf.HasValue)
                .OrderByDescending(x => x.DateSrf)
                .Select(x => new
                {
                    CountSrf = _srf.ActualSrf(PreofileId, x.DateSrf.Value,true, (x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null), User),
                    Approved = x.Approved,
                    Progress = _srf.ActualSrf(PreofileId, x.DateSrf.Value, false, (x.DepartmentId.HasValue ? x.DepartmentId : null), (x.DepartmentSubId.HasValue ? x.DepartmentSubId : null), User),
                    Forecast = x.Forecast,
                    DateSrf =  x.DateSrf,
                    Department = x.Departement.Name,
                    DepartementSub = x.DepartmentSubId.HasValue ? x.DepartementSub.SubName : null,
                });

            return CreateData(request, data.AsQueryable());


            #endregion

        }

        private DataTablesJsonResult CreateData(IDataTablesRequest request, IQueryable data)
        {
           
            var searchQuery = request.Search.Value;

            var filteredData = data.AsQueryable();

            var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

            if (!string.IsNullOrEmpty(globalFilter))
                filteredData = filteredData.Where(globalFilter);

            var filter = request.Columns.GetFilter();

            if (!string.IsNullOrEmpty(filter))
                filteredData = filteredData.Where(filter);

            //var sort = request.Columns.GetSort();

            //if (!string.IsNullOrEmpty(sort))
           //filteredData = filteredData.OrderBy(sort);

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            var response = DataTablesResponse.Create(request,
                data.Count(),
                filteredData.Count(),
                dataPage);

            return new DataTablesJsonResult(response, true);
        }

        public override IActionResult Delete(Guid id)
        {
            var item = Service.GetById(id);
            if(item!=null)
            {
                item.CountSrf = 0;
                item.Approved = 0;
                item.Forecast = 0;
                item.DateSrf = null;
                Service.Update(item);
                return Ok(item);
            }
            return Ok(false);
        }

    }
}
