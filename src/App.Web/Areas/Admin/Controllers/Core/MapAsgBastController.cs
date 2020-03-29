using App.Domain.Models.Core;
using App.Services.Core.Interfaces;
using App.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Helper;
using App.Services.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using App.Web.Models.ViewModels.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
using FileIO = System.IO.File;
using App.Domain.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using App.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using App.Web.Models.ViewModels.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System.IO;
using System.Linq.Expressions;
using System.Globalization;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, ASP Admin, IM, PA, CPM, ASP PM")]
    public class MapAsgBastController : BaseController<MapAsgBast, IMapAsgBastService, MapAsgBastViewModel, MapAsgBastFormModel, Guid>
    {
        
        private readonly IUserProfileService _user;
        private readonly NotifHelper _notif;
        private readonly FileHelper _file;
        private readonly IUserHelper _userHelper;
        private readonly IUserService _userService;
        private readonly IUserProfileService _profileUser;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HostConfiguration _hostConfiguration;
        private readonly IMapAsgBastService _service;
        private readonly ExcelHelper _excel;
        private readonly IASPService _asp;
        private readonly IMapAsgBastService _mappingAsgBast;
        private readonly IAssignmentService _asg;
        private readonly IConverter _converter;
        //private readonly IMapAsgBastService _MapAsgBast;


        public MapAsgBastController(IHttpContextAccessor httpContextAccessor, 
            IUserService userService, IMapper mapper, 
            IMapAsgBastService service,
            IUserProfileService profileUser,
            IUserProfileService user,
            IOptions<HostConfiguration> hostConfiguration,
            UserManager<ApplicationUser> userManager,
            NotifHelper notif,
            FileHelper file,
            IHostingEnvironment env,
            ExcelHelper excel,
            IASPService asp,
            IConverter converter,
            IMapAsgBastService mappingAsgBast,
            IAssignmentService asg,
            //IMapAsgBastService MapAsgBast,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _user = user;
            _notif = notif;
            _userService = userService;
            _profileUser = profileUser;
            _file = file;
            _userHelper = userHelper;
            _env = env;
            _userManager = userManager;
            _hostConfiguration = hostConfiguration.Value;
            _service = service;
            _excel = excel;
            //_MapAsgBast = MapAsgBast;
            _mappingAsgBast = mappingAsgBast;
            _converter = converter;
            _asg = asg;
            this._asp = asp;
        }

        private string GetCurentUser()
        {
            var AppUser = _userHelper.GetUser(User);
            var UserProfile = _profileUser.GetByUserId(AppUser.Id);
            return UserProfile.Name;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,ASP Admin, IM, PA, CPM, ASP PM")]
        public override IActionResult Index()
        {
            List<string> UserRole = _userManager.GetRolesAsync(_userHelper.GetUser(User)).Result.ToList();
            ViewBag.UserRole = UserRole.FirstOrDefault();
            return base.Index();
        }

        
    }
}
