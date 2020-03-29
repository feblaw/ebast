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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace App.Web.Areas.Admin.Controllers.Api.Core
{

    [Area("Admin")]
    [Produces("application/json")]
    [Route("admin/api/escalation")]
    public class EscalationController : BaseApiController<SrfEscalationRequest, ISrfEscalationRequestService, EscalationDto, Guid>
    {
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;

        public EscalationController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IHostingEnvironment env,
            IMapper mapper,
            FileHelper fileHelper,
            ISrfEscalationRequestService service, 
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
         
            Includes = new Expression<Func<SrfEscalationRequest, object>>[2];
            Includes[0] = pack => pack.ServicePack;
            Includes[1] = pack => pack.SrfRequest;
            _env = env;
            _fileHelper = fileHelper;
        }

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload(string id)
        {
            var item = Service.GetById(Guid.Parse(id));
            if (item == null) return NotFound();

            return FileController.DoPlUpload(Request, _env.WebRootPath, "uploads/escalation",
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
            return base.Delete(id);
        }
    }
}
