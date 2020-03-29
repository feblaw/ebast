using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Domain.DTO.Core;
using App.Web.Controllers;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using App.Domain.Models.Identity;
using System.IO;
using DataTables.AspNet.Core;
using DataTables.AspNet.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{
    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/candidate")]
    public class CandidateController : BaseApiController<CandidateInfo, ICandidateInfoService, CandidateDto, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;
        private readonly IUserProfileService _userProfile;
        private readonly IUserService _AppUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ISrfRequestService _srf;

        public CandidateController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IMapper mapper,
            IHostingEnvironment env,
            FileHelper fileHelper,
            ICandidateInfoService service,
            IUserProfileService userProfile,
            UserManager<ApplicationUser> userManager,
            ISrfRequestService srf,
            IUserService AppUser,
            IUserHelper userHelper) : base(httpContextAccessor, userService, mapper, service, userHelper)
        {

            _env = env;
            _fileHelper = fileHelper;
            _userProfile = userProfile;
            _AppUser = AppUser;
            _userManager = userManager;
            _userHelper = userHelper;
            _srf = srf;
        }

        [HttpPost]
        [Route("PostDatatables/{id}")]
        public IActionResult PostDataTables(Guid id, IDataTablesRequest request)
        {
            var PreofileId = _userHelper.GetUser(User).UserProfile.Id;

            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();

            Includes = new Expression<Func<CandidateInfo, object>>[3];
            Includes[0] = pack => pack.Account;
            Includes[1] = pack => pack.Vacancy;
            Includes[2] = pack => pack.RequestBy;

            var response = Service.GetDataTablesResponse<CandidateDto>(request,
               Mapper,
               $"VacancyId.toString().Equals(\"{id}\") && (Vacancy.ApproverOneId.toString().Equals(\"{PreofileId}\") || Vacancy.ApproverTwoId.toString().Equals(\"{PreofileId}\") || AgencyId.toString().Equals(\"{PreofileId}\") )", Includes);

            if (UserRole.Contains("Administrator"))
            {
                response = Service.GetDataTablesResponse<CandidateDto>(request,
                Mapper,
                $"VacancyId.toString().Equals(\"{id}\")", Includes);
            }


            return new DataTablesJsonResult(response, true);
        }

        [HttpGet]
        [HttpPost]
        [Route("DeleteFile")]
        public IActionResult DeleteFile(string file, string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            var Srf = _srf.GetAll().Where(x => x.CandidateId.Equals(Guid.Parse(id))).ToList();
            if (item == null || Srf?.Any() == true)
            {
                return NotFound();
            }
            else if (!string.IsNullOrEmpty(item.Attachments))
            {
                var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(((item.Attachments)));
                CurrentFiles.Remove(file);
                item.Attachments = JsonConvert.SerializeObject(CurrentFiles);
                Service.Update(item);
                var Deleted = System.IO.Path.Combine(_env.WebRootPath, file);
                System.IO.File.Delete(Deleted);
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

            return FileController.DoPlUpload(Request, _env.WebRootPath, "uploads/candidate",
                (result) => {
                    if (!string.IsNullOrEmpty(item.Attachments))
                    {
                        var CurrentFiles = JsonConvert.DeserializeObject<List<string>>(item.Attachments);
                        CurrentFiles.Add(result.Path);
                        item.Attachments = JsonConvert.SerializeObject(CurrentFiles);
                        Service.Update(item);
                    }
                    else
                    {
                        var list = new List<string>();
                        list.Add(result.Path);
                        item.Attachments = JsonConvert.SerializeObject(list);
                        Service.Update(item);
                    }
                    return true;
                }
            );
        }

        public override IActionResult Delete(Guid id)
        {
            var Data = Service.GetById(id);

            if (!string.IsNullOrEmpty(Data.Attachments))
            {
                var FileData = JsonConvert.DeserializeObject<List<string>>(Data.Attachments);
                if (FileData != null)
                {
                    foreach (var row in FileData)
                    {
                        var uploads = System.IO.Path.Combine(_env.WebRootPath, row);
                        System.IO.File.Delete(uploads);
                    }
                }
            }
            return base.Delete(id);
        }

        // API VALIDATION

        [Route("EmailValidation")]
        [HttpGet]
        public async Task<IActionResult> EmailValidation(string email = null, string id = null, string candidate = null)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var CheckEmailInSystem = await _userManager.FindByEmailAsync(email);
                if (CheckEmailInSystem == null)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var CheckEmailCandidate = Service.GetAll().Where(x => x.Email.Equals(email) && x.VacancyId.Equals(Guid.Parse(id))).FirstOrDefault();
                        if (!string.IsNullOrEmpty(candidate))
                        {
                            CheckEmailCandidate = Service.GetAll().Where(x => x.Email.Equals(email) && x.Id != Guid.Parse(candidate)).FirstOrDefault();
                        }
                        if (CheckEmailCandidate == null)
                        {
                            return Json(new { message = true });
                        }
                    }
                }
                else
                {
                    var AppUser = _userManager.FindByEmailAsync(email).Result;
                    var UserProfile = _userProfile.GetByUserId(AppUser.Id);
                    if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                    {
                        return Json(new { message = false });
                    }

                    if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                    {
                        return Json(new { message = true });
                    }

                   
                }
            }
            return Json(new { message = false });
        }

        [Route("IdNumberValidation")]
        [HttpGet]
        public IActionResult IdNumberValidation(string number = null, string id = null, string candidate = null)
        {
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(number))
            {
                if (!string.IsNullOrWhiteSpace(candidate)) // Mode Update
                {
                    var Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(number) && x.IsUser == true && x.Id != Guid.Parse(candidate)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    if (Candidate != null)
                    {
                        // Candidate is User
                        var UserProfile = _userProfile.GetById(Candidate.AccountId);
                        var CandidateSaved = Service.GetAll().Where(x => x.AccountId.Equals(UserProfile.Id)).FirstOrDefault();
                        var Srf = _srf.GetAll().Where(x => x.CandidateId.Equals(CandidateSaved.Id)).FirstOrDefault();

                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            return Json(new { message = false, notif = "Resource are blacklist with srf number " + Srf.Number });
                        }
                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            return Json(new { message = true, notif = "Notif resource already with srf number " + Srf.Number });
                        }

                    }
                    else
                    {
                        Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(number) && x.VacancyId.Equals(Guid.Parse(id)) && x.Id != Guid.Parse(candidate)).FirstOrDefault();
                        if (Candidate != null)
                        {
                            return Json(new { message = false, notif = "Resource already with Id Number " });
                        }
                        else
                        {
                            return Json(new { message = true });
                        }
                    }
                }
                else // Mode Insert
                {
                    var Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(number) && x.IsUser == true).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                    if (Candidate != null)
                    {
                        // Candidate is User
                        var UserProfile = _userProfile.GetById(Candidate.AccountId);
                        var CandidateSaved = Service.GetAll().Where(x => x.AccountId.Equals(UserProfile.Id)).FirstOrDefault();
                        var Srf = _srf.GetAll().Where(x => x.CandidateId.Equals(CandidateSaved.Id)).FirstOrDefault();

                        if (UserProfile.IsBlacklist == true && UserProfile.IsTerminate == false)
                        {
                            return Json(new { message = false, notif = "Resource already with srf number " + Srf.Number });
                        }

                        if (UserProfile.IsBlacklist == false && UserProfile.IsTerminate == true)
                        {
                            return Json(new { message = true, notif = "Notif resource already with srf number " + Srf.Number });
                        }

                    }
                    else
                    {
                        Candidate = Service.GetAll().Where(x => !string.IsNullOrWhiteSpace(x.IdNumber) && x.IdNumber.Equals(number) && x.VacancyId.Equals(Guid.Parse(id))).FirstOrDefault();
                        if (Candidate != null)
                        {
                            return Json(new { message = false, notif = "Resource already with Id Number " });
                        }
                        else
                        {
                            return Json(new { message = true });
                        }
                    }
                }

            }
            return Json(new { message = true });
        }



    }
}