using App.Services.Core;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Services.Localization;
using App.Services.Messages;
using App.Services.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services
{
    public static class ServicesRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            #region Identity

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserProfileService, UserProfileService>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<IMailSenderService, MailSenderService>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ISmtpOptionsService, SmtpOptionsService>();

            #endregion

            #region Core

            services.AddTransient<IWebSettingService, WebSettingService>();
            services.AddTransient<IEmailArchieveService, EmailArchieveService>();
            services.AddTransient<IAccountNameService, AccountNameService>();
            services.AddTransient<IActivityCodeService, ActivityCodeService>();
            services.AddTransient<IAllowanceFormService, AllowanceFormService>();
            services.AddTransient<IAllowanceListService, AllowanceListService>();
            services.AddTransient<IAttendaceExceptionListService, AttendaceExceptionListService>();
            services.AddTransient<IAttendanceRecordService, AttendanceRecordService>();
            services.AddTransient<IAutoGenerateVariableService, AutoGenerateVariableService>();
            services.AddTransient<IBackupLogService, BackupLogService>();
            services.AddTransient<IBusinessNoteService, BusinessNoteService>();
            services.AddTransient<ICandidateInfoService, CandidateInfoService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IASPService, ASPService>();
            services.AddTransient<IClaimService, ClaimService>();
            services.AddTransient<IClaimCategoryService, ClaimCategoryService>();
            services.AddTransient<ICostCenterService, CostCenterService>();
            services.AddTransient<ICustomerClaimService, CustomerClaimService>();
            services.AddTransient<ICustomerDataService, CustomerDataService>();
            services.AddTransient<IDepartementService, DepartementService>();
            services.AddTransient<IDepartementSubService, DepartementSubService>();
            services.AddTransient<IDutyScheduleService, DutyScheduleService>();
            services.AddTransient<IFingerPrintService, FingerPrintService>();
            services.AddTransient<IFortestService, FortestService>();
            services.AddTransient<IGenerateLogService, GenerateLogService>();
            services.AddTransient<IJobStageService, JobStageService>();
            services.AddTransient<INetworkNumberService, NetworkNumberService>();
            services.AddTransient<IPackageTypeService, PackageTypeService>();
            services.AddTransient<IPanelCategoryService, PanelCategoryService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<IProjectsService, ProjectsService>();
            services.AddTransient<IRequestSparePartsService, RequestSparePartsService>();
            services.AddTransient<IServicePackService, ServicePackService>();
            services.AddTransient<IServicePackCategoryService, ServicePackCategoryService>();
            services.AddTransient<ISrfEscalationRequestService, SrfEscalationRequestService>();
            services.AddTransient<ISrfRequestService, SrfRequestService>();
            services.AddTransient<ISrfMigrationService, SrfMigrationService>();
            services.AddTransient<ISubdivisionService, SubdivisionService>();
            services.AddTransient<ISubOpsService, SubOpsService>();
            services.AddTransient<ISystemBranchService, SystemBranchService>();
            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<ITicketReplyService, TicketReplyService>();
            services.AddTransient<ITimeSheetPeriodService, TimeSheetPeriodService>();
            services.AddTransient<ITimeSheetTypeService, TimeSheetTypeService>();
            services.AddTransient<IVacancyListService, VacancyListService>();
            services.AddTransient<IAssignmentService, AssignmentService>();
            services.AddTransient<IBastService, BastService>();
            services.AddTransient<IMapAsgBastService, MapAsgBastService>();
            services.AddTransient<IWoItemService, WoItemService>();
            services.AddTransient<IWotListService, WotListService>();
            services.AddTransient<ITicketInfoService, TicketInfoService>();
            services.AddTransient<IHolidaysService, HolidaysService>();
            services.AddTransient<ITacticalResourceService, TacticalResourceService>();
            services.AddTransient<IDashboardService, DashboardService>();
            #endregion

            #region Localization

            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<ILocaleResourceService, LocaleResourceService>();

            #endregion
        }
    }
}
