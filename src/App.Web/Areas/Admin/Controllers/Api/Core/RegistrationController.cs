using App.Domain.DTO.Core;
using App.Domain.Models.Core;
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
using System.Linq.Expressions;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using App.Services.Utils;
using System.Linq.Dynamic.Core;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{

    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/registration")]
    [Authorize]
    public class RegistrationController : BaseApiController<AttendaceExceptionList, IAttendaceExceptionListService, AttendaceExceptionListDto, Guid>
    {
        private readonly IUserHelper _userHelper;
        private readonly ISrfRequestService _srf;


        public RegistrationController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IAttendaceExceptionListService service,
            ISrfRequestService srf,
            IUserHelper userHelper) :
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userHelper = userHelper;
            _srf = srf;
        }

       

        [HttpGet]
        [Route("GetTimeSheet")]
        public IActionResult GetTimeSheet(IDataTablesRequest request, string datefirst = null, string dateend = null)
        {
            var searchQuery = request.Search.Value;
            var userId = _userHelper.GetUser(User).UserProfile.Id;

            Includes = new Expression<Func<AttendaceExceptionList, object>>[4];
            Includes[0] = pack => pack.Contractor;
            Includes[1] = pack => pack.ApproverOne;
            Includes[2] = pack => pack.ApproverTwo;
            Includes[3] = pack => pack.Vacancy;
            //Includes[4] = pack => pack.Vacancy.PONumber;
            //Includes[3] = pack => pack.CostCenter;
            //Includes[4] = pack => pack.Network;
            //Includes[5] = pack => pack.Activity;
            //Includes[6] = pack => pack.Departement;
            //Includes[7] = pack => pack.DepartementSub;
            //Includes[8] = pack => pack.Contractor;
            //Includes[9] = pack => pack.TimeSheetType;
            //Includes[10] = pack => pack.Record;
            //Includes[11] = pack => pack.Location;

            var data = Service.GetAllQ(Includes);

            DateTime dStartd, dEnd;
            if (!string.IsNullOrEmpty(datefirst) && !string.IsNullOrEmpty(dateend)
                && DateTime.TryParseExact(datefirst, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dStartd)
                && DateTime.TryParseExact(dateend, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dEnd))
            {
                data = data.Where(x => x.DateStart >= dStartd && x.DateEnd <= dEnd);
                #region Filter
                if (User.IsInRole("HR Agency"))
                    data = data.Where(x => x.ContractorId == userId && x.DateStart >= dStartd && x.DateEnd <= dEnd);
                else if (User.IsInRole("Regional Project Manager") || (User.IsInRole("Head Of Service Line")))
                    data = data.Where(x => (x.ApproverOneId == userId || x.ApproverTwoId == userId) && (x.DateStart >= dStartd && x.DateEnd <= dEnd));
                //else if (User.IsInRole("HR Agency"))
                //    data = data.Where(x => x.AgencyId == userId && x.DateStart >= dStartd && x.DateEnd <= dEnd);
                #endregion
            }
            else
            {
                #region Filter
                if (User.IsInRole("HR Agency"))
                    data = data.Where(x => x.ContractorId == userId);
                else if (User.IsInRole("Regional Project Manager") || (User.IsInRole("Head Of Service Line")))
                    data = data.Where(x => x.ApproverOneId == userId || x.ApproverTwoId == userId);
                //else if (User.IsInRole("HR Agency"))
                //    data = data.Where(x => x.AgencyId == userId);
                #endregion
            }

            var filteredData = data.AsQueryable();

            var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

            if (!string.IsNullOrEmpty(globalFilter))
                filteredData = data.Where(globalFilter);

            var filter = request.Columns.GetFilter();

            if (!string.IsNullOrEmpty(filter))
                filteredData = filteredData.Where(filter);

            var sort = request.Columns.GetSort();

            if (User.IsInRole("Contractor"))
                sort = "createdAt desc";


            if (!string.IsNullOrEmpty(sort))
                filteredData = filteredData.OrderBy(sort);

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            var response = DataTablesResponse.Create(request,
                data.Count(),
                filteredData.Count(),
                Mapper.Map<List<AttendaceExceptionListDto>>(dataPage));

            return new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        [Route("GetRemaining")]
        public IActionResult GetRemaining(String hours, String id)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(hours))
            {
                var data = _userHelper.GetCurrentSrfByUserProfile(int.Parse(id));
                if (data != null)
                {
                    int TotalHours = int.Parse(hours);
                    var TotalAnnual = Math.Ceiling((decimal)TotalHours / 24);
                    var Remaining = data.AnnualLeave - TotalAnnual;
                    return Json(new { val = Remaining });
                }
            }
            return Json(new { val = 0});
        }

        [HttpGet]
        [Route("CheckingDate")]
        public IActionResult CheckingDate(string DateFirst = null, string DateLast = null, string id = null)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            var Annual = _userHelper.GetCurrentSrfByLogin(User).AnnualLeave;

            DateTime First = DateTime.Parse(DateFirst);
            DateTime Last = DateTime.Parse(DateLast);
            double total_day = ((Last - First).TotalDays) + 1;
            total_day = total_day < 1 ? 0 : total_day;

            if (!string.IsNullOrEmpty(DateFirst) && !string.IsNullOrEmpty(DateLast))
            {
                var Checking = Service.GetAll().Where(x => x.DateStart.Date.ToString("yyyy-MM-dd").Equals(DateFirst) && x.DateEnd.Date.ToString("yyyy-MM-dd").Equals(DateLast) && x.ContractorId == PreofileId).FirstOrDefault();
                if (!string.IsNullOrEmpty(id))
                {
                    Checking = Service.GetAll().Where(x => x.DateStart.Date.ToString("yyyy-MM-dd").Equals(DateFirst) && x.DateEnd.Date.ToString("yyyy-MM-dd").Equals(DateLast) && x.Id != Guid.Parse(id) && x.ContractorId == PreofileId).FirstOrDefault();
                }
                if (Checking == null)
                {
                    if (total_day <= Annual && total_day != 0)
                    {
                        return Json(new { message = true, total_day = total_day, annual = Annual });
                    }
                    else
                    {
                        return Json(new { message = false, total_day = total_day, annual = Annual });
                    }
                }
                else
                {
                    return Json(new { message = false, total_day = total_day, annual = Annual });
                }
            }
            return Json(new { message = false, total_day = total_day, annual = Annual });
        }


    }
}
