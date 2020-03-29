using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Web.Controllers;
using App.Web.Models.ViewModels.Core.Business;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Web.Areas.Admin.Controllers.Core
{
    [Area("Admin")]
    [Authorize]
    public class ContractorDataEscalationController : BaseController<SrfRequest, ISrfRequestService, SrfRequestViewModel, SrfRequestModelForm, Guid>
    {
        private readonly ICandidateInfoService _candidate;
        private readonly IVacancyListService _vacancy;
        private readonly IServicePackService _ssow;
        private readonly IServicePackCategoryService _ssowCategory;
        private readonly IPackageTypeService _packageType;
        private readonly IJobStageService _jobStage;
        private readonly IDepartementSubService _departmentSub;
        private readonly IUserProfileService _profileService;
        private readonly INetworkNumberService _network;
        private readonly IAccountNameService _account;
        private readonly IDepartementService _department;
        private readonly IUserHelper _userHelper;
        private readonly ICostCenterService _costCenter;
        private readonly ISrfEscalationRequestService _escalation;
        private readonly FileHelper _fileHelper;

        public ContractorDataEscalationController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            IMapper mapper, 
            ISrfRequestService service,
            IVacancyListService vacancy,
            ICandidateInfoService candidate,
            IDepartementService department,
            IServicePackService ssow,
            IPackageTypeService packageType,
            ICostCenterService costCenter,
            IDepartementSubService departmentSub,
            IServicePackCategoryService ssowCategory,
            IJobStageService jobsTage,
            IUserProfileService profileService,
            INetworkNumberService network,
            IAccountNameService account,
            ISrfEscalationRequestService escalation,
            FileHelper fileHelper,
            IUserHelper userHelper) : 
            base(httpContextAccessor, userService, mapper, service, userHelper)
        {
            _vacancy = vacancy;
            _candidate = candidate;
            _department = department;
            _ssow = ssow;
            _ssowCategory = ssowCategory;
            _packageType = packageType;
            _jobStage = jobsTage;
            _departmentSub = departmentSub;
            _profileService = profileService;
            _network = network;
            _account = account;
            _userHelper = userHelper;
            _costCenter = costCenter;
            _fileHelper = fileHelper;
            _escalation = escalation;
        }

         [HttpGet]
        public IActionResult Add(Guid id)
        {
            try
            {
                var srf = Service.GetById(id);
                var Candidate = _candidate.GetById(srf.CandidateId);
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                var CurrentSSO = _ssow.GetById(Vacancy.ServicePackId);
                var Departement = _department.GetById(Vacancy.DepartmentId);
                
                ViewBag.Candidate = Candidate;
                ViewBag.Vacancy = Vacancy;
                ViewBag.Departement = Departement;
                ViewBag.SSOW = CurrentSSO;
                ViewBag.SCategory = _ssowCategory.GetById(CurrentSSO.ServicePackCategoryId);
                ViewBag.JobsStage = _jobStage.GetById(Vacancy.JobStageId);
                ViewBag.ListDepartement = _department.GetAll().ToList();

                if (Departement.OperateOrNon == 1)
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                }
                else
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                }
                ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
                ViewBag.ListHeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.ListHeadNonOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                ViewBag.ListHeadSourcing = _userHelper.GetByRoleName("Head Of Sourcing").ToList();
                ViewBag.ListAccount = _account.GetAll().ToList();
                ViewBag.ListSubDepartment = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                //ViewBag.ListSSOW = _ssow.GetAll().Where(x => x.Type.Equals(CurrentSSO.Type)).ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });


                Dictionary<string, bool> ws = new Dictionary<string, bool>();
                ws.Add("No", false);
                ws.Add("Yes", true);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

                var ProfileId = _userHelper.GetLoginUser(User).Id;
                ViewBag.ProfileId = ProfileId;

                if (srf != null)
                {
                    var Model = new EscalationModelForm();
                    Model.Srf = Mapper.Map<SrfRequestModelForm>(srf);
                    Model.SrfId = id;
                    return View(Model);
                }
            }
            catch(Exception e)
            {
                return Content(e.ToString());
            }
            return NoContent();

        }

        [HttpPost]
        public IActionResult Add(EscalationModelForm model)
        {
            if (ModelState.IsValid)
            {
                var srf = Service.GetById(model.SrfId);
                var Dept = _department.GetById(srf.DepartmentId);

                if (srf != null)
                {

                    var item = Mapper.Map<SrfEscalationRequest>(model);
                    int ProfileId = 0;
                    if (Dept.OperateOrNon == 1)
                    {
                        ProfileId = model.Srf.ApproveTwoId;
                    }
                    else
                    {
                        ProfileId = model.Srf.ApproveFourId;
                    }
                    item.OtLevel = model.OtLevel;
                    item.IsWorkstation = model.IsWorkstation;
                    item.IsCommnunication = model.IsCommnunication;
                    item.SparateValue = model.SparateValue;
                    item.Note = model.Note;
                    item.ServicePackId = model.ServicePackId;

                    //nitip
                    item.CustomField1 = model.PackageTypeId?.ToString();
                    item.CustomField2 = model.ServicePackCategoryId?.ToString();

                    if (!string.IsNullOrWhiteSpace(model.Files))
                    {
                        string fileDestination = "uploads/escalation";
                        var AttachmentUpload = model.Files.Split('|');
                        var listSaveAttachment = new List<string>();
                        if (AttachmentUpload != null)
                        {
                            foreach (var file in AttachmentUpload)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    string FileName = Path.GetFileNameWithoutExtension(file).ToSlug();
                                    string MovedFiles = _fileHelper.FileMove(file, fileDestination, FileName, true);
                                    if (!string.IsNullOrEmpty(MovedFiles))
                                    {
                                        listSaveAttachment.Add(MovedFiles);
                                    }

                                }
                            }
                            if (listSaveAttachment != null)
                            {
                                item.Files = JsonConvert.SerializeObject(listSaveAttachment);
                            }
                        }
                    }


                    _escalation.Add(item);


                    // Update Srf General
                    srf.SrfBegin = model.Srf.SrfBegin;
                    srf.SrfEnd = model.Srf.SrfEnd;
                    srf.AnnualLeave = Extension.MonthDistance(model.Srf.SrfBegin, model.Srf.SrfEnd);
                    srf.DepartmentId = model.Srf.DepartmentId;
                    srf.DepartmentSubId = model.Srf.DepartmentSubId;
                    srf.CostCenterId = model.Srf.CostCenterId;
                    srf.AccountId = model.Srf.AccountId;
                    srf.NetworkId = model.Srf.NetworkId;
                    srf.ApproveFiveId = model.Srf.ApproveFiveId;
                    srf.ApproveSixId = model.Srf.ApproveSixId;
                    srf.SpectValue = model.SparateValue;
                    if (model.SparateValue > 0)
                    {
                        srf.RateType = RateType.SpecialRate;
                    }
                    else
                    {
                        srf.RateType = RateType.Normal;
                    }
                    //srf.RateType = RateType.SpecialRate;
                    Service.Update(srf);

                    // Update Vacancy
                    var Candidate = _candidate.GetById(srf.CandidateId);
                    if (Candidate != null)
                    {
                        var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                        if (Vacancy != null)
                        {
                            Vacancy.AccountNameId = srf.AccountId.Value;
                            Vacancy.CostCodeId = srf.CostCenterId;
                            Vacancy.DepartmentId = srf.DepartmentId;
                            Vacancy.DepartmentSubId = srf.DepartmentSubId;
                            Vacancy.NetworkId = srf.NetworkId;
                            Vacancy.ApproverOneId = srf.ApproveOneId;
                            Vacancy.ServicePackId = srf.ServicePackId;
                            Vacancy.isLaptop = srf.isWorkstation;
                            Vacancy.isUsim = srf.isCommunication;
                            Vacancy.isHrms = srf.IsHrms;
                            Vacancy.OtLevel = model.OtLevel;
                            _vacancy.Update(Vacancy);
                        }
                    }
                }
            }
            TempData["Saved"] = "OK";
            return RedirectToAction("Edit", "ContractorDataEscalation", new { id = model.SrfId });
        }

        [HttpGet]
        public override IActionResult Edit(Guid id)
        {
            try
            {
                var srf = Service.GetById(id);
                var item = _escalation.GetAll().Where(x => x.SrfId.Equals(id)).FirstOrDefault();
                var Candidate = _candidate.GetById(srf.CandidateId);
                var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                var CurrentSSO = _ssow.GetById(srf.ServicePackId);
                var Departement = _department.GetById(srf.DepartmentId);
                ViewBag.Candidate = Candidate;
                ViewBag.Vacancy = Vacancy;
                ViewBag.Departement = Departement;
                ViewBag.SSOW = CurrentSSO;
                ViewBag.SCategory = _ssowCategory.GetById(CurrentSSO.ServicePackCategoryId);
                ViewBag.JobsStage = _jobStage.GetById(Vacancy.JobStageId);
                ViewBag.ListDepartement = _department.GetAll().ToList();

                if (Departement.OperateOrNon == 1)
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                }
                else
                {
                    ViewBag.HeadOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                }
                ViewBag.ListServiceLine = _userHelper.GetByRoleName("Head Of Service Line").ToList();
                ViewBag.ListServiceCordinator = _userHelper.GetByRoleName("Service Coordinator").ToList();
                ViewBag.ListHeadOperation = _userHelper.GetByRoleName("Head Of Operation").ToList();
                ViewBag.ListHeadNonOperation = _userHelper.GetByRoleName("Head Of Non Operation").ToList();
                ViewBag.ListHeadSourcing = _userHelper.GetByRoleName("Head Of Sourcing").ToList();
                ViewBag.ListAccount = _account.GetAll().ToList();
                ViewBag.ListSubDepartment = _departmentSub.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListCostCenter = _costCenter.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                ViewBag.ListNetwork = _network.GetAll().Where(x => x.DepartmentId.Equals(srf.DepartmentId)).ToList();
                //ViewBag.ListSSOW = _ssow.GetAll().Where(x => x.Type.Equals(CurrentSSO.Type)).ToList();
                ViewBag.PackageType = _packageType.GetAll().ToList();
                ViewBag.ServicePackCategory = _ssowCategory.GetAll().ToList();
                ViewBag.BasicServiceLevel = new List<int>(new int[] { 0, 20, 30, 40 }).Select(x => new { Id = x, Name = x.ToString() });


                Dictionary<string, bool> ws = new Dictionary<string, bool>();
                ws.Add("No", false);
                ws.Add("Yes", true);

                Dictionary<string, bool> com = new Dictionary<string, bool>();
                com.Add("No USIM", false);
                com.Add("USIM", true);

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.EscalationId = item.Id;
                ViewBag.ServiceCode = _ssow.GetById(item.ServicePackId).Code;
                ViewBag.SrfNumber = srf.Number;

                var PreofileId = _userHelper.GetUser(User).UserProfile.Id;
                ViewBag.PreofileId = PreofileId;
                ViewBag.LineManagerId = srf.ApproveOneId;
                ViewBag.ServiceLineId = srf.ApproveTwoId;
                ViewBag.HeadOpId = srf.ApproveThreeId;
                ViewBag.HeadNonId = srf.ApproveFourId;
                ViewBag.HeadOfSourceId = srf.ApproveFourId;
                ViewBag.ServiceCoordId = srf.ApproveSixId;

                ViewBag.WorkstationService = ws.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();
                ViewBag.CommunicationService = com.Select(x => new { Id = x.Value, Name = x.Key.ToString() }).ToList();

                if (srf != null)
                {
                    var Model = Mapper.Map<EscalationModelForm>(item);
                    Model.Srf = Mapper.Map<SrfRequestModelForm>(srf);
                    Model.SrfId = id;
                    string typeName = CurrentSSO.Type.ToString();
                    Guid packageTypeId, servicePackCategoryId;
                    if (Guid.TryParse(item.CustomField1, out packageTypeId))
                        Model.PackageTypeId = packageTypeId;
                    if (Guid.TryParse(item.CustomField2, out servicePackCategoryId))
                        Model.ServicePackCategoryId = servicePackCategoryId;
                    if (!string.IsNullOrEmpty(item.Files))
                    {
                        var attachments = (JsonConvert.DeserializeObject<List<string>>(item.Files)
                            ?? new List<string>());
                        Model.Files = string.Join("|", attachments.ToArray()) + "|";
                    }
                    else
                    {
                        Model.Files = null;
                    }
                    return View(Model);
                }
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Update(EscalationModelForm model)
        {
            if (ModelState.IsValid)
            {
                var srf = Service.GetById(model.SrfId);
                if (srf != null)
                {
                    var item = _escalation.GetAll().Where(x => x.SrfId.Equals(model.SrfId)).FirstOrDefault();
                    var Dept = _department.GetById(srf.DepartmentId);
                    int ProfileId = 0;
                    if (Dept.OperateOrNon == 1)
                    {
                        ProfileId = model.Srf.ApproveTwoId;
                    }
                    else
                    {
                        ProfileId = model.Srf.ApproveFourId;
                    }
                    item.OtLevel = model.OtLevel;
                    item.IsWorkstation = model.IsWorkstation;
                    item.IsCommnunication = model.IsCommnunication;
                    item.SparateValue = model.SparateValue;
                    item.Note = model.Note;
                    item.ServicePackId = model.ServicePackId;

                    //nitip
                    item.CustomField1 = model.PackageTypeId?.ToString();
                    item.CustomField2 = model.ServicePackCategoryId?.ToString();

                    if (!string.IsNullOrWhiteSpace(model.Files))
                    {
                        var split = model.Files.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        item.Files = JsonConvert.SerializeObject(split);
                    }

                    _escalation.Update(item);

                    // Update Srf General
                    srf.SrfBegin = model.Srf.SrfBegin;
                    srf.SrfEnd = model.Srf.SrfEnd;
                    srf.AnnualLeave = Extension.MonthDistance(model.Srf.SrfBegin, model.Srf.SrfEnd);
                    srf.DepartmentId = model.Srf.DepartmentId;
                    srf.DepartmentSubId = model.Srf.DepartmentSubId;
                    srf.CostCenterId = model.Srf.CostCenterId;
                    srf.AccountId = model.Srf.AccountId;
                    srf.NetworkId = model.Srf.NetworkId;
                    srf.ApproveFiveId = model.Srf.ApproveFiveId;
                    srf.ApproveSixId = model.Srf.ApproveSixId;
                    srf.SpectValue = model.SparateValue;
                    if (model.SparateValue > 0)
                    {
                        srf.RateType = RateType.SpecialRate;
                    }
                    else
                    {
                        srf.RateType = RateType.Normal;
                    }
                    //srf.RateType = RateType.SpecialRate;
                    Service.Update(srf);

                    // Update Vacancy
                    var Candidate = _candidate.GetById(srf.CandidateId);
                    if (Candidate != null)
                    {
                        var Vacancy = _vacancy.GetById(Candidate.VacancyId);
                        if (Vacancy != null)
                        {
                            Vacancy.AccountNameId = srf.AccountId.Value;
                            Vacancy.CostCodeId = srf.CostCenterId;
                            Vacancy.DepartmentId = srf.DepartmentId;
                            Vacancy.DepartmentSubId = srf.DepartmentSubId;
                            Vacancy.NetworkId = srf.NetworkId;
                            Vacancy.ApproverOneId = srf.ApproveOneId;
                            Vacancy.ServicePackId = srf.ServicePackId;
                            Vacancy.isLaptop = srf.isWorkstation;
                            Vacancy.isUsim = srf.isCommunication;
                            Vacancy.isHrms = srf.IsHrms;
                            Vacancy.OtLevel = model.OtLevel;
                            _vacancy.Update(Vacancy);
                        }
                    }
                }

            }
            TempData["Saved"] = "OK";
            return RedirectToAction("Edit", "ContractorDataEscalation", new { id = model.SrfId });
        }

        public IActionResult DeleteEscalation(Guid id)
        {
            var srf = Service.GetById(id);
            if(srf!=null)
            {
                var Escalation = _escalation.GetAll().Where(x => x.SrfId.Equals(id)).FirstOrDefault();
                if(Escalation!=null)
                {
                    _escalation.Delete(Escalation);

                    srf.SpectValue = 0;
                    srf.RateType = RateType.Normal;
                    srf.ApproveStatusFive = SrfApproveStatus.Waiting;
                    srf.ApproveFiveId = null;
                    Service.Update(srf);

                    return RedirectToAction("Edit", "SrfProfile", new { area = "Admin" , id = id });
                }
            }
            return NoContent();
        }

    }
}
