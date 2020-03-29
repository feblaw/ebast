using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain.DTO.Core;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Identity;
using DataTables.AspNet.AspNetCore;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/travel")]
    [Authorize]
    public class TravelController : BaseApiController<Claim, IClaimService, ClaimDto, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ITicketInfoService _ticket;
        private readonly FileHelper _fileHelper;
        private readonly IHostingEnvironment _env;

        public TravelController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IClaimService service,
            FileHelper fileHelper,
            ITicketInfoService ticket,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _ticket = ticket;
            _fileHelper = fileHelper;
            _env = env;
        }

        [HttpPost]
        [Route("PostDatatables")]
        public override IActionResult PostDataTables(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Claim, object>>[15];
            Includes[0] = pack => pack.Contractor;
            Includes[1] = pack => pack.Agency;
            Includes[2] = pack => pack.Departure;
            Includes[3] = pack => pack.Destination;
            Includes[4] = pack => pack.ClaimApproverOne;
            Includes[5] = pack => pack.ClaimApproverTwo;
            Includes[6] = pack => pack.CostCenter;
            Includes[7] = pack => pack.NetworkNumber;
            Includes[8] = pack => pack.ActivityCode;
            Includes[9] = pack => pack.ContractorProfile;
            Includes[10] = pack => pack.ContractorProfile.Vacancy;
            Includes[11] = pack => pack.ContractorProfile.Vacancy.Departement;
            Includes[12] = pack => pack.ContractorProfile.Vacancy.DepartementSub;
            Includes[13] = pack => pack.Ticket;
            Includes[14] = pack => pack.Vacancy;

            var ProfileId = _userHelper.GetLoginUser(User).Id;
            var Type = ClaimType.TravelClaim;
            var response = Service.GetDataTablesResponse<TravelDto>(request,
               Mapper,$"ClaimType.toString().Equals(\"{Type}\")", Includes);

            #region Filter
            if (User.IsInRole("Contractor"))
            {
                response = Service.GetDataTablesResponse<TravelDto>(request,
                    Mapper,
                    $"ClaimType.toString().Equals(\"{Type}\") && ContractorId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            else if (User.IsInRole("Line Manager"))
            {
                response = Service.GetDataTablesResponse<TravelDto>(request,
                     Mapper,
                    $"ClaimType.toString().Equals(\"{Type}\") && ClaimApproverTwoId.toString().Equals(\"{ProfileId}\") || ClaimApproverOneId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            else if (User.IsInRole("Regional Project Manager"))
            {
                response = Service.GetDataTablesResponse<TravelDto>(request,
                    Mapper,
                    $"ClaimType.toString().Equals(\"{Type}\") && ClaimApproverOneId.toString().Equals(\"{ProfileId}\") || ClaimApproverTwoId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            else if (User.IsInRole("HR Agency"))
            {
                response = Service.GetDataTablesResponse<TravelDto>(request,
                    Mapper,
                    $"ClaimType.toString().Equals(\"{Type}\") && AgencyId.toString().Equals(\"{ProfileId}\")", Includes);
            }
            #endregion

            return new DataTablesJsonResult(response, true);
        }

        [HttpPost]
        [Route("GetFlight")]
        public IActionResult GetFlight(IDataTablesRequest request)
        {
            Includes = new Expression<Func<Claim, object>>[2];
            Includes[0] = pack => pack.Departure;
            Includes[1] = pack => pack.Destination;

            var ProfileId = _userHelper.GetLoginUser(User).Id;
            var Type = ClaimType.TravelClaim;
            var response = Service.GetDataTablesResponse<TravelDto>(request,
               Mapper,
               $" ClaimType.toString().Equals(\"{Type}\") && ContractorId.toString().Equals(\"{ProfileId}\") && StatusOne.toString().Equals(\"{StatusOne.Approved}\") && StatusTwo.toString().Equals(\"{StatusTwo.Approved}\") ", Includes);

            return new DataTablesJsonResult(response, true);
        }

        [Authorize(Roles = "Contractor,Administrator")]
        public override IActionResult Delete(Guid id)
        {
            return base.Delete(id);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Upload")]
        public IActionResult Upload(IFormFile filedata, string id)
        {
            var item = _ticket.GetById(Guid.Parse(id));
            if (item == null)
            {
                return NotFound();
            }

            var _uploadDir = "uploads/travel";
            var upload = Path.Combine(_env.WebRootPath, _uploadDir);
            if (!Directory.Exists(upload))
            {
                Directory.CreateDirectory(upload);
            }

            if (filedata.Length > 0)
            {
                var prefix = Guid.NewGuid().ToString("n") + "_";
                var filepath = Path.Combine(upload, prefix + filedata.FileName);
                var imageSrc = $"{_uploadDir}/{prefix}{filedata.FileName}";
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    filedata.CopyTo(filestream);
                }

                var result = new Attachment()
                {
                    Name = filedata.FileName,
                    FileType = _fileHelper.GetMimeTypeByName(filedata.FileName),
                    Type = Attachment.FILE_TYPE_UPLOAD,
                    Path = imageSrc,
                    Size = filedata.Length / 1024
                };


                if (!string.IsNullOrEmpty(item.Files))
                {
                    var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(item.Files);
                    CurrentFiles.Add(result.Path);
                    item.Files = JsonConvert.SerializeObject(CurrentFiles);
                    _ticket.Update(item);
                }
                else
                {
                    var list = new List<string>();
                    list.Add(result.Path);
                    item.Files = JsonConvert.SerializeObject(list);
                    _ticket.Update(item);
                }
                return Json(new
                {
                    data = result
                });


            }

            return Json(new BadRequestResult());
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        [Route("DeleteFile")]
        public IActionResult DeleteFile(string file, string id)
        {
            var item = _ticket.GetById(Guid.Parse(id));
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
                    _ticket.Update(item);
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
