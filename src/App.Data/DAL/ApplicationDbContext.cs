using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using App.Data.Utils;
using App.Domain.Models.Core;
using App.Domain.Models.Localization;

namespace App.Data.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Identity
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual new DbSet<ApplicationRole> Roles { get; set; }

        #endregion

        #region Core

        #region Default

        public virtual DbSet<WebSetting> WebSettings { get; set; }
        public virtual DbSet<EmailArchieve> EmailArchieves { get; set; }

        #endregion

        public virtual DbSet<AccountName> AccountName { get; set; }

        public virtual DbSet<AllowanceForm> AllowanceForm { get; set; }

        public virtual DbSet<ActivityCode> ActivityCode { get; set; }

        public virtual DbSet<AllowanceList> AllowanceList { get; set; }

        public virtual DbSet<AttendaceExceptionList> AttendaceExceptionList { get; set; }

        public virtual DbSet<AttendanceRecord> AttendanceRecord { get; set; }

        public virtual DbSet<AutoGenerateVariable> AutoGenerateVariable { get; set; }

        public virtual DbSet<BackupLog> BackupLog { get; set; }

        public virtual DbSet<BusinessNote> BusinessNote { get; set; }

        public virtual DbSet<CandidateInfo> CandidateInfo { get; set; }

        public virtual DbSet<City> City { get; set; }

        public virtual DbSet<ASP> ASP { get; set; }

        public virtual DbSet<Claim> Claim { get; set; }

        public virtual DbSet<ClaimCategory> ClaimCategory { get; set; }

        public virtual DbSet<CostCenter> CostCenter { get; set; }

        public virtual DbSet<CustomerClaim> CustomerClaim { get; set; }

        public virtual DbSet<CustomerData> CustomerData { get; set; }

        public virtual DbSet<Departement> Departement { get; set; }

        public virtual DbSet<DepartementSub> DepartementSub { get; set; }

        public virtual DbSet<DutySchedule> DutySchedule { get; set; }

        public virtual DbSet<FingerPrint> FingerPrint { get; set; }

        public virtual DbSet<Fortest> Fortest { get; set; }

        public virtual DbSet<GenerateLog> GenerateLog { get; set; }

        public virtual DbSet<JobStage> JobStage { get; set; }

        public virtual DbSet<NetworkNumber> NetworkNumber { get; set; }

        public virtual DbSet<PackageType> PackageType { get; set; }

        public virtual DbSet<PanelCategory> PanelCategory { get; set; }

        public virtual DbSet<Position> Position { get; set; }

        public virtual DbSet<Projects> Projects { get; set; }

        public virtual DbSet<RequestSpareParts> RequestSpareParts { get; set; }

        public virtual DbSet<ServicePack> ServicePack { get; set; }

        public virtual DbSet<ServicePackCategory> ServicePackCategory { get; set; }

        public virtual DbSet<SrfEscalationRequest> SrfEscalationRequest { get; set; }

        public virtual DbSet<SrfRequest> SrfRequest { get; set; }

        public virtual DbSet<Subdivision> Subdivision { get; set; }

        public virtual DbSet<SubOps> SubOps { get; set; }

        public virtual DbSet<SystemBranch> SystemBranch { get; set; }

        public virtual DbSet<SystemPropertiesRecord> SystemPropertiesRecord { get; set; }

        public virtual DbSet<Ticket> Ticket { get; set; }

        public virtual DbSet<TicketInfo> TicketInfo { get; set; }

        public virtual DbSet<TicketReply> TicketReply { get; set; }

        public virtual DbSet<TimeSheetPeriod> TimeSheetPeriod { get; set; }

        public virtual DbSet<TimeSheetType> TimeSheetType { get; set; }

        public virtual DbSet<VacancyList> VacancyList { get; set; }

        public virtual DbSet<Assignment> Assignment { get; set; }

        public virtual DbSet<Bast> Bast { get; set; }

        public virtual DbSet<MapAsgBast> MapAsgBast { get; set; }

        public virtual DbSet<WoItem> WoItem { get; set; }

        public virtual DbSet<WotList> WotList { get; set; }

        public virtual DbSet<Holidays> Holidays { get; set; }

        public virtual DbSet<TacticalResource> TacticalResource { get; set; }
        #endregion

        #region Localization
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LocaleResource> LocaleResources { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.RemovePluralizingTableNameConvention();

            builder.HasPostgresExtension("uuid-ossp");

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.UserProfile)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey<UserProfile>(x => x.ApplicationUserId);

            builder.Entity<SrfRequest>()
               .HasOne(x => x.Escalation)
               .WithOne(x => x.SrfRequest)
               .HasForeignKey<SrfEscalationRequest>(x => x.SrfId);

            builder.Entity<Claim>()
              .HasOne(x => x.Ticket)
              .WithOne(x => x.Claim)
              .HasForeignKey<TicketInfo>(x => x.ClaimId);
        }
    }
}
