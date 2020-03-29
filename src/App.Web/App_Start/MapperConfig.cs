using App.Domain.Mapper;
using App.Domain.Models.Core;
using App.Domain.Models.Identity;
using App.Domain.Models.Localization;
using App.Web.Models.ViewModels;
using App.Web.Models.ViewModels.Core;
using App.Web.Models.ViewModels.Core.Business;
using App.Web.Models.ViewModels.Identity;
using App.Web.Models.ViewModels.Localization;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.App_Start
{
    public static class MapperConfig
    {
        public static void Map(IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                #region Identity

                cfg.CreateMap<ApplicationUser, ApplicationUserViewModel>()
                    .ForMember(x => x.Username, opt => opt.MapFrom(x => x.UserName))
                    .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
                    .ForMember(x => x.UserProfile, opt => opt.MapFrom(x => x.UserProfile))
                    .ReverseMap();

                cfg.CreateMap<ApplicationUser, ApplicationUserForm>()
                    .ForMember(x => x.Username, opt => opt.MapFrom(x => x.UserName))
                    .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
                    .ForMember(x => x.Name, opt => opt.MapFrom(x => x.UserProfile.Name))
                    .ForMember(x => x.Birthplace, opt => opt.MapFrom(x => x.UserProfile.Birthplace))
                    .ForMember(x => x.Birthdate, opt => opt.MapFrom(x => x.UserProfile.Birthdate))
                    .ForMember(x => x.Address, opt => opt.MapFrom(x => x.UserProfile.Address))
                    .ForMember(x => x.Photo, opt => opt.MapFrom(x => x.UserProfile.Photo))
                    .ReverseMap();

                cfg.CreateMap<ApplicationUser, UpdateProfileForm>()
                .ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.UserProfile.Name))
                .ForMember(x => x.Birthplace, opt =>opt.MapFrom(x => x.UserProfile.Birthplace))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(x => x.UserProfile.Birthdate))
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.UserProfile.Address))
                .ForMember(x => x.Photo, opt => opt.MapFrom(x => x.UserProfile.Photo))
                .ForMember(x => x.HomePhoneNumber, opt => opt.MapFrom(x => x.UserProfile.HomePhoneNumber))
                .ForMember(x => x.MobilePhoneNumber, opt => opt.MapFrom(x => x.UserProfile.MobilePhoneNumber))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.UserProfile.Description))
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.UserProfile.Email))
                .ReverseMap();

                cfg.CreateMap<ApplicationRole, RoleForm>()
                    .ReverseMap();

                cfg.CreateMap<ApplicationRole, RoleViewModel>()
                    .ReverseMap();

                cfg.CreateMap<UserProfile, UserProfileViewModel>()
                    .ReverseMap();

                cfg.CreateMap<UserProfile, UserProfileFormViewModel>()
                    .ReverseMap();

                cfg.CreateMap<UserProfile, UserProfileFormViewModel>()
                    .ReverseMap();

                cfg.CreateMap<UserProfile, CandidateInfoModelForm>()
                  .ReverseMap();

                #endregion

                #region Core

                cfg.CreateMap<WebSetting, WebSettingViewModel>()
                    .ReverseMap();

                cfg.CreateMap<WebSetting, WebSettingForm>()
                    .ReverseMap();

                #endregion

                #region Core Business
                cfg.CreateMap<AccountName, AccountNameViewModel>()
                   .ReverseMap();
                cfg.CreateMap<AccountName, AccountNameModelForm>()
                    .ReverseMap();

                cfg.CreateMap<AllowanceForm, AllowanceFormViewModel>()
                    .ReverseMap();
                cfg.CreateMap <AllowanceForm, AllowanceFormModelForm>()
                    .ReverseMap();

                cfg.CreateMap<ActivityCode, ActivityCodeViewModel>()
                  .ReverseMap();
                cfg.CreateMap<ActivityCode, ActivityCodeModelForm>()
                    .ReverseMap();

                cfg.CreateMap<AllowanceList, AllowanceListViewModel>()
                    .ForMember(x => x.ServicePack, opt => opt.MapFrom(x => x.ServicePack))
                    .ForMember(x => x.ServicePackName, opt => opt.MapFrom(x => x.ServicePack.Name))
                    .ForMember(x => x.ServicePackCategory, opt => opt.MapFrom(x => x.ServicePack.ServicePackCategory))
                    .ForMember(x => x.ServicePackCategoryName, opt => opt.MapFrom(x => x.ServicePack.ServicePackCategory.Name))
                    .ReverseMap();
                cfg.CreateMap<AllowanceList, AllowanceListModelForm>()
                    .ReverseMap();

                cfg.CreateMap<City, CityViewModel>()
                    .ReverseMap();
                cfg.CreateMap<City, CityModelForm>()
                    .ReverseMap();

                cfg.CreateMap<ASP, ASPViewModel>()
                    .ReverseMap();
                cfg.CreateMap<ASP, ASPModelForm>()
                    .ReverseMap();

                cfg.CreateMap<ClaimCategory, ClaimCategoryViewModel>()
                     .ReverseMap();
                cfg.CreateMap<ClaimCategory, ClaimCategoryModelForm>()
                     .ReverseMap();

                cfg.CreateMap<CostCenter, CostCenterViewModel>()
                     .ReverseMap();
                cfg.CreateMap<CostCenter, CostCenterModelForm>()
                     .ReverseMap();

                cfg.CreateMap<Departement, DepartementViewModel>()
                    .ReverseMap();
                cfg.CreateMap<Departement, DepartementModelForm>()
                     .ReverseMap();

                cfg.CreateMap<DepartementSub, DepartementSubViewModel>()
                    .ForMember(x => x.DepartementName, opt => opt.MapFrom(x => x.Departement.Name))
                    .ForMember(x => x.LineManagerName, opt => opt.MapFrom(x => !string.IsNullOrWhiteSpace(x.LineManager.Name)?x.LineManager.Name:""))
                    .ReverseMap();
                cfg.CreateMap<DepartementSub, DepartementSubModelForm>()
                     .ReverseMap();

                cfg.CreateMap<JobStage, JobStageViewModel>()
                    .ReverseMap();
                cfg.CreateMap<JobStage, JobStageModelForm>()
                     .ReverseMap();

                cfg.CreateMap<Holidays, HolidaysViewModel>()
                   .ReverseMap();
                cfg.CreateMap<Holidays, HolidaysFormModel>()
                     .ReverseMap();

                cfg.CreateMap<NetworkNumber, NetworkNumberViewModel>()
                   .ReverseMap();
                cfg.CreateMap<NetworkNumber, NetworkNumberModelForm>()
                     .ReverseMap();

                cfg.CreateMap<Projects, ProjectViewModel>()
                    .ReverseMap();
                cfg.CreateMap<Projects, ProjectModelForm>()
                     .ReverseMap();

                cfg.CreateMap<PackageType, PackageTypeViewModel>()
                   .ReverseMap();
                cfg.CreateMap<PackageType, PackageTypeyModelForm>()
                     .ReverseMap();

                cfg.CreateMap<ServicePack, ServicePackViewModel>()
                    .ForMember(x => x.ServicePackCategoryName, opt => opt.MapFrom(x => x.ServicePackCategory.Name))
                   .ReverseMap();
                cfg.CreateMap<ServicePack, ServicePackModelForm>()
                    .ReverseMap();

                cfg.CreateMap<ServicePackCategory, ServicePackCategoryViewModel>()
                   .ReverseMap();
                cfg.CreateMap<ServicePackCategory, ServicePackCategoryModelForm>()
                    .ReverseMap();

                cfg.CreateMap<SubOps, SubOpsViewModel>()
                    .ReverseMap();
                cfg.CreateMap<SubOps, SubOpsModelForm>()
                    .ReverseMap();

                cfg.CreateMap<SrfRequest, SrfRequestViewModel>()
                    .ReverseMap();
                cfg.CreateMap<SrfRequest, SrfRequestModelForm>()
                    .ReverseMap();

                cfg.CreateMap<TimeSheetType, TimeSheetTypeViewModel>()
                    .ReverseMap();
                cfg.CreateMap<TimeSheetType, TimeSheetTypeModelForm>()
                    .ReverseMap();

                cfg.CreateMap<Claim, TravelRequestViewModel>()
                   .ReverseMap();
                cfg.CreateMap<Claim, TravelRequestModelForm>()
                    .ReverseMap();

                cfg.CreateMap<Assignment, AssignmentViewModel>()
                    .ForMember(x => x.Asp, opt => opt.MapFrom(x => x.Asp.Name))
                    .ReverseMap();
                cfg.CreateMap<Assignment, AssignmentFormModel>()
                    .ReverseMap();

                cfg.CreateMap<Bast, BastViewModel>()
                    .ForMember(x => x.ApproverOne, opt => opt.MapFrom(x => x.ApprovalOne.Name))
                    .ForMember(x => x.ApproverTwo, opt => opt.MapFrom(x => x.ApprovalTwo.Name))
                    .ForMember(x => x.ApproverThree, opt => opt.MapFrom(x => x.ApprovalThree.Name))
                    .ForMember(x => x.ApproverFour, opt => opt.MapFrom(x => x.ApprovalFour.Name))
                    .ForMember(x => x.RequestBy, opt => opt.MapFrom(x => x.RequestBy.Name))
                    .ForMember(x => x.Asp, opt => opt.MapFrom(x => x.Asp.Name))
                    .ReverseMap();
                cfg.CreateMap<Bast, BastFormModel>()
                    .ReverseMap();

                cfg.CreateMap<MapAsgBast, MapAsgBastViewModel>()
                    .ReverseMap();
                cfg.CreateMap<MapAsgBast, MapAsgBastFormModel>()
                    .ReverseMap();

                cfg.CreateMap<Claim, ClaimViewModel>()
                    .ReverseMap();
                cfg.CreateMap<Claim, ClaimModelForm>()
                    .ForMember(x => x.ActivityCodeId, opt => opt.MapFrom(x => x.ActivityCodeId))
                    .ReverseMap();

                cfg.CreateMap<VacancyList, VacancyListViewModel>()
                     .ForMember(x => x.DepartmentName, opt => opt.MapFrom(x => x.Departement.Name))
                     .ForMember(x => x.DepartmentSubName, opt => opt.MapFrom(x => x.DepartementSub.SubName))
                     .ForMember(x => x.ServicePackName, opt => opt.MapFrom(x => x.ServicePack.Name))
                     .ForMember(x => x.NetworkName, opt => opt.MapFrom(x => x.Network.DisplayName))
                     .ForMember(x => x.ServicePackCategoryName, opt => opt.MapFrom(x => x.ServicePack.ServicePackCategory.Name))
                     .ForMember(x => x.ApproverOne, opt => opt.MapFrom(x => x.ApproverOne.Name))
                     .ForMember(x => x.ApproverTwo, opt => opt.MapFrom(x => x.ApproverTwo.Name))
                     .ForMember(x => x.Vendor, opt => opt.MapFrom(x => x.Vendor.Name))

                     .ReverseMap();
                cfg.CreateMap<VacancyList, VacancyListFormModel>()
                    .ReverseMap();

                cfg.CreateMap<CandidateInfo, CandidateInfoViewModel>()
                   .ReverseMap();

                cfg.CreateMap<CandidateInfo, CandidateInfoModelForm>()
                   .ReverseMap();

                cfg.CreateMap<SrfEscalationRequest, EscalationViewModel>()
                  .ReverseMap();
                cfg.CreateMap<SrfEscalationRequest, EscalationModelForm>()
                  .ReverseMap();

                cfg.CreateMap<AttendaceExceptionList, AttendaceExceptionListViewModel>()
                .ReverseMap();
                cfg.CreateMap<AttendaceExceptionList, AttendaceExceptionListModelForm>()
                .ReverseMap();

                cfg.CreateMap<TicketInfo, TicketInfoViewModel>()
                 .ReverseMap();
                cfg.CreateMap<TicketInfo, TicketInfoFormModel>()
                 .ReverseMap();

                cfg.CreateMap<TacticalResource, TacticalResourceViewModel>()
                  .ReverseMap();

                cfg.CreateMap<TacticalResource, TacticalResourceFormModel>()
                    .ReverseMap();

                #endregion

                #region Localization

                cfg.CreateMap<Language, LanguageViewModel>()
                    .ReverseMap();

                cfg.CreateMap<Language, LanguageForm>()
                    .ReverseMap();

                cfg.CreateMap<LocaleResource, LocaleResourceViewModel>()
                    .ReverseMap();

                cfg.CreateMap<LocaleResource, LocaleResourceForm>()
                    .ReverseMap();

                #endregion

                DtoMapping.Map(cfg);
            });
        }
    }
}
