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
    [Route("admin/api/vacancy")]
    //[Authorize]
    public class VacancyController : BaseApiController<VacancyList, IVacancyListService, VacancyListDto, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;
        private readonly ICostCenterService _costCenter;
        private readonly IServicePackService _servicePack;
        private readonly IServicePackCategoryService _servicePackCategory;
        private readonly IDepartementService _department;
        private readonly IDepartementSubService _departmentSub;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly INetworkNumberService _network;
        private readonly IMapAsgBastService _mapAsgBast;

        public VacancyController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            FileHelper fileHelper,
            ICostCenterService costCenter,
            IServicePackService servicePack,
            IServicePackCategoryService servicePackCategory,
            IVacancyListService service,
            UserManager<ApplicationUser> userManager,
            IDepartementService department,
            INetworkNumberService network,
            IDepartementSubService departmentSub,
            IMapAsgBastService mapAsgBast,
        IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {

            _env = env;
            _fileHelper = fileHelper;
            _costCenter = costCenter;
            _servicePack = servicePack;
            _servicePackCategory = servicePackCategory;
            _department = department;
            _departmentSub = departmentSub;
            _userManager = userManager;
            _userHelper = userHelper;
            _network = network;
            _mapAsgBast = mapAsgBast;
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<VacancyList, object>>[9];
            Includes[0] = pack => pack.ServicePack;
            Includes[1] = pack => pack.ServicePackCategory;
            Includes[2] = pack => pack.ApproverOne;
            Includes[3] = pack => pack.Candidate;
            Includes[4] = pack => pack.Departement;
            Includes[5] = pack => pack.DepartementSub;
            Includes[6] = pack => pack.Vendor;
            Includes[7] = pack => pack.ApproverTwo;
            Includes[8] = pack => pack.ApproverThree;


            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            var response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"EndDate.AddMonths(1) >= (\"{DateTime.Now}\")", Includes);

            if (User.IsInRole("Line Manager"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverOneId.toString().Equals(\"{PreofileId}\")", Includes);
            }

            else if (User.IsInRole("Head Of Service Line"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverTwoId.toString().Equals(\"{PreofileId}\")", Includes);
            }
            else if (User.IsInRole("Head Of Operation"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverThreeId.toString().Equals(\"{PreofileId}\")", Includes);
            }
            else if (User.IsInRole("HR Agency"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"VendorId.toString().Equals(\"{PreofileId}\") && StatusThree.toString().Equals(\"{SrfApproveStatus.Approved}\")", Includes);
            }

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("ExpiredWP")]
        public IActionResult ExpiredWP(IDataTablesRequest request)
        {
            Includes = new Expression<Func<VacancyList, object>>[9];
            Includes[0] = pack => pack.ServicePack;
            Includes[1] = pack => pack.ServicePackCategory;
            Includes[2] = pack => pack.ApproverOne;
            Includes[3] = pack => pack.Candidate;
            Includes[4] = pack => pack.Departement;
            Includes[5] = pack => pack.DepartementSub;
            Includes[6] = pack => pack.Vendor;
            Includes[7] = pack => pack.ApproverTwo;
            Includes[8] = pack => pack.ApproverThree;

            //var dateReminder = DateTime.Today.AddMonths(1);
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
            var response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"EndDate < (\"{DateTime.Now}\") && Status != (\"{SrfStatus.Terminate}\")", Includes);

            if (User.IsInRole("Line Manager"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverOneId.toString().Equals(\"{PreofileId}\") && EndDate < (\"{DateTime.Now}\") && Status != (\"{SrfStatus.Terminate}\")", Includes);
            }

            else if (User.IsInRole("Head Of Service Line"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverTwoId.toString().Equals(\"{PreofileId}\")&& EndDate < (\"{DateTime.Now}\")&& Status != (\"{SrfStatus.Terminate}\")", Includes);
            }
            else if (User.IsInRole("Head Of Operation"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"ApproverThreeId.toString().Equals(\"{PreofileId}\")&& EndDate < (\"{DateTime.Now}\")&& Status != (\"{SrfStatus.Terminate}\")", Includes);
            }
            else if (User.IsInRole("HR Agency"))
            {
                response = Service.GetDataTablesResponse<VacancyListDto>(request, Mapper, $"VendorId.toString().Equals(\"{PreofileId}\")&& EndDate < (\"{DateTime.Now}\")&& Status != (\"{SrfStatus.Terminate}\")", Includes);
            }

            return new DataTablesJsonResult(response, true);
        }

        [Route("PostPending")]
        [HttpPost]
        public IActionResult PostPending(IDataTablesRequest request)
        {
            Includes = new Expression<Func<VacancyList, object>>[10];
            Includes[0] = pack => pack.ServicePack;
            Includes[1] = pack => pack.ServicePackCategory;
            Includes[2] = pack => pack.ApproverOne;
            Includes[3] = pack => pack.Candidate;
            Includes[4] = pack => pack.Departement;
            Includes[5] = pack => pack.DepartementSub;
            Includes[6] = pack => pack.Vendor;
            Includes[7] = pack => pack.ApproverTwo;
            Includes[8] = pack => pack.ApproverThree;
            Includes[9] = pack => pack.Rpm;

            var profileId = _userHelper.GetUser(User).UserProfile.Id;

            var data = Service.GetAllQ(Includes);
            //.Where(x => x.IsLocked == false && x.IsActive == false);
            DataTablesResponse response;


            if (User.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Waiting && x.ApproverTwoId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Waiting && x.ApproverThreeId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }
            if (User.IsInRole("Regional Project Manager"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved
                    && x.RpmId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Administrator") || User.IsInRole("Line Manager"))
            {
                data = data.Where(x => x.StatusTwo == SrfApproveStatus.Waiting
                    && x.StatusThree == SrfApproveStatus.Waiting);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("HR Agency"))
            {
                data = data.Where(x => x.VendorId == profileId);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            response = DataTablesResponse.Create(request, 0, 0, new VacancyListDto());
            return new DataTablesJsonResult(response, true);
        }

        [Route("BASTView")]
        [HttpPost]
        public IActionResult BASTView(IDataTablesRequest request)
        {
            Includes = new Expression<Func<VacancyList, object>>[10];
            Includes[0] = pack => pack.ServicePack;
            Includes[1] = pack => pack.ServicePackCategory;
            Includes[2] = pack => pack.ApproverOne;
            Includes[3] = pack => pack.Candidate;
            Includes[4] = pack => pack.Departement;
            Includes[5] = pack => pack.DepartementSub;
            Includes[6] = pack => pack.Vendor;
            Includes[7] = pack => pack.ApproverTwo;
            Includes[8] = pack => pack.ApproverThree;
            Includes[9] = pack => pack.Rpm;

            var profileId = _userHelper.GetUser(User).UserProfile.Id;

            var data = Service.GetAllQ(Includes);
            //.Where(x => x.IsLocked == false && x.IsActive == false);
            DataTablesResponse response;

            if (User.IsInRole("Regional Project Manager"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved
                    && x.RpmId == profileId && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Line Manager"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved
                    && x.ApproverOneId == profileId && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Service Line"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved
                    && x.ApproverTwoId == profileId && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Head Of Operation"))
            {
                data = data.Where(x => (x.StatusOne == SrfApproveStatus.Approved)
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved
                    && x.ApproverThreeId == profileId && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("HR Agency"))
            {
                data = data.Where(x => x.VendorId == profileId && x.StatusOne == SrfApproveStatus.Approved
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            if (User.IsInRole("Administrator"))
            {
                data = data.Where(x => x.StatusOne == SrfApproveStatus.Approved
                    && x.StatusTwo == SrfApproveStatus.Approved
                    && x.StatusThree == SrfApproveStatus.Approved && x.Status != SrfStatus.Terminate && x.EndDate.AddMonths(1) > DateTime.Now);
                if (data.Any())
                {
                    response = Service.GetDataTablesResponseByQuery<VacancyListDto>(request, Mapper, data);
                    return new DataTablesJsonResult(response, true);
                }
            }

            response = DataTablesResponse.Create(request, 0, 0, new VacancyListDto());
            return new DataTablesJsonResult(response, true);
        }

        

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item == null) return NotFound();

            return FileController.DoPlUpload(Request, _env.WebRootPath, "uploads/wpKTP",
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

        //public override IActionResult Delete(Guid id)
        //{
        //    var Data = Service.GetById(id);
        //    if (!string.IsNullOrEmpty(Data.Files))
        //    {
        //        var FileData = JsonConvert.DeserializeObject<List<string>>(Data.Files);
        //        if (FileData != null)
        //        {
        //            foreach (var row in FileData)
        //            {
        //                var uploads = System.IO.Path.Combine(_env.WebRootPath, row);
        //                System.IO.File.Delete(uploads);
        //            }
        //        }

        //    }
        //    return base.Delete(id);
        //}


    }
}
