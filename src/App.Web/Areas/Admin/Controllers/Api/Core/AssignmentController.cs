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
using System.Linq.Expressions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Identity;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using App.Services.Utils;


namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/assignment")]
    //[Authorize]
    public class AssignmentController : BaseApiController<Assignment, IAssignmentService, AssignmentDto, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;
        //private readonly ICostCenterService _costCenter;
        //private readonly IServicePackService _servicePack;
        //private readonly IServicePackCategoryService _servicePackCategory;
        //private readonly IDepartementService _department;
        //private readonly IDepartementSubService _departmentSub;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        //private readonly INetworkNumberService _network;

        public AssignmentController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            FileHelper fileHelper,
            //ICostCenterService costCenter,
            //IServicePackService servicePack,
            //IServicePackCategoryService servicePackCategory,
            IAssignmentService service,
            UserManager<ApplicationUser> userManager,
            //IDepartementService department,
            //INetworkNumberService network,
            //IDepartementSubService departmentSub,
            IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {

            _env = env;
            _fileHelper = fileHelper;
            //_costCenter = costCenter;
            //_servicePack = servicePack;
            //_servicePackCategory = servicePackCategory;
            //_department = department;
            //_departmentSub = departmentSub;
            _userManager = userManager;
            _userHelper = userHelper;
            //_network = network;
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Assignment, object>>[1];
            Includes[0] = pack => pack.Asp;
            //Includes[1] = pack => pack.Bast2;
            //Includes[2] = pack => pack.Bast3;
            //Includes[3] = pack => pack.Candidate;
            //Includes[4] = pack => pack.Departement;
            //Includes[5] = pack => pack.DepartementSub;
            //Includes[6] = pack => pack.Vendor;
            //Includes[7] = pack => pack.ApproverTwo;
            //Includes[8] = pack => pack.ApproverThree;


            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            var response = Service.GetDataTablesResponse<AssignmentDto>(request, Mapper, $"AssignmentCancel == (\"{false}\")", Includes);

            if (User.IsInRole("ASP Admin"))
            {
                response = Service.GetDataTablesResponse<AssignmentDto>(request, Mapper, $"AspId.toString().Equals(\"{ASP.ToString()}\") && AssignmentCancel == (\"{false}\")", Includes);
            }

            //else if (User.IsInRole("Head Of Service Line"))
            //{
            //    response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverTwoId.toString().Equals(\"{PreofileId}\")", Includes);
            //}
            //else if (User.IsInRole("Head Of Operation"))
            //{
            //    response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverThreeId.toString().Equals(\"{PreofileId}\")", Includes);
            //}
            //else if (User.IsInRole("HR Agency"))
            //{
            //    response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"VendorId.toString().Equals(\"{PreofileId}\") && StatusThree.toString().Equals(\"{SrfApproveStatus.Approved}\")", Includes);
            //}

            return new DataTablesJsonResult(response, true);
        }

        


    }
}
