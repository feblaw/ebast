using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using App.Data.DAL;
using App.Domain.Models.Enum;

namespace App.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170515005458_adding master data holidays")]
    partial class addingmasterdataholidays
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("App.Domain.Models.Core.AccountName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Com");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<bool>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("Com");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("AccountName");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ActivityCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("ActivityCode");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AllowanceForm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("AllowanceForm");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AllowanceList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllowanceNote");

                    b.Property<int>("AllowanceStatus");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("DataToken");

                    b.Property<decimal>("GrantedHoliday14");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<decimal>("OnCallHoliday");

                    b.Property<decimal>("OnCallNormal");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("ServicePackId");

                    b.Property<decimal>("ShiftHoliday");

                    b.Property<decimal>("ShiftNormal");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("ServicePackId");

                    b.ToTable("AllowanceList");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AttendaceExceptionList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountNameId");

                    b.Property<Guid>("ActivityId");

                    b.Property<DateTime>("AddDate");

                    b.Property<int?>("AgencyId");

                    b.Property<DateTime>("ApprovedOneDate");

                    b.Property<DateTime>("ApprovedTwoDate");

                    b.Property<int?>("ApproverOneId");

                    b.Property<int?>("ApproverTwoId");

                    b.Property<int?>("ContractorId");

                    b.Property<Guid>("CostId");

                    b.Property<int>("CreateBy");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("DateEnd");

                    b.Property<DateTime>("DateStart");

                    b.Property<Guid>("DepartmentId");

                    b.Property<Guid>("DepartmentSubId");

                    b.Property<string>("Description");

                    b.Property<string>("Files");

                    b.Property<string>("Hours");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<Guid>("LocationId");

                    b.Property<Guid>("NetworkId");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("ProjectId");

                    b.Property<int>("RequestStatus");

                    b.Property<int>("StatusOne");

                    b.Property<int>("StatusTwo");

                    b.Property<Guid>("SubOpsId");

                    b.Property<Guid>("TimeSheetTypeId");

                    b.Property<string>("Token");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("AccountNameId");

                    b.HasIndex("ActivityId");

                    b.HasIndex("AgencyId");

                    b.HasIndex("ApproverOneId");

                    b.HasIndex("ApproverTwoId");

                    b.HasIndex("ContractorId");

                    b.HasIndex("CostId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DepartmentSubId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("LocationId");

                    b.HasIndex("NetworkId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SubOpsId");

                    b.HasIndex("TimeSheetTypeId");

                    b.ToTable("AttendaceExceptionList");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AttendanceRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("AttendanceDate");

                    b.Property<DateTime?>("AttendanceHour");

                    b.Property<int?>("AttendanceType");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("InsertDate");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("TimeSheetId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("TimeSheetId");

                    b.ToTable("AttendanceRecord");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AutoGenerateVariable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AutoStatus");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<int>("Cycle");

                    b.Property<string>("DayNight")
                        .HasColumnType("char(2)");

                    b.Property<string>("GenerateDate")
                        .HasColumnType("char(2)");

                    b.Property<string>("GenerateHour")
                        .HasColumnType("char(2)");

                    b.Property<string>("GenerateMinute")
                        .HasColumnType("char(2)");

                    b.Property<string>("GenerateSecond")
                        .HasColumnType("char(2)");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("SetBy")
                        .HasColumnType("char(4)");

                    b.Property<DateTime>("SetDate");

                    b.Property<string>("function");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("AutoGenerateVariable");
                });

            modelBuilder.Entity("App.Domain.Models.Core.BackupLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BackupDate");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<int>("Cycle");

                    b.Property<string>("Database");

                    b.Property<string>("Failed");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Succeed");

                    b.Property<string>("Tables");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("BackupLog");
                });

            modelBuilder.Entity("App.Domain.Models.Core.BusinessNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int>("NoteBy");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("BusinessNote");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CandidateInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("Address");

                    b.Property<int?>("AgencyId");

                    b.Property<int>("AgencyType");

                    b.Property<DateTime>("ApproveOneDate");

                    b.Property<string>("ApproveOneNotes");

                    b.Property<int>("ApproveOneStatus");

                    b.Property<string>("ApproveTwoNotes");

                    b.Property<int>("ApproveTwoStatus");

                    b.Property<DateTime>("ApproveTwoeDate");

                    b.Property<string>("Attachments");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Description");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<int>("Gender");

                    b.Property<Guid?>("HomeBaseId");

                    b.Property<string>("HomePhoneNumber");

                    b.Property<string>("IdNumber");

                    b.Property<bool>("IsCandidate");

                    b.Property<bool>("IsContractor");

                    b.Property<bool?>("IsFormerEricsson");

                    b.Property<bool>("IsUser");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int?>("Martial");

                    b.Property<string>("MobilePhoneNumber");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Nationality");

                    b.Property<string>("NickName");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("PlaceOfBirth");

                    b.Property<int?>("RequestById");

                    b.Property<Guid>("VacancyId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AgencyId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("HomeBaseId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("RequestById");

                    b.HasIndex("VacancyId");

                    b.ToTable("CandidateInfo");
                });

            modelBuilder.Entity("App.Domain.Models.Core.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("City");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Claim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ActivityCodeId");

                    b.Property<DateTime>("AddDate");

                    b.Property<int?>("AgencyId");

                    b.Property<int>("AllowanceOption");

                    b.Property<DateTime>("ApprovedDateOne");

                    b.Property<DateTime>("ApprovedDateTwo");

                    b.Property<int>("ApproverOne");

                    b.Property<string>("ApproverOneNotes");

                    b.Property<int>("ApproverTwo");

                    b.Property<string>("ApproverTwoNotes");

                    b.Property<int?>("ClaimApproverOneId");

                    b.Property<int?>("ClaimApproverTwoId");

                    b.Property<Guid?>("ClaimCategoryId");

                    b.Property<DateTime>("ClaimDate");

                    b.Property<int?>("ClaimForId");

                    b.Property<int>("ClaimStatus");

                    b.Property<int>("ClaimType");

                    b.Property<int?>("ContractorId");

                    b.Property<Guid?>("ContractorProfileId");

                    b.Property<Guid?>("CostCenterId");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<int>("DayType");

                    b.Property<Guid?>("DepartureId");

                    b.Property<string>("Description");

                    b.Property<Guid?>("DestinationId");

                    b.Property<decimal>("Domallo1");

                    b.Property<decimal>("Domallo2");

                    b.Property<decimal>("Domallo3");

                    b.Property<decimal>("Domallo4");

                    b.Property<decimal>("Domallo5");

                    b.Property<decimal>("Domallo6");

                    b.Property<int>("EmployeeLevel");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("Files");

                    b.Property<decimal>("Intallo1");

                    b.Property<decimal>("Intallo2");

                    b.Property<decimal>("Intallo3");

                    b.Property<decimal>("Intallo4");

                    b.Property<decimal>("Intallo5");

                    b.Property<decimal>("Intallo6");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<Guid?>("NetworkNumberId");

                    b.Property<decimal>("OnCallShift");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid?>("ProjectId");

                    b.Property<int?>("RedeemForId");

                    b.Property<int>("Schedule");

                    b.Property<DateTime>("StartDate");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("StatusOne");

                    b.Property<int>("StatusTwo");

                    b.Property<int>("TripType");

                    b.Property<decimal>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ActivityCodeId");

                    b.HasIndex("AgencyId");

                    b.HasIndex("ClaimApproverOneId");

                    b.HasIndex("ClaimApproverTwoId");

                    b.HasIndex("ClaimCategoryId");

                    b.HasIndex("ClaimForId");

                    b.HasIndex("ContractorId");

                    b.HasIndex("ContractorProfileId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartureId");

                    b.HasIndex("DestinationId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("NetworkNumberId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RedeemForId");

                    b.ToTable("Claim");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ClaimCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("ClaimCategory");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CostCenter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<Guid>("DepartmentId");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("CostCenter");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CustomerClaim", b =>
                {
                    b.Property<int>("Row")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Fax");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Phone");

                    b.HasKey("Row");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("CustomerClaim");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CustomerData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Add");

                    b.Property<string>("BankName");

                    b.Property<Guid>("BranchId");

                    b.Property<string>("Branding");

                    b.Property<string>("BrokenCommissionTargetName");

                    b.Property<string>("ClaimDocument");

                    b.Property<string>("ClaimProcess");

                    b.Property<string>("CommissionPaymentType");

                    b.Property<decimal>("CommissionTotal");

                    b.Property<decimal>("CommissionValue");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<int>("CustomerType");

                    b.Property<string>("Email");

                    b.Property<string>("Fax");

                    b.Property<bool>("IsAntarJemput");

                    b.Property<bool>("IsCommissioned");

                    b.Property<bool>("IsOwnRisk");

                    b.Property<bool>("IsSparepartSupplied");

                    b.Property<bool>("IsWaitingSpk");

                    b.Property<string>("KPTSBankName");

                    b.Property<string>("KPTSRekNumber");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Mobile");

                    b.Property<string>("MontlyCommissionTargetName");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Phone");

                    b.Property<string>("Picture");

                    b.Property<char>("StatusCustomer");

                    b.Property<int>("Times");

                    b.Property<string>("Token");

                    b.Property<string>("Warranty");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("CustomerData");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Departement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<int>("HeadId");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<int>("OperateOrNon");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("HeadId");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Departement");
                });

            modelBuilder.Entity("App.Domain.Models.Core.DepartementSub", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<Guid>("DepartmentId");

                    b.Property<int>("DsStatus");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int>("LineManagerid");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("SubName");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("LineManagerid");

                    b.ToTable("DepartementSub");
                });

            modelBuilder.Entity("App.Domain.Models.Core.DutySchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<int>("OffDutyHour");

                    b.Property<int>("OffDutyMinute");

                    b.Property<int>("OnDutyHour");

                    b.Property<int>("OnDutyMinute");

                    b.Property<string>("OtherInfo");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("DutySchedule");
                });

            modelBuilder.Entity("App.Domain.Models.Core.EmailArchieve", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Activity");

                    b.Property<string>("Bcc");

                    b.Property<string>("Cc");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("ExceptionSendingMessage");

                    b.Property<string>("From")
                        .IsRequired();

                    b.Property<string>("FromName");

                    b.Property<string>("HtmlMessage");

                    b.Property<bool>("IsRead");

                    b.Property<bool>("IsSent");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastTrySentDate");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("LinkTo");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("PlainMessage");

                    b.Property<string>("ReplyTo");

                    b.Property<string>("ReplyToName");

                    b.Property<DateTime?>("SentDate");

                    b.Property<string>("Status");

                    b.Property<string>("Subject")
                        .IsRequired();

                    b.Property<string>("Tos")
                        .IsRequired();

                    b.Property<int>("TrySentCount");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("EmailArchieve");
                });

            modelBuilder.Entity("App.Domain.Models.Core.FingerPrint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Ip");

                    b.Property<bool>("IsEnabled");

                    b.Property<string>("Key");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("FingerPrint");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Fortest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Fortest");
                });

            modelBuilder.Entity("App.Domain.Models.Core.GenerateLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("By");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("Date");

                    b.Property<DateTime>("GeneratedPeriod");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Ledger");

                    b.Property<string>("OtherInfo");

                    b.Property<DateTime>("PeriodBegin");

                    b.Property<string>("Product");

                    b.Property<string>("Subscriber");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("GenerateLog");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Holidays", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("DateDay");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Holidays");
                });

            modelBuilder.Entity("App.Domain.Models.Core.JobStage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Stage");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("JobStage");
                });

            modelBuilder.Entity("App.Domain.Models.Core.NetworkNumber", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountNameId");

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<Guid>("DepartmentId");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int>("LineManagerId");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("ProjectId");

                    b.Property<int>("ProjectManagerId");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("AccountNameId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("LineManagerId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ProjectManagerId");

                    b.ToTable("NetworkNumber");
                });

            modelBuilder.Entity("App.Domain.Models.Core.PackageType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("PackageType");
                });

            modelBuilder.Entity("App.Domain.Models.Core.PanelCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("PanelCategory");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Position", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Remark");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Position");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Projects", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("App.Domain.Models.Core.RequestSpareParts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Conpensation")
                        .HasColumnType("char(1)");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<bool>("IsSupply");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("PanelCategoryId");

                    b.Property<double>("PartAppinsco");

                    b.Property<double>("PartQty");

                    b.Property<double>("Price");

                    b.Property<Guid>("RepiredId");

                    b.Property<string>("SpareAs")
                        .HasColumnType("char(1)");

                    b.Property<string>("SpareWdl")
                        .HasColumnType("char(1)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("PanelCategoryId");

                    b.ToTable("RequestSpareParts");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ServicePack", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<decimal>("Hourly");

                    b.Property<decimal>("Laptop");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<decimal>("Otp20");

                    b.Property<decimal>("Otp30");

                    b.Property<decimal>("Otp40");

                    b.Property<decimal>("Rate");

                    b.Property<Guid>("ServicePackCategoryId");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.Property<int>("Type");

                    b.Property<decimal>("Usin");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("ServicePackCategoryId");

                    b.ToTable("ServicePack");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ServicePackCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("ServicePackCategory");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SrfEscalationRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApproveStatusFour");

                    b.Property<int>("ApproveStatusOne");

                    b.Property<int>("ApproveStatusThree");

                    b.Property<int>("ApproveStatusTwo");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Files");

                    b.Property<bool>("IsCommnunication");

                    b.Property<bool>("IsWorkstation");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Note");

                    b.Property<int>("OtLevel");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("ServicePackId");

                    b.Property<decimal>("SparateValue");

                    b.Property<Guid>("SrfId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("ServicePackId");

                    b.HasIndex("SrfId");

                    b.ToTable("SrfEscalationRequest");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SrfRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AccountId");

                    b.Property<Guid?>("ActivityId");

                    b.Property<int?>("ApproveFiveId");

                    b.Property<int?>("ApproveFourId");

                    b.Property<int?>("ApproveOneId");

                    b.Property<int?>("ApproveSixId");

                    b.Property<int>("ApproveStatusFive");

                    b.Property<int>("ApproveStatusFour");

                    b.Property<int>("ApproveStatusOne");

                    b.Property<int>("ApproveStatusSix");

                    b.Property<int>("ApproveStatusThree");

                    b.Property<int>("ApproveStatusTwo");

                    b.Property<int?>("ApproveThreeId");

                    b.Property<int?>("ApproveTwoId");

                    b.Property<Guid>("CandidateId");

                    b.Property<Guid>("CostCenterId");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<Guid>("DepartmentId");

                    b.Property<Guid>("DepartmentSubId");

                    b.Property<string>("Description");

                    b.Property<Guid?>("ExtendFrom");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsExtended");

                    b.Property<bool>("IsHrms");

                    b.Property<bool>("IsLocked");

                    b.Property<bool>("IsManager");

                    b.Property<bool>("IsOps");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<int>("LineManagerId");

                    b.Property<Guid>("NetworkId");

                    b.Property<string>("NotesFirst");

                    b.Property<string>("NotesLast");

                    b.Property<string>("Number");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("ProjectManagerId");

                    b.Property<int>("RateType");

                    b.Property<string>("RequestBy");

                    b.Property<int>("ServiceLevel");

                    b.Property<Guid>("ServicePackId");

                    b.Property<decimal>("SpectValue");

                    b.Property<DateTime?>("SrfBegin");

                    b.Property<DateTime?>("SrfEnd");

                    b.Property<int>("Status");

                    b.Property<string>("TeriminateNote");

                    b.Property<string>("TerimnatedBy");

                    b.Property<DateTime>("TerminatedDate");

                    b.Property<int>("Type");

                    b.Property<bool>("isCommunication");

                    b.Property<bool>("isWorkstation");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ApproveFiveId");

                    b.HasIndex("ApproveFourId");

                    b.HasIndex("ApproveOneId");

                    b.HasIndex("ApproveSixId");

                    b.HasIndex("ApproveThreeId");

                    b.HasIndex("ApproveTwoId");

                    b.HasIndex("CandidateId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DepartmentSubId");

                    b.HasIndex("ExtendFrom");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("LineManagerId");

                    b.HasIndex("NetworkId");

                    b.HasIndex("ProjectManagerId");

                    b.HasIndex("ServicePackId");

                    b.ToTable("SrfRequest");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Subdivision", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Remark");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Subdivision");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SubOps", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<bool>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("SubOps");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SystemBranch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("BranchCode");

                    b.Property<bool>("BranchStatus");

                    b.Property<string>("CabangCode");

                    b.Property<string>("CabangToken");

                    b.Property<string>("City");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Criteria");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Email");

                    b.Property<string>("Fax");

                    b.Property<string>("Guaranty");

                    b.Property<string>("GuarantyNota");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("NotaCode");

                    b.Property<string>("NotaNonSercvice");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("PhoneOne");

                    b.Property<string>("PhoneTwo");

                    b.Property<string>("PkbRemark");

                    b.Property<string>("PoCode");

                    b.Property<string>("Remark");

                    b.Property<string>("UnitInCode");

                    b.Property<bool>("isHeadOffice");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("SystemBranch");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SystemPropertiesRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("PropertyName");

                    b.Property<string>("PropertyValue");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("SystemPropertiesRecord");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Detail");

                    b.Property<bool>("IsArchive");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<int>("Status");

                    b.Property<DateTime>("TicketDate");

                    b.Property<string>("Title");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TicketInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClaimId");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("Files");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Note");

                    b.Property<string>("OtherInfo");

                    b.Property<double>("Price");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("ClaimId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("TicketInfo");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TicketReply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<DateTime>("ReplyDate");

                    b.Property<Guid>("TicketId");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketReply");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TimeSheetPeriod", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("DateActual");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("TimeSheetType");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("TimeSheetPeriod");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TimeSheetType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("Token");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("TimeSheetType");
                });

            modelBuilder.Entity("App.Domain.Models.Core.VacancyList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountNameId");

                    b.Property<int?>("ApproverFourId");

                    b.Property<int?>("ApproverOneId");

                    b.Property<int?>("ApproverThreeId");

                    b.Property<int?>("ApproverTwoId");

                    b.Property<Guid>("CostCodeId");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<DateTime>("DateApprovedFour");

                    b.Property<DateTime>("DateApprovedOne");

                    b.Property<DateTime>("DateApprovedThree");

                    b.Property<DateTime>("DateApprovedTwo");

                    b.Property<Guid>("DepartmentId");

                    b.Property<Guid>("DepartmentSubId");

                    b.Property<string>("Description");

                    b.Property<string>("Files");

                    b.Property<Guid>("JobStageId");

                    b.Property<DateTime>("JoinDate");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<Guid>("NetworkId");

                    b.Property<decimal>("NoarmalRate");

                    b.Property<int>("OtLevel");

                    b.Property<string>("OtherInfo");

                    b.Property<Guid>("PackageTypeId");

                    b.Property<int?>("RequestById");

                    b.Property<Guid>("ServicePackCategoryId");

                    b.Property<Guid>("ServicePackId");

                    b.Property<int>("Status");

                    b.Property<int>("StatusFourth");

                    b.Property<int>("StatusOne");

                    b.Property<int>("StatusThree");

                    b.Property<int>("StatusTwo");

                    b.Property<int>("VacancyStatus");

                    b.Property<bool>("isHrms");

                    b.Property<bool>("isLaptop");

                    b.Property<bool>("isManager");

                    b.Property<bool>("isUsim");

                    b.HasKey("Id");

                    b.HasIndex("AccountNameId");

                    b.HasIndex("ApproverFourId");

                    b.HasIndex("ApproverOneId");

                    b.HasIndex("ApproverThreeId");

                    b.HasIndex("ApproverTwoId");

                    b.HasIndex("CostCodeId");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DepartmentSubId");

                    b.HasIndex("JobStageId");

                    b.HasIndex("LastEditedBy");

                    b.HasIndex("NetworkId");

                    b.HasIndex("PackageTypeId");

                    b.HasIndex("RequestById");

                    b.HasIndex("ServicePackCategoryId");

                    b.HasIndex("ServicePackId");

                    b.ToTable("VacancyList");
                });

            modelBuilder.Entity("App.Domain.Models.Core.WebSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OtherInfo");

                    b.Property<bool>("SystemSetting");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("WebSetting");
                });

            modelBuilder.Entity("App.Domain.Models.Core.WoItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<double>("Disc");

                    b.Property<string>("ItemId");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("PartCode");

                    b.Property<double>("Price");

                    b.Property<int>("Qty");

                    b.Property<int>("Type");

                    b.Property<string>("WoNumber");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("WoItem");
                });

            modelBuilder.Entity("App.Domain.Models.Core.WotList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddDate");

                    b.Property<bool>("ApproveOne");

                    b.Property<bool>("ApproveTwo");

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndTime");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("Status");

                    b.Property<int>("StatusOne");

                    b.Property<int>("StatusTwo");

                    b.Property<string>("Token");

                    b.Property<DateTime>("WotDate");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("WotList");
                });

            modelBuilder.Entity("App.Domain.Models.Identity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("App.Domain.Models.Identity.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Address");

                    b.Property<string>("AhId");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnName("UserId");

                    b.Property<DateTime?>("Birthdate");

                    b.Property<string>("Birthplace");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<int?>("Gender");

                    b.Property<string>("HomePhoneNumber");

                    b.Property<string>("IdNumber")
                        .HasMaxLength(16);

                    b.Property<bool?>("IsActive");

                    b.Property<bool?>("IsBlacklist");

                    b.Property<bool?>("IsTerminate");

                    b.Property<string>("MobilePhoneNumber");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("Photo");

                    b.Property<string>("Roles");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId")
                        .IsUnique();

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("App.Domain.Models.Localization.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<bool>("DefaultLanguage");

                    b.Property<string>("Flag")
                        .IsRequired();

                    b.Property<string>("LanguageCulture")
                        .IsRequired();

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("UniqueSeoCode")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("Language");
                });

            modelBuilder.Entity("App.Domain.Models.Localization.LocaleResource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CustomField1");

                    b.Property<string>("CustomField2");

                    b.Property<string>("CustomField3");

                    b.Property<int>("LanguageId");

                    b.Property<string>("LastEditedBy");

                    b.Property<DateTime?>("LastUpdateTime");

                    b.Property<string>("OtherInfo");

                    b.Property<string>("ResourceName")
                        .IsRequired();

                    b.Property<string>("ResourceValue")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LastEditedBy");

                    b.ToTable("LocaleResource");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("App.Domain.Models.Identity.ApplicationRole", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole");

                    b.Property<string>("Description");

                    b.Property<string>("OtherInfo");

                    b.ToTable("ApplicationRole");

                    b.HasDiscriminator().HasValue("ApplicationRole");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AccountName", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Coms")
                        .WithMany()
                        .HasForeignKey("Com")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ActivityCode", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AllowanceForm", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.AllowanceList", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.ServicePack", "ServicePack")
                        .WithMany()
                        .HasForeignKey("ServicePackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.AttendaceExceptionList", b =>
                {
                    b.HasOne("App.Domain.Models.Core.AccountName", "AccountName")
                        .WithMany()
                        .HasForeignKey("AccountNameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.ActivityCode", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Agency")
                        .WithMany()
                        .HasForeignKey("AgencyId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverOne")
                        .WithMany()
                        .HasForeignKey("ApproverOneId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverTwo")
                        .WithMany()
                        .HasForeignKey("ApproverTwoId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Contractor")
                        .WithMany()
                        .HasForeignKey("ContractorId");

                    b.HasOne("App.Domain.Models.Core.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.DepartementSub", "DepartementSub")
                        .WithMany()
                        .HasForeignKey("DepartmentSubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.City", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.NetworkNumber", "Network")
                        .WithMany()
                        .HasForeignKey("NetworkId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.Projects", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.SubOps", "SubOps")
                        .WithMany()
                        .HasForeignKey("SubOpsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.TimeSheetType", "TimeSheetType")
                        .WithMany()
                        .HasForeignKey("TimeSheetTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.AttendanceRecord", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.TimeSheetPeriod", "TimeSheet")
                        .WithMany()
                        .HasForeignKey("TimeSheetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.AutoGenerateVariable", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.BackupLog", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.BusinessNote", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CandidateInfo", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Agency")
                        .WithMany()
                        .HasForeignKey("AgencyId");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.City", "HomeBase")
                        .WithMany()
                        .HasForeignKey("HomeBaseId");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "RequestBy")
                        .WithMany()
                        .HasForeignKey("RequestById");

                    b.HasOne("App.Domain.Models.Core.VacancyList", "Vacancy")
                        .WithMany("Candidate")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.City", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Claim", b =>
                {
                    b.HasOne("App.Domain.Models.Core.ActivityCode", "ActivityCode")
                        .WithMany()
                        .HasForeignKey("ActivityCodeId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Agency")
                        .WithMany()
                        .HasForeignKey("AgencyId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ClaimApproverOne")
                        .WithMany()
                        .HasForeignKey("ClaimApproverOneId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ClaimApproverTwo")
                        .WithMany()
                        .HasForeignKey("ClaimApproverTwoId");

                    b.HasOne("App.Domain.Models.Core.ClaimCategory", "ClaimCategory")
                        .WithMany()
                        .HasForeignKey("ClaimCategoryId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ClaimFor")
                        .WithMany()
                        .HasForeignKey("ClaimForId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Contractor")
                        .WithMany()
                        .HasForeignKey("ContractorId");

                    b.HasOne("App.Domain.Models.Core.CandidateInfo", "ContractorProfile")
                        .WithMany()
                        .HasForeignKey("ContractorProfileId");

                    b.HasOne("App.Domain.Models.Core.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.City", "Departure")
                        .WithMany()
                        .HasForeignKey("DepartureId");

                    b.HasOne("App.Domain.Models.Core.City", "Destination")
                        .WithMany()
                        .HasForeignKey("DestinationId");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.NetworkNumber", "NetworkNumber")
                        .WithMany()
                        .HasForeignKey("NetworkNumberId");

                    b.HasOne("App.Domain.Models.Core.Projects", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "RedeemFor")
                        .WithMany()
                        .HasForeignKey("RedeemForId");
                });

            modelBuilder.Entity("App.Domain.Models.Core.ClaimCategory", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CostCenter", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CustomerClaim", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.CustomerData", b =>
                {
                    b.HasOne("App.Domain.Models.Core.SystemBranch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Departement", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "Head")
                        .WithMany()
                        .HasForeignKey("HeadId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.DepartementSub", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "LineManager")
                        .WithMany()
                        .HasForeignKey("LineManagerid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.DutySchedule", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.EmailArchieve", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.FingerPrint", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Fortest", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.GenerateLog", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Holidays", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.JobStage", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.NetworkNumber", b =>
                {
                    b.HasOne("App.Domain.Models.Core.AccountName", "AccountName")
                        .WithMany()
                        .HasForeignKey("AccountNameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "LineManager")
                        .WithMany()
                        .HasForeignKey("LineManagerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.Projects", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ProjectManager")
                        .WithMany()
                        .HasForeignKey("ProjectManagerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.PackageType", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.PanelCategory", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Position", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Projects", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.RequestSpareParts", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.PanelCategory", "PanelCategory")
                        .WithMany()
                        .HasForeignKey("PanelCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.ServicePack", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.ServicePackCategory", "ServicePackCategory")
                        .WithMany()
                        .HasForeignKey("ServicePackCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.ServicePackCategory", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SrfEscalationRequest", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.ServicePack", "ServicePack")
                        .WithMany()
                        .HasForeignKey("ServicePackId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.SrfRequest", "SrfRequest")
                        .WithMany("Escalation")
                        .HasForeignKey("SrfId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.SrfRequest", b =>
                {
                    b.HasOne("App.Domain.Models.Core.AccountName", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("App.Domain.Models.Core.ActivityCode", "ActivityCode")
                        .WithMany()
                        .HasForeignKey("ActivityId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveFiveBy")
                        .WithMany()
                        .HasForeignKey("ApproveFiveId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveFourBy")
                        .WithMany()
                        .HasForeignKey("ApproveFourId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveOneBy")
                        .WithMany()
                        .HasForeignKey("ApproveOneId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveSixBy")
                        .WithMany()
                        .HasForeignKey("ApproveSixId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveThreeBy")
                        .WithMany()
                        .HasForeignKey("ApproveThreeId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproveTwoBy")
                        .WithMany()
                        .HasForeignKey("ApproveTwoId");

                    b.HasOne("App.Domain.Models.Core.CandidateInfo", "Candidate")
                        .WithMany()
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.DepartementSub", "DepartementSub")
                        .WithMany()
                        .HasForeignKey("DepartmentSubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.SrfRequest", "Extend")
                        .WithMany()
                        .HasForeignKey("ExtendFrom");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "LineManager")
                        .WithMany()
                        .HasForeignKey("LineManagerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.NetworkNumber", "NetworkNumber")
                        .WithMany()
                        .HasForeignKey("NetworkId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ProjectManager")
                        .WithMany()
                        .HasForeignKey("ProjectManagerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.ServicePack", "ServicePack")
                        .WithMany()
                        .HasForeignKey("ServicePackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.Subdivision", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SubOps", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SystemBranch", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.SystemPropertiesRecord", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.Ticket", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TicketInfo", b =>
                {
                    b.HasOne("App.Domain.Models.Core.Claim", "Claim")
                        .WithMany("ListTicket")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TicketReply", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.Ticket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.TimeSheetPeriod", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.TimeSheetType", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.VacancyList", b =>
                {
                    b.HasOne("App.Domain.Models.Core.AccountName", "AccountName")
                        .WithMany()
                        .HasForeignKey("AccountNameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverFour")
                        .WithMany()
                        .HasForeignKey("ApproverFourId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverOne")
                        .WithMany()
                        .HasForeignKey("ApproverOneId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverThree")
                        .WithMany()
                        .HasForeignKey("ApproverThreeId");

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "ApproverTwo")
                        .WithMany()
                        .HasForeignKey("ApproverTwoId");

                    b.HasOne("App.Domain.Models.Core.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Core.Departement", "Departement")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.DepartementSub", "DepartementSub")
                        .WithMany()
                        .HasForeignKey("DepartmentSubId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.JobStage", "JobStage")
                        .WithMany()
                        .HasForeignKey("JobStageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");

                    b.HasOne("App.Domain.Models.Core.NetworkNumber", "Network")
                        .WithMany()
                        .HasForeignKey("NetworkId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.PackageType", "PackageType")
                        .WithMany()
                        .HasForeignKey("PackageTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.UserProfile", "RequestBy")
                        .WithMany()
                        .HasForeignKey("RequestById");

                    b.HasOne("App.Domain.Models.Core.ServicePackCategory", "ServicePackCategory")
                        .WithMany()
                        .HasForeignKey("ServicePackCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Core.ServicePack", "ServicePack")
                        .WithMany()
                        .HasForeignKey("ServicePackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("App.Domain.Models.Core.WebSetting", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.WoItem", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Core.WotList", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Identity.UserProfile", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "ApplicationUser")
                        .WithOne("UserProfile")
                        .HasForeignKey("App.Domain.Models.Identity.UserProfile", "ApplicationUserId");
                });

            modelBuilder.Entity("App.Domain.Models.Localization.Language", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("App.Domain.Models.Localization.LocaleResource", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.HasOne("App.Domain.Models.Localization.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser", "Editor")
                        .WithMany()
                        .HasForeignKey("LastEditedBy");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("App.Domain.Models.Identity.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("App.Domain.Models.Identity.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
