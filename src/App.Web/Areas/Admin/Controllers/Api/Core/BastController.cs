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
using System.Linq.Dynamic.Core;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/Bast")]
    //[Authorize]
    public class BastController : BaseApiController<Bast, IBastService, BastDto, Guid>
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
        private readonly IMapAsgBastService _mapAsgBast;
        //private readonly INetworkNumberService _network;

        public BastController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            FileHelper fileHelper,
            //ICostCenterService costCenter,
            //IServicePackService servicePack,
            //IServicePackCategoryService servicePackCategory,
            IBastService service,
            IMapAsgBastService mapAsgBast,
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
            _mapAsgBast = mapAsgBast;
            //_network = network;
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Bast, object>>[5];
            Includes[0] = pack => pack.ApprovalFour;
            Includes[1] = pack => pack.ApprovalOne;
            Includes[2] = pack => pack.ApprovalThree;
            Includes[3] = pack => pack.ApprovalTwo;
            Includes[4] = pack => pack.Asp;
            //Includes[5] = pack => pack.DepartementSub;
            //Includes[6] = pack => pack.Vendor;
            //Includes[7] = pack => pack.ApproverTwo;
            //Includes[8] = pack => pack.ApproverThree;


            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            var response = Service.GetDataTablesResponse<BastDto>(request, Mapper, null, Includes);
            var profileId = _userHelper.GetUser(User).UserProfile.Id;

            if (User.IsInRole("ASP Admin"))
            {
                response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"AspId.toString().Equals(\"{ASP.ToString()}\")", Includes);
            }

            if (User.IsInRole("ASP PM"))
            {
                response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"ApprovalOneID.toString().Equals(\"{profileId}\")", Includes);
            }

            if (User.IsInRole("PA"))
            {
                response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"ApprovalTwoID.toString().Equals(\"{profileId}\")", Includes);
            }

            if (User.IsInRole("IM"))
            {
                response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"ApprovalThreeID.toString().Equals(\"{profileId}\")", Includes);
            }

            if (User.IsInRole("CPM"))
            {
                response = Service.GetDataTablesResponse<BastDto>(request, Mapper, $"ApprovalFourID.toString().Equals(\"{profileId}\")", Includes);
            }

            return new DataTablesJsonResult(response, true);
        }

        [Route("PostPending")]
        [HttpPost]
        public IActionResult PostPending(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Bast, object>>[5];
            Includes[0] = pack => pack.ApprovalFour;
            Includes[1] = pack => pack.ApprovalOne;
            Includes[2] = pack => pack.ApprovalThree;
            Includes[3] = pack => pack.ApprovalTwo;
            Includes[4] = pack => pack.Asp;
            //Includes[5] = pack => pack.DepartementSub;
            //Includes[6] = pack => pack.Vendor;
            //Includes[7] = pack => pack.ApproverTwo;
            //Includes[8] = pack => pack.ApproverThree;


            var ASP = _userHelper.GetUser(User).UserProfile.ASPId;
            //var response = Service.GetDataTablesResponse<BastDto>(request, Mapper, null, Includes);
            var profileId = _userHelper.GetUser(User).UserProfile.Id;
            var data = Service.GetAllQ(Includes);
            //.Where(x => x.IsLocked == false && x.IsActive == false);
            DataTablesResponse response;

            if (User.IsInRole("ASP PM"))
            {
                data = data.Where(x => (x.ApprovalOneStatus == BastApproveStatus.Waiting)
                    && x.ApprovalOneID == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<BastDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("PA"))
            {
                data = data.Where(x => (x.ApprovalTwoStatus == BastApproveStatus.Waiting && x.ApprovalOneStatus == BastApproveStatus.Approved)
                    && x.ApprovalTwoID == profileId);
                if (data.Any())
                {
                    //response = Service.GetDataTablesResponseByQuery<BastDto>(request, Mapper, data);
                    var filteredData = data.AsQueryable();

                    var searchQuery = request.Search.Value;

                    var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

                    if (!string.IsNullOrEmpty(globalFilter))
                        filteredData = data.Where(globalFilter);

                    var filter = request.Columns.GetFilter();

                    if (!string.IsNullOrEmpty(filter))
                        filteredData = filteredData.Where(filter);

                    var sort = "approvalOneDate asc";


                    if (!string.IsNullOrEmpty(sort))
                        filteredData = filteredData.OrderBy(sort);

                    var dataPage = filteredData.Skip(request.Start).Take(request.Length);

                    response = DataTablesResponse.Create(request,
                        data.Count(),
                        filteredData.Count(),
                        Mapper.Map<List<BastDto>>(dataPage));

                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("IM"))
            {
                data = data.Where(x => (x.ApprovalThreeStatus == BastApproveStatus.Waiting && x.ApprovalTwoStatus == BastApproveStatus.Approved)
                    && x.ApprovalThreeID == profileId);
                if (data.Any())
                {
                    //response = Service.GetDataTablesResponseByQuery<BastDto>(request, Mapper, data);

                    var filteredData = data.AsQueryable();

                    var searchQuery = request.Search.Value;

                    var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

                    if (!string.IsNullOrEmpty(globalFilter))
                        filteredData = data.Where(globalFilter);

                    var filter = request.Columns.GetFilter();

                    if (!string.IsNullOrEmpty(filter))
                        filteredData = filteredData.Where(filter);

                    var sort = "approvalTwoDate asc";


                    if (!string.IsNullOrEmpty(sort))
                        filteredData = filteredData.OrderBy(sort);

                    var dataPage = filteredData.Skip(request.Start).Take(request.Length);

                    response = DataTablesResponse.Create(request,
                        data.Count(),
                        filteredData.Count(),
                        Mapper.Map<List<BastDto>>(dataPage));

                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("CPM"))
            {
                data = data.Where(x => (x.ApprovalFourStatus == BastApproveStatus.Waiting && x.ApprovalThreeStatus == BastApproveStatus.Approved)
                    && x.ApprovalFourID == profileId);
                if (data.Any())
                {
                    //response = Service.GetDataTablesResponseByQuery<BastDto>(request, Mapper, data);
                    var filteredData = data.AsQueryable();

                    var searchQuery = request.Search.Value;

                    var globalFilter = request.Columns.GetFilter(searchQuery, "OR");

                    if (!string.IsNullOrEmpty(globalFilter))
                        filteredData = data.Where(globalFilter);

                    var filter = request.Columns.GetFilter();

                    if (!string.IsNullOrEmpty(filter))
                        filteredData = filteredData.Where(filter);

                    var sort = "approvalThreeDate asc";


                    if (!string.IsNullOrEmpty(sort))
                        filteredData = filteredData.OrderBy(sort);

                    var dataPage = filteredData.Skip(request.Start).Take(request.Length);

                    response = DataTablesResponse.Create(request,
                        data.Count(),
                        filteredData.Count(),
                        Mapper.Map<List<BastDto>>(dataPage));

                    return new DataTablesJsonResult(response, true);
                }
            }


            response = DataTablesResponse.Create(request, 0, 0, new BastDto());
            return new DataTablesJsonResult(response, true);
        }

        public override IActionResult Delete(Guid id)
        {
            var Data = Service.GetById(id);
            if (!string.IsNullOrEmpty(Data.Files))
            {
                var FileData = JsonConvert.DeserializeObject<List<string>>(Data.Files);
                if (FileData != null)
                {
                    foreach (var row in FileData)
                    {
                        var uploads = System.IO.Path.Combine(_env.WebRootPath, row);
                        System.IO.File.Delete(uploads);
                    }
                }

            }

            var mapAsgBast2 = _mapAsgBast.GetAll().Where(x => x.IdBast.Equals(id)).ToList();
            if (mapAsgBast2 != null)
            {
                //_mapAsgBast.Delete(mapAsgBast2);
                foreach (var row in mapAsgBast2)
                {
                    var Temp = _mapAsgBast.GetById(row.Id);
                    _mapAsgBast.Delete(Temp);
                }
            }
            //_contractor.Delete(Contractor);
            //_profileService.Delete(UserProfile);
            //var Deleted = _userManager.DeleteAsync(AppUser).Result;
            //if (Deleted.Succeeded)
            //{
            //    return true;
            //}


            return base.Delete(id);
        }

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item == null) return NotFound();

            return FileController.DoPlUpload(Request, _env.WebRootPath, "uploads/bast",
                (result) => {
                    if (!string.IsNullOrEmpty(item.Files))
                    {
                        var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(item.Files);
                        CurrentFiles.Add(result.Path);
                        item.Files = JsonConvert.SerializeObject(CurrentFiles);
                        Service.Update(item);
                    }
                    else
                    {
                        var list = new List<string>();
                        list.Add(result.Path);
                        item.Files = JsonConvert.SerializeObject(list);
                        Service.Update(item);
                    }
                    return true;
                }
            );
        }

        [HttpGet]
        [HttpPost]
        [Route("DeleteFile")]
        public IActionResult DeleteFile(string file, string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                if (!string.IsNullOrEmpty(item.Files))
                {
                    var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(item.Files);
                    CurrentFiles.Remove(file);
                    item.Files = JsonConvert.SerializeObject(CurrentFiles);
                    Service.Update(item);
                    var Deleted = System.IO.Path.Combine(_env.WebRootPath, file);
                    System.IO.File.Delete(Deleted);
                }
            }
            return Json(new
            {
                message = file
            });
        }


    }
}
