using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    public class ReconcileController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {
        public ReconcileController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISrfRequestService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
        }

        public override IActionResult Index()
        {
            var FirstRecord = Service.GetAll().Where(x=>x.SrfBegin.HasValue).OrderBy(x => x.SrfBegin).FirstOrDefault();
            var FirstYears = FirstRecord.SrfBegin.Value.Year;
            var LastRecord = Service.GetAll().Where(x => x.SrfEnd.HasValue).OrderByDescending(x => x.SrfEnd).FirstOrDefault();
            var LastYears = LastRecord.SrfEnd.Value.Year;
            var OptionYears = "<option selected disabled>-- Select Years--</option>";
            for(int i= LastYears; i>= FirstYears; i--)
            {
                OptionYears += "<option value='"+ LastYears + "'>" + LastYears + "</option>";
                LastYears += -1;
            }
            var OptionsMonths = "<option selected disabled>-- Select Months--</option>";
            for (int j=1;j<=12;j++)
            {
                var Value = (j > 9) ? j.ToString() : "0"+j;
                OptionsMonths += "<option value='" + Value + "'>" + Value + "</option>";
            }
            ViewBag.MonthsOption = OptionsMonths;
            ViewBag.YearsOption = OptionYears;
            return base.Index();
        }
    }
}
