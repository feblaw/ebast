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
    [Route("admin/api/mapasgbast")]
    //[Authorize]
    public class MapAsgBastController : BaseApiController<MapAsgBast, IMapAsgBastService, MapAsgBastDto, Guid>
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

        public MapAsgBastController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            FileHelper fileHelper,
            //ICostCenterService costCenter,
            //IServicePackService servicePack,
            //IServicePackCategoryService servicePackCategory,
            IMapAsgBastService service,
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
        [Route("PostDatatables/{id}")]
        public IActionResult PostDataTables(Guid id, IDataTablesRequest request)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;

            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();

            Includes = new Expression<Func<MapAsgBast, object>>[2];
            Includes[0] = pack => pack.Assignment;
            Includes[1] = pack => pack.Bast;

            var response = Service.GetDataTablesResponse<MapAsgBastDto>(request,
               Mapper,
               $"IdBAst.toString().Equals(\"{id}\")", Includes);

            //if (UserRole.Contains("Administrator"))
            //{
            //    response = Service.GetDataTablesResponse<CandidateDto>(request,
            //    Mapper,
            //    $"VacancyId.toString().Equals(\"{id}\")", Includes);
            //}


            return new DataTablesJsonResult(response, true);
        }


        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<MapAsgBast, object>>[7];
            Includes[0] = pack => pack.Assignment;
            Includes[1] = pack => pack.Bast;
            Includes[2] = pack => pack.Assignment.Asp;
            Includes[3] = pack => pack.Bast.ApprovalOne;
            Includes[4] = pack => pack.Bast.ApprovalTwo;
            Includes[5] = pack => pack.Bast.ApprovalThree;
            Includes[6] = pack => pack.Bast.ApprovalFour;
            //Includes[4] = pack => pack.Departement;
            //Includes[5] = pack => pack.DepartementSub;
            //Includes[6] = pack => pack.Vendor;
            //Includes[7] = pack => pack.ApproverTwo;
            //Includes[8] = pack => pack.ApproverThree;

            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            var response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, null, Includes);

            if(User.IsInRole("ASP Admin") || (User.IsInRole("ASP PM")))
            {
                response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"Assignment.AspId.toString().Equals(\"{ASP.ToString()}\")", Includes);
            }
            else if (User.IsInRole("PA"))
            {
                response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"Bast.ApprovalTwoID.toString().Equals(\"{PreofileId}\")", Includes);
            }

            else if (User.IsInRole("IM"))
            {
                response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"Bast.ApprovalThreeID.toString().Equals(\"{PreofileId}\")", Includes);
            }

            else if (User.IsInRole("CPM"))
            {
                response = Service.GetDataTablesResponse<MapAsgBastDto>(request, Mapper, $"Bast.ApprovalFourID.toString().Equals(\"{PreofileId}\")", Includes);
            }

            return new DataTablesJsonResult(response, true);
        }
        

    }
}
