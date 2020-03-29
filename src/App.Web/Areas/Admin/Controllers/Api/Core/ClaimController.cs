using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using DataTables.AspNet.Core;
using System.Linq.Expressions;
using App.Domain.Models.Enum;
using DataTables.AspNet.AspNetCore;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/claim")]
    public class ClaimController : BaseApiController<Claim, IClaimService, ClaimDto, Guid>
    {
        private readonly IAllowanceListService _allowanceList;
        private readonly IUserHelper _userHelper;
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;
        private readonly IHolidaysService _holiday;

        public ClaimController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IClaimService service,
            IHostingEnvironment env,
            FileHelper fileHelper,
            IAllowanceListService allowanceList,
            IHolidaysService holiday,
            IUserHelper
            userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _allowanceList = allowanceList;
            _userHelper = userHelper;
            _env = env;
            _fileHelper = fileHelper;
            _holiday = holiday;
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Claim, object>>[12];
            Includes[0] = pack => pack.Contractor;
            Includes[1] = pack => pack.Agency;
            Includes[2] = pack => pack.ClaimApproverOne;
            Includes[3] = pack => pack.ClaimApproverTwo;
            Includes[4] = pack => pack.CostCenter;
            Includes[5] = pack => pack.NetworkNumber;
            Includes[6] = pack => pack.ActivityCode;
            Includes[7] = pack => pack.ContractorProfile;
            Includes[8] = pack => pack.ContractorProfile.Vacancy;
            Includes[9] = pack => pack.ContractorProfile.Vacancy.Departement;
            Includes[10] = pack => pack.ContractorProfile.Vacancy.DepartementSub;
            Includes[11] = pack => pack.ClaimCategory;

            var ProfileId = _userHelper.GetLoginUser(User).Id;
            var Type = ClaimType.GeneralClaim;
            var response = Service.GetDataTablesResponse<ClaimDto>(request,
                Mapper,
                $"ClaimType.toString().Equals(\"{Type}\")", Includes);

            if (User.IsInRole("Contractor"))
            {
                response = Service.GetDataTablesResponse<ClaimDto>(request,
                Mapper,
                $"ClaimType.toString().Equals(\"{Type}\") && ContractorId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            else if (User.IsInRole("Line Manager"))
            {
                response = Service.GetDataTablesResponse<ClaimDto>(request,
                Mapper,
                $"ClaimType.toString().Equals(\"{Type}\") && ClaimApproverTwoId.toString().Equals(\"{ProfileId}\") || ClaimApproverOneId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            //else if (User.IsInRole("Project Manager"))
            //{
            //    response = Service.GetDataTablesResponse<ClaimDto>(request,
            //    Mapper,
            //    $"ClaimType.toString().Equals(\"{Type}\") && ClaimApproverOneId.toString().Equals(\"{ProfileId}\") || ClaimApproverTwoId.toString().Equals(\"{ProfileId}\")", Includes);
            //}
            else if (User.IsInRole("HR Agency"))
            {
                //response = Service.GetDataTablesResponse<ClaimDto>(request,
                //Mapper,
                //$"ClaimType.toString().Equals(\"{Type}\") && AgencyId.toString().Equals(\"{ProfileId}\")", Includes);

                response = Service.GetDataTablesResponse<ClaimDto>(request,
                Mapper,
                $"ClaimType.toString().Equals(\"{Type}\") && ContractorId.toString().Equals(\"{ProfileId}\")", Includes);

            }

            return new DataTablesJsonResult(response, true);
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
                    var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(((item.Files)));
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

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item == null) return NotFound();

            return FileController.DoPlUpload(Request, _env.WebRootPath, "uploads/claim",
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
        [Route("GetAllowance")]
        public IActionResult GetAllowance()
        {
            var Srf = _userHelper.GetCurrentSrfByLogin(User);
            var Allowance = _allowanceList.GetAll().Where(x => x.ServicePackId.Equals(Srf.ServicePackId)).FirstOrDefault();
            if (Allowance != null)
            {
                return Ok(Allowance);
            }
            else
            {
                Dictionary<string, int> data = new Dictionary<string, int>();
                data.Add("onCallNormal", 0);
                data.Add("onCallHoliday", 0);
                data.Add("shiftNormal", 0);
                data.Add("shiftHoliday", 0);
                return Ok(data);
            }
        }

        [HttpGet]
        [Route("CheckHolidays")]
        public IActionResult CheckHolidays(string first, string last)
        {
            var Check = _holiday.GetAll().Where(x => x.DateDay.Date.ToString("yyyy-MM-dd").Equals(first) || x.DateDay.Date.ToString("yyyy-MM-dd").Equals(last)).FirstOrDefault();
            var TotalDays = (DateTime.Parse(last) - DateTime.Parse(first)).TotalDays <= 0 ? 1 : (DateTime.Parse(last) - DateTime.Parse(first)).TotalDays + 1;
            if (Check != null)
            {
                return Json(new { message = Check.DayType, total = TotalDays });
            }
            return Json(new { message = false, total = TotalDays });
        }

    }
}
