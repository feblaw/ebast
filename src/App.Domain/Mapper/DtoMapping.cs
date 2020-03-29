using App.Domain.DTO.Core;
using App.Domain.DTO.Identity;
using App.Domain.DTO.Localization;
using App.Domain.Models.Core;
using App.Domain.Models.Enum;
using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using AutoMapper;
using System;

namespace App.Domain.Mapper
{
    public static class DtoMapping
    {
        public static void Map(IMapperConfigurationExpression cfg)
        {
            #region Identity

            cfg.CreateMap<ApplicationRole, ApplicationRoleDto>()
                .ReverseMap();

            cfg.CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(x => x.Username, opt => opt.MapFrom(x => x.UserName))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.UserProfile.Name))
                .ForMember(x => x.Roles, opt => opt.MapFrom(x => x.UserProfile.Roles))
                .ForMember(x => x.Status, opt => opt.MapFrom(x => x.UserProfile.IsActive))
                .ReverseMap();

            cfg.CreateMap<UserProfile, UserProfileDto>()
               .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ApplicationUserId))
               .ForMember(x => x.Status, opt => opt.MapFrom(x => x.IsActive))
               .ForMember(x => x.Ahid, opt => opt.MapFrom(x => x.AhId))
              .ReverseMap();

            #endregion

            #region core business

            cfg.CreateMap<AccountName, AccountNameDto>()
                .ForMember(x => x.ComName, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.Coms.Name)?x.Coms.Name:""))
                .ReverseMap();

            cfg.CreateMap<ActivityCode, ActivityCodeDto>()
                .ReverseMap();

            cfg.CreateMap<AllowanceForm, AllowanceFormDto>()
                .ReverseMap();

            cfg.CreateMap<AllowanceList, AllowanceListDto>()
                .ForMember(x => x.ServicePacks, opt => opt.MapFrom(x => x.ServicePack))
                .ReverseMap();

            cfg.CreateMap<City, CityDto>()
                .ReverseMap();

            cfg.CreateMap<ASP, ASPDto>()
                .ReverseMap();

            cfg.CreateMap<ClaimCategory, ClaimCategoryDto>()
                .ReverseMap();

            cfg.CreateMap<CostCenter, CostCenterDto>()
                .ForMember(dest => dest.DepartementName, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.Departement.Name)?x.Departement.Name:""))
                .ReverseMap();

            cfg.CreateMap<Departement, DepartementDto>()
                .ForMember(dest => dest.HeadName, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.Head.Name)?x.Head.Name:""))
                .ReverseMap();

            cfg.CreateMap<DepartementSub, DepartementSubDto>()
                .ReverseMap();
            cfg.CreateMap<ServicePack, ServicePackDto>()
                .ForMember(dest => dest.ServicePackCategory, opt => opt.MapFrom(x => x.ServicePackCategory))
                .ForMember(dest => dest.Package, opt => opt.MapFrom(x => Enum.GetName(typeof(PackageTypes),x.Type)))
                .ReverseMap();

            cfg.CreateMap<Projects, ProjectDto>()
                .ReverseMap();

            cfg.CreateMap<ServicePackCategory, ServicePackCategoryDto>()
                .ReverseMap();

            cfg.CreateMap<JobStage, JobStageDTO>()
                .ReverseMap();

            cfg.CreateMap<SubOps, SubOpsDto>()
                .ReverseMap();

            cfg.CreateMap<SrfRequest, SrfRequestDto>()
               .ReverseMap();

            cfg.CreateMap<NetworkNumber, NetworkNumberDto>()
                .ForMember(dest => dest.Departement, opt => opt.MapFrom(x => x.Departement))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(x => x.AccountName))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(x => x.Project))
                .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.ProjectManager))
                .ReverseMap();

            cfg.CreateMap<TimeSheetType, TimeSheetTypeDto>()
              .ForMember(dest => dest.CountTimeSheet, opt => opt.MapFrom(x => x.ListTimeSheet.Count))
              .ReverseMap();

            cfg.CreateMap<PackageType, PackageTypeDto>()
             .ReverseMap();

            cfg.CreateMap<Holidays, HolidaysDto>()
             .ForMember(dest => dest.TypeName, opt => opt.MapFrom(x => x.DayType.ToString()))
             .ReverseMap();

            cfg.CreateMap<Claim, ClaimDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(x => x.ClaimCategory.Name))
            .ForMember(dest => dest.Contractor, opt => opt.MapFrom(x => x.Contractor.Name))
            .ForMember(dest => dest.Agency, opt => opt.MapFrom(x => x.Agency.Name))
            .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.ClaimApproverOne.Name))
            .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ClaimApproverTwo.Name))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(x => x.CostCenter.DisplayName))
            .ForMember(dest => dest.NetworkNumber, opt => opt.MapFrom(x => x.NetworkNumber.DisplayName))
            .ForMember(dest => dest.Activity, opt => opt.MapFrom(x => x.ActivityCode.DisplayName))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(x => x.ContractorProfile.Vacancy.Departement.Name))
            .ForMember(dest => dest.DepartmentSub, opt => opt.MapFrom(x => x.ContractorProfile.Vacancy.DepartementSub.SubName))
            .ReverseMap();

            cfg.CreateMap<Claim, TravelDto>()
              .ForMember(dest => dest.Contractor, opt => opt.MapFrom(x => x.Contractor.Name))
              .ForMember(dest => dest.Vacancy, opt => opt.MapFrom(x => x.Vacancy.Name))
              .ForMember(dest => dest.Agency, opt => opt.MapFrom(x => x.Agency.Name))
              .ForMember(dest => dest.Departure, opt => opt.MapFrom(x => x.Departure.Name))
              .ForMember(dest => dest.Destionation, opt => opt.MapFrom(x => x.Destination.Name))
              .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.ClaimApproverOne.Name))
              .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ClaimApproverTwo.Name))
              .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(x => x.CostCenter.DisplayName))
              .ForMember(dest => dest.NetworkNumber, opt => opt.MapFrom(x => x.NetworkNumber.DisplayName))
              .ForMember(dest => dest.Activity, opt => opt.MapFrom(x => x.ActivityCode.DisplayName))
              .ForMember(dest => dest.Department, opt => opt.MapFrom(x => x.ContractorProfile.Vacancy.Departement.Name))
              .ForMember(dest => dest.DepartmentSub, opt => opt.MapFrom(x => x.ContractorProfile.Vacancy.DepartementSub.SubName))
              .ForMember(dest => dest.TicketStatus, opt => opt.MapFrom(x => x.Ticket.Status))
              .ForMember(dest => dest.TicketAttachment, opt => opt.MapFrom(x => x.Ticket.Files))
              .ReverseMap();

            cfg.CreateMap<VacancyList, VacancyListDto>()
               .ForMember(dest => dest.BastApprover1, opt => opt.MapFrom(x => x.BastApprover1.Name))
               .ForMember(dest => dest.BastApprover2, opt => opt.MapFrom(x => x.BastApprover2.Name))
               .ForMember(dest => dest.BastApprover3, opt => opt.MapFrom(x => x.BastApprover3.Name))
               .ForMember(dest => dest.RequestBy, opt => opt.MapFrom(x => x.ApproverOne.Name))
               .ForMember(dest => dest.ApproverTwo, opt => opt.MapFrom(x => x.ApproverTwo.Name))
               .ForMember(dest => dest.ApproverThree, opt => opt.MapFrom(x => x.ApproverThree.Name))
               .ForMember(dest => dest.Rpm, opt => opt.MapFrom(x => x.Rpm.Name))
               .ForMember(dest => dest.ServicePackCategory, opt => opt.MapFrom(x => x.ServicePackCategory.Name))
               .ForMember(dest => dest.ServicePack, opt => opt.MapFrom(x => x.ServicePack.Code))
               .ForMember(dest => dest.Vendor, opt => opt.MapFrom(x => x.Vendor.Name))
               //.ForMember(dest => dest.AccountNameId, opt => opt.MapFrom(x => x.AccountName.Name))
               .ForMember(dest => dest.Account, opt => opt.MapFrom(x => x.Departement.Name))
               .ForMember(dest => dest.Project, opt => opt.MapFrom(x => x.DepartementSub.SubName))
               .ReverseMap();

            cfg.CreateMap<MapAsgBast, MapAsgBastDto>()
                .ForMember(dest => dest.Assignment, opt => opt.MapFrom(x => x.IdAsg))
                .ForMember(dest => dest.Bast, opt => opt.MapFrom(x => x.IdBast))
                .ForMember(dest => dest.AssignmentId, opt => opt.MapFrom(x => x.Assignment.AssignmentId))
                .ForMember(dest => dest.SiteName, opt => opt.MapFrom(x => x.Assignment.SiteName))
                .ForMember(dest => dest.PONumber, opt => opt.MapFrom(x => x.Assignment.PONumber))
                .ForMember(dest => dest.LineItemPO, opt => opt.MapFrom(x => x.Assignment.LineItemPO))
                .ForMember(dest => dest.TOP, opt => opt.MapFrom(x => x.Bast.TOP))
                .ForMember(dest => dest.ValueAssignment, opt => opt.MapFrom(x => x.Assignment.ValueAssignment))
                .ForMember(dest => dest.BastReqNo, opt => opt.MapFrom(x => x.Bast.BastReqNo))
                .ForMember(dest => dest.BastNo, opt => opt.MapFrom(x => x.Bast.BastNo))
                .ForMember(dest => dest.ASPName, opt => opt.MapFrom(x => x.Assignment.Asp.Name))
                .ForMember(dest => dest.Approver1, opt => opt.MapFrom(x => x.Bast.ApprovalOne.Name))
                .ForMember(dest => dest.Approver2, opt => opt.MapFrom(x => x.Bast.ApprovalTwo.Name))
                .ForMember(dest => dest.Approver3, opt => opt.MapFrom(x => x.Bast.ApprovalThree.Name))
                .ForMember(dest => dest.Approver4, opt => opt.MapFrom(x => x.Bast.ApprovalFour.Name))
                .ForMember(dest => dest.Approver1Status, opt => opt.MapFrom(x => x.Bast.ApprovalOneStatus))
                .ForMember(dest => dest.Approver2Status, opt => opt.MapFrom(x => x.Bast.ApprovalTwoStatus))
                .ForMember(dest => dest.Approver3Status, opt => opt.MapFrom(x => x.Bast.ApprovalThreeStatus))
                .ForMember(dest => dest.Approver4Status, opt => opt.MapFrom(x => x.Bast.ApprovalFourStatus))
                .ForMember(dest => dest.ApprovalOneDate, opt => opt.MapFrom(x => x.Bast.ApprovalOneDate))
                .ForMember(dest => dest.ApprovalTwoDate, opt => opt.MapFrom(x => x.Bast.ApprovalTwoDate))
                .ForMember(dest => dest.ApprovalThreeDate, opt => opt.MapFrom(x => x.Bast.ApprovalThreeDate))
                .ForMember(dest => dest.ApprovalFourDate, opt => opt.MapFrom(x => x.Bast.ApprovalFourDate))
                .ForMember(dest => dest.ReqBy, opt => opt.MapFrom(x => x.Bast.CreatedBy))
               .ReverseMap();

            cfg.CreateMap<Assignment, AssignmentDto>()
                .ForMember(dest => dest.ASP, opt => opt.MapFrom(x => x.Asp.Name))
                //.ForMember(dest => dest.Bast, opt => opt.MapFrom(x => x.BastId))
               .ReverseMap();

            cfg.CreateMap<Bast, BastDto>()
                .ForMember(dest => dest.ASP, opt => opt.MapFrom(x => x.Asp.Name))
                .ForMember(dest => dest.ApprovalOne, opt => opt.MapFrom(x => x.ApprovalOne.Name))
                .ForMember(dest => dest.ApprovalTwo, opt => opt.MapFrom(x => x.ApprovalTwo.Name))
                .ForMember(dest => dest.ApprovalThree, opt => opt.MapFrom(x => x.ApprovalThree.Name))
                .ForMember(dest => dest.ApprovalFour, opt => opt.MapFrom(x => x.ApprovalFour.Name))
                .ForMember(dest => dest.RequestBy, opt => opt.MapFrom(x => x.RequestBy.Name))
               //.ForMember(dest => dest.Bast, opt => opt.MapFrom(x => x.BastId))
               .ReverseMap();

            cfg.CreateMap<CandidateInfo, CandidateDto>()
              .ForMember(dest => dest.Vacancy, opt => opt.MapFrom(x => x.Vacancy))
              .ForMember(dest => dest.Account, opt => opt.MapFrom(x => x.Account))
              .ForMember(dest => dest.RequestBy, opt => opt.MapFrom(x => x.RequestBy))
              .ForMember(dest => dest.Agency, opt => opt.MapFrom(x => x.Agency))
              .ReverseMap();


            cfg.CreateMap<SrfRequest, SrfRequestDto>()
               .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(x => x.Candidate.Name))
               .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ApproveOneBy.Name))
               .ForMember(dest => dest.ServiceCoordinator, opt => opt.MapFrom(x => x.ApproveSixBy.Name))
               .ForMember(dest => dest.DepartmentSub, opt => opt.MapFrom(x => x.DepartementSub.SubName))
               .ForMember(dest => dest.IsEndSoon, opt => opt.MapFrom(x => x.IsEndSoon()))
               .ForMember(dest => dest.Position, opt => opt.MapFrom(x => x.ServicePack.Name))
               .ForMember(dest => dest.StatusEscalationLineManager, opt => opt.MapFrom(x => x.Escalation.Status))
               .ForMember(dest => dest.StatusEscalationServiceLine, opt => opt.MapFrom(x => x.Escalation.ApproveStatusOne))
               .ForMember(dest => dest.StatusEscalationHeadOperation, opt => opt.MapFrom(x => x.Escalation.ApproveStatusTwo))
               .ForMember(dest => dest.StatusEscalationHeadSourcing, opt => opt.MapFrom(x => x.Escalation.ApproveStatusThree))
               .ForMember(dest => dest.StatusEscalationServiceCoordinator, opt => opt.MapFrom(x => x.Escalation.ApproveStatusFour))
               .ForMember(dest => dest.EscalationRate, opt => opt.MapFrom(x => x.Escalation.SparateValue))
               .ForMember(dest => dest.IsOperation, opt => opt.MapFrom(x => x.Departement.OperateOrNon == 1 ? true : false))
               .ForMember(dest => dest.IsEscalation, opt => opt.MapFrom(x => x.Escalation != null ? true : false))
               .ForMember(dest => dest.PackageType, opt => opt.MapFrom(x => x.Candidate.Vacancy.PackageType != null ? "-" : x.Candidate.Vacancy.PackageType.Name))
               .ReverseMap();

            cfg.CreateMap<SrfEscalationRequest, SrfEscalationRequestDto>()
                .ForMember(dest => dest.ServicePack, opt => opt.MapFrom(x => x.ServicePack))
                .ForMember(dest => dest.SrfRequest, opt => opt.MapFrom(x => x.SrfRequest))
                .ReverseMap();

            cfg.CreateMap<EscalationDto, SrfEscalationRequest>()
                .ForMember(dest => dest.ServicePack, opt => opt.MapFrom(x => x.ServicePack))
                .ForMember(dest => dest.SrfRequest, opt => opt.MapFrom(x => x.SrfRequest))
                .ReverseMap();

            cfg.CreateMap<SrfRequest, ContractorDataDto>()
             .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(x => x.Candidate.Name))
             .ForMember(dest => dest.ContractorAhID, opt => opt.MapFrom(x => x.Candidate.Account.AhId))
             .ForMember(dest => dest.ContractorEmail, opt => opt.MapFrom(x => x.Candidate.Email))
             .ForMember(dest => dest.Position, opt => opt.MapFrom(x => x.ServicePack.Name))
             .ForMember(dest => dest.OrganizationUnit, opt => opt.MapFrom(x => x.Departement.Name))
             .ForMember(dest => dest.SubOrganizationUnit, opt => opt.MapFrom(x => x.DepartementSub.SubName))
             .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ApproveOneBy.Name))
             .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.NetworkNumber.ProjectManager.Name))
             .ForMember(dest => dest.Supplier, opt => opt.MapFrom(x => x.Candidate.Agency.Name))
             .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(x => x.Candidate.Vacancy.PackageType.Name))
             .ReverseMap();

            cfg.CreateMap<SrfRequest, ReconcileDto>()
              .ForMember(dest => dest.Duration, opt => opt.MapFrom(x => x.GetDuration()))
              .ForMember(dest => dest.isUsim, opt => opt.MapFrom(x => x.Candidate.Vacancy.isUsim))
              .ForMember(dest => dest.Project, opt => opt.MapFrom(x => x.NetworkNumber.Project.Name))
              .ForMember(dest => dest.JobStage, opt => opt.MapFrom(x => x.Candidate.Vacancy.JobStage.Stage))
              .ForMember(dest => dest.NetworkNumber, opt => opt.MapFrom(x => x.NetworkNumber.DisplayName))
              .ForMember(dest => dest.PriceType, opt => opt.MapFrom(x => x.Candidate.Vacancy.PackageType.Name))
              .ForMember(dest => dest.Contractor, opt => opt.MapFrom(x => x.Candidate.Name))
              .ForMember(dest => dest.ServicePack, opt => opt.MapFrom(x => x.ServicePack))
              .ForMember(dest => dest.ServicePackCategory, opt => opt.MapFrom(x => x.ServicePack.ServicePackCategory.Name))
              .ForMember(dest => dest.Contractor, opt => opt.MapFrom(x => x.Candidate.Name))
              .ForMember(dest => dest.Department, opt => opt.MapFrom(x => x.Departement.Name))
              .ForMember(dest => dest.DepartmentSub, opt => opt.MapFrom(x => x.DepartementSub.SubName))
              .ForMember(dest => dest.Account, opt => opt.MapFrom(x => x.Account.Name))
              .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(x => x.CostCenter.DisplayName))
              .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ApproveOneBy.Name))
              .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.ProjectManager.Name))
              .ForMember(dest => dest.Agency, opt => opt.MapFrom(x => x.Candidate.Agency.Name))
              .ForMember(dest => dest.IsEscalation, opt => opt.MapFrom(x => x.Escalation == null ? false  : true))
              .ForMember(dest => dest.ServicePackEsc, opt => opt.MapFrom(x => x.Escalation.ServicePack))
              .ForMember(dest => dest.OtLevelEsc, opt => opt.MapFrom(x => x.Escalation.OtLevel))
              .ForMember(dest => dest.IsWorkstationEsc, opt => opt.MapFrom(x => x.Escalation.IsWorkstation))
              .ForMember(dest => dest.IsCommnunicationEsc, opt => opt.MapFrom(x => x.Escalation.IsCommnunication))
              .ForMember(dest => dest.SparateValueEsc, opt => opt.MapFrom(x => x.Escalation.SparateValue))
              .ReverseMap();

            cfg.CreateMap<AttendaceExceptionList, AttendaceExceptionListDto>()
              .ForMember(dest => dest.TimeSheetType, opt => opt.MapFrom(x => x.TimeSheetType.Type))
              .ForMember(dest => dest.Contractor, opt => opt.MapFrom(x => x.Contractor.Name))
              .ForMember(dest => dest.Supplier, opt => opt.MapFrom(x => x.Agency.Name))
              .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(x => x.ApproverOne.Name))
              .ForMember(dest => dest.LineManager, opt => opt.MapFrom(x => x.ApproverTwo.Name))
              .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(x => x.CostCenter.DisplayName))
              .ForMember(dest => dest.NetworkNumber, opt => opt.MapFrom(x => x.Network.DisplayName))
              .ForMember(dest => dest.Activiy, opt => opt.MapFrom(x => x.Activity.DisplayName))
              .ForMember(dest => dest.Department, opt => opt.MapFrom(x => x.Departement.Name))
              .ForMember(dest => dest.DepartmentSub, opt => opt.MapFrom(x => x.DepartementSub.SubName))
              .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(x => x.OtherInfo))
              //.ForMember(dest => dest.DetailTimeSheet, opt => opt.MapFrom(x => x.DetailTimeSheet()))
              .ForMember(dest => dest.IsAnnual, opt => opt.MapFrom(x => x.IsAnnual()))
              .ForMember(dest => dest.Location, opt => opt.MapFrom(x => x.Location.Name))
              .ReverseMap();

            cfg.CreateMap<TacticalResource, TacticalResourceDto>()
               .ForMember(dest => dest.Departement, opt => opt.MapFrom(x => x.Departement))
               .ForMember(dest => dest.DepartementSub, opt => opt.MapFrom(x => x.DepartementSub))
               .ReverseMap();

            #endregion
            #region Core

            cfg.CreateMap<WebSetting, WebSettingDto>()
                .ReverseMap();

            cfg.CreateMap<EmailArchieve, EmailArchieveDto>()
                .ReverseMap();


            #endregion

            #region Localization

            cfg.CreateMap<Language, LanguageDto>()
                .ReverseMap();

            cfg.CreateMap<LocaleResource, LocaleResourceDto>()
                .ForMember(x => x.Language, opt => opt.MapFrom(x => x.Language))
                .ReverseMap();

            #endregion
        }
    }
}
