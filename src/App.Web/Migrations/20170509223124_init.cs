using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace App.Web.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "ActivityCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityCode_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityCode_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllowanceForm_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllowanceForm_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AutoGenerateVariable",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AutoStatus = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Cycle = table.Column<int>(nullable: false),
                    DayNight = table.Column<string>(type: "char(2)", nullable: true),
                    GenerateDate = table.Column<string>(type: "char(2)", nullable: true),
                    GenerateHour = table.Column<string>(type: "char(2)", nullable: true),
                    GenerateMinute = table.Column<string>(type: "char(2)", nullable: true),
                    GenerateSecond = table.Column<string>(type: "char(2)", nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    SetBy = table.Column<string>(type: "char(4)", nullable: true),
                    SetDate = table.Column<DateTime>(nullable: false),
                    function = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoGenerateVariable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoGenerateVariable_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoGenerateVariable_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BackupLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BackupDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Cycle = table.Column<int>(nullable: false),
                    Database = table.Column<string>(nullable: true),
                    Failed = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Succeed = table.Column<string>(nullable: true),
                    Tables = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackupLog_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackupLog_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusinessNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    NoteBy = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessNote_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusinessNote_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_City_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimCategory_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimCategory_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerClaim",
                columns: table => new
                {
                    Row = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Address = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerClaim", x => x.Row);
                    table.ForeignKey(
                        name: "FK_CustomerClaim_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerClaim_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DutySchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OffDutyHour = table.Column<int>(nullable: false),
                    OffDutyMinute = table.Column<int>(nullable: false),
                    OnDutyHour = table.Column<int>(nullable: false),
                    OnDutyMinute = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutySchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DutySchedule_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DutySchedule_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailArchieve",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Activity = table.Column<string>(nullable: true),
                    Bcc = table.Column<string>(nullable: true),
                    Cc = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    ExceptionSendingMessage = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: false),
                    FromName = table.Column<string>(nullable: true),
                    HtmlMessage = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false),
                    IsSent = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastTrySentDate = table.Column<DateTime>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LinkTo = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PlainMessage = table.Column<string>(nullable: true),
                    ReplyTo = table.Column<string>(nullable: true),
                    ReplyToName = table.Column<string>(nullable: true),
                    SentDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: false),
                    Tos = table.Column<string>(nullable: false),
                    TrySentCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailArchieve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailArchieve_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailArchieve_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FingerPrint",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FingerPrint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FingerPrint_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FingerPrint_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fortest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fortest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fortest_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fortest_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GenerateLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    By = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    GeneratedPeriod = table.Column<DateTime>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Ledger = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PeriodBegin = table.Column<DateTime>(nullable: false),
                    Product = table.Column<string>(nullable: true),
                    Subscriber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerateLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerateLog_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenerateLog_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobStage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Stage = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobStage_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobStage_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageType_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageType_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PanelCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PanelCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PanelCategory_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PanelCategory_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Position_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicePackCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePackCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicePackCategory_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePackCategory_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subdivision",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subdivision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subdivision_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subdivision_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubOps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubOps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubOps_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubOps_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemBranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    BranchCode = table.Column<string>(nullable: true),
                    BranchStatus = table.Column<bool>(nullable: false),
                    CabangCode = table.Column<string>(nullable: true),
                    CabangToken = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Criteria = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    Guaranty = table.Column<string>(nullable: true),
                    GuarantyNota = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    NotaCode = table.Column<string>(nullable: true),
                    NotaNonSercvice = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PhoneOne = table.Column<string>(nullable: true),
                    PhoneTwo = table.Column<string>(nullable: true),
                    PkbRemark = table.Column<string>(nullable: true),
                    PoCode = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    UnitInCode = table.Column<string>(nullable: true),
                    isHeadOffice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemBranch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemBranch_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemBranch_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemPropertiesRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPropertiesRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemPropertiesRecord_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemPropertiesRecord_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    IsArchive = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TicketDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ticket_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ticket_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSheetPeriod",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateActual = table.Column<DateTime>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    TimeSheetType = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheetPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSheetPeriod_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeSheetPeriod_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSheetType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSheetType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSheetType_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeSheetType_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WebSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    SystemSetting = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebSetting_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WebSetting_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Disc = table.Column<double>(nullable: false),
                    ItemId = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PartCode = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    WoNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WoItem_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WoItem_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WotList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddDate = table.Column<DateTime>(nullable: false),
                    ApproveOne = table.Column<bool>(nullable: false),
                    ApproveTwo = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusOne = table.Column<int>(nullable: false),
                    StatusTwo = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    WotDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WotList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WotList_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WotList_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Address = table.Column<string>(nullable: true),
                    AhId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Birthdate = table.Column<DateTime>(nullable: true),
                    Birthplace = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: true),
                    HomePhoneNumber = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(maxLength: 16, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    IsBlacklist = table.Column<bool>(nullable: true),
                    IsTerminate = table.Column<bool>(nullable: true),
                    MobilePhoneNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    Roles = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DefaultLanguage = table.Column<bool>(nullable: false),
                    Flag = table.Column<string>(nullable: false),
                    LanguageCulture = table.Column<string>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    UniqueSeoCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Language_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Language_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestSpareParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Conpensation = table.Column<string>(type: "char(1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    IsSupply = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PanelCategoryId = table.Column<Guid>(nullable: false),
                    PartAppinsco = table.Column<double>(nullable: false),
                    PartQty = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    RepiredId = table.Column<Guid>(nullable: false),
                    SpareAs = table.Column<string>(type: "char(1)", nullable: true),
                    SpareWdl = table.Column<string>(type: "char(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSpareParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestSpareParts_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestSpareParts_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestSpareParts_PanelCategory_PanelCategoryId",
                        column: x => x.PanelCategoryId,
                        principalTable: "PanelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicePack",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Hourly = table.Column<decimal>(nullable: false),
                    Laptop = table.Column<decimal>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Otp20 = table.Column<decimal>(nullable: false),
                    Otp30 = table.Column<decimal>(nullable: false),
                    Otp40 = table.Column<decimal>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    ServicePackCategoryId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Usin = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicePack_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePack_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePack_ServicePackCategory_ServicePackCategoryId",
                        column: x => x.ServicePackCategoryId,
                        principalTable: "ServicePackCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Add = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    BranchId = table.Column<Guid>(nullable: false),
                    Branding = table.Column<string>(nullable: true),
                    BrokenCommissionTargetName = table.Column<string>(nullable: true),
                    ClaimDocument = table.Column<string>(nullable: true),
                    ClaimProcess = table.Column<string>(nullable: true),
                    CommissionPaymentType = table.Column<string>(nullable: true),
                    CommissionTotal = table.Column<decimal>(nullable: false),
                    CommissionValue = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    CustomerType = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Fax = table.Column<string>(nullable: true),
                    IsAntarJemput = table.Column<bool>(nullable: false),
                    IsCommissioned = table.Column<bool>(nullable: false),
                    IsOwnRisk = table.Column<bool>(nullable: false),
                    IsSparepartSupplied = table.Column<bool>(nullable: false),
                    IsWaitingSpk = table.Column<bool>(nullable: false),
                    KPTSBankName = table.Column<string>(nullable: true),
                    KPTSRekNumber = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    MontlyCommissionTargetName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Picture = table.Column<string>(nullable: true),
                    StatusCustomer = table.Column<char>(nullable: false),
                    Times = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Warranty = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerData_SystemBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "SystemBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerData_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerData_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketReply",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    ReplyDate = table.Column<DateTime>(nullable: false),
                    TicketId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketReply_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketReply_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketReply_Ticket_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(nullable: true),
                    AttendanceHour = table.Column<DateTime>(nullable: true),
                    AttendanceType = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    InsertDate = table.Column<DateTime>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    TimeSheetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_TimeSheetPeriod_TimeSheetId",
                        column: x => x.TimeSheetId,
                        principalTable: "TimeSheetPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountName",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Com = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountName_UserProfile_Com",
                        column: x => x.Com,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountName_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountName_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departement",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    HeadId = table.Column<int>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OperateOrNon = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departement_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departement_UserProfile_HeadId",
                        column: x => x.HeadId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Departement_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocaleResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    LanguageId = table.Column<int>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    ResourceName = table.Column<string>(nullable: false),
                    ResourceValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaleResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocaleResource_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocaleResource_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocaleResource_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AllowanceNote = table.Column<string>(nullable: true),
                    AllowanceStatus = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DataToken = table.Column<string>(nullable: true),
                    GrantedHoliday14 = table.Column<decimal>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OnCallHoliday = table.Column<decimal>(nullable: false),
                    OnCallNormal = table.Column<decimal>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    ServicePackId = table.Column<Guid>(nullable: false),
                    ShiftHoliday = table.Column<decimal>(nullable: false),
                    ShiftNormal = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowanceList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllowanceList_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllowanceList_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllowanceList_ServicePack_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostCenter_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenter_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostCenter_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartementSub",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    DsStatus = table.Column<int>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LineManagerid = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    SubName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartementSub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartementSub_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartementSub_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartementSub_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartementSub_UserProfile_LineManagerid",
                        column: x => x.LineManagerid,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkNumber",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountNameId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LineManagerId = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ProjectManagerId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_AccountName_AccountNameId",
                        column: x => x.AccountNameId,
                        principalTable: "AccountName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_UserProfile_LineManagerId",
                        column: x => x.LineManagerId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NetworkNumber_UserProfile_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendaceExceptionList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountNameId = table.Column<Guid>(nullable: false),
                    ActivityId = table.Column<Guid>(nullable: false),
                    AddDate = table.Column<DateTime>(nullable: false),
                    AgencyId = table.Column<int>(nullable: true),
                    ApprovedOneDate = table.Column<DateTime>(nullable: false),
                    ApprovedTwoDate = table.Column<DateTime>(nullable: false),
                    ApproverOneId = table.Column<int>(nullable: true),
                    ApproverTwoId = table.Column<int>(nullable: true),
                    ContractorId = table.Column<int>(nullable: true),
                    CostId = table.Column<Guid>(nullable: false),
                    CreateBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    DepartmentSubId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true),
                    Hours = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false),
                    NetworkId = table.Column<Guid>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: false),
                    RequestStatus = table.Column<int>(nullable: false),
                    StatusOne = table.Column<int>(nullable: false),
                    StatusTwo = table.Column<int>(nullable: false),
                    SubOpsId = table.Column<Guid>(nullable: false),
                    TimeSheetTypeId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendaceExceptionList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_AccountName_AccountNameId",
                        column: x => x.AccountNameId,
                        principalTable: "AccountName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_ActivityCode_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "ActivityCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_UserProfile_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_UserProfile_ApproverOneId",
                        column: x => x.ApproverOneId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_UserProfile_ApproverTwoId",
                        column: x => x.ApproverTwoId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_UserProfile_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_CostCenter_CostId",
                        column: x => x.CostId,
                        principalTable: "CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_DepartementSub_DepartmentSubId",
                        column: x => x.DepartmentSubId,
                        principalTable: "DepartementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_City_LocationId",
                        column: x => x.LocationId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_NetworkNumber_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "NetworkNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_SubOps_SubOpsId",
                        column: x => x.SubOpsId,
                        principalTable: "SubOps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendaceExceptionList_TimeSheetType_TimeSheetTypeId",
                        column: x => x.TimeSheetTypeId,
                        principalTable: "TimeSheetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountNameId = table.Column<Guid>(nullable: false),
                    ApproverFourId = table.Column<int>(nullable: true),
                    ApproverOneId = table.Column<int>(nullable: true),
                    ApproverThreeId = table.Column<int>(nullable: true),
                    ApproverTwoId = table.Column<int>(nullable: true),
                    CostCodeId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateApprovedFour = table.Column<DateTime>(nullable: false),
                    DateApprovedOne = table.Column<DateTime>(nullable: false),
                    DateApprovedThree = table.Column<DateTime>(nullable: false),
                    DateApprovedTwo = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    DepartmentSubId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true),
                    JobStageId = table.Column<Guid>(nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    NetworkId = table.Column<Guid>(nullable: false),
                    NoarmalRate = table.Column<decimal>(nullable: false),
                    OtLevel = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    PackageTypeId = table.Column<Guid>(nullable: false),
                    RequestById = table.Column<int>(nullable: true),
                    ServicePackCategoryId = table.Column<Guid>(nullable: false),
                    ServicePackId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusFourth = table.Column<int>(nullable: false),
                    StatusOne = table.Column<int>(nullable: false),
                    StatusThree = table.Column<int>(nullable: false),
                    StatusTwo = table.Column<int>(nullable: false),
                    VacancyStatus = table.Column<int>(nullable: false),
                    isHrms = table.Column<bool>(nullable: false),
                    isLaptop = table.Column<bool>(nullable: false),
                    isManager = table.Column<bool>(nullable: false),
                    isUsim = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyList_AccountName_AccountNameId",
                        column: x => x.AccountNameId,
                        principalTable: "AccountName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_UserProfile_ApproverFourId",
                        column: x => x.ApproverFourId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_UserProfile_ApproverOneId",
                        column: x => x.ApproverOneId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_UserProfile_ApproverThreeId",
                        column: x => x.ApproverThreeId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_UserProfile_ApproverTwoId",
                        column: x => x.ApproverTwoId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_CostCenter_CostCodeId",
                        column: x => x.CostCodeId,
                        principalTable: "CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_DepartementSub_DepartmentSubId",
                        column: x => x.DepartmentSubId,
                        principalTable: "DepartementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_JobStage_JobStageId",
                        column: x => x.JobStageId,
                        principalTable: "JobStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_NetworkNumber_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "NetworkNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_PackageType_PackageTypeId",
                        column: x => x.PackageTypeId,
                        principalTable: "PackageType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_UserProfile_RequestById",
                        column: x => x.RequestById,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VacancyList_ServicePackCategory_ServicePackCategoryId",
                        column: x => x.ServicePackCategoryId,
                        principalTable: "ServicePackCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyList_ServicePack_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<int>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AgencyId = table.Column<int>(nullable: true),
                    AgencyType = table.Column<int>(nullable: false),
                    ApproveOneDate = table.Column<DateTime>(nullable: false),
                    ApproveOneNotes = table.Column<string>(nullable: true),
                    ApproveOneStatus = table.Column<int>(nullable: false),
                    ApproveTwoNotes = table.Column<string>(nullable: true),
                    ApproveTwoStatus = table.Column<int>(nullable: false),
                    ApproveTwoeDate = table.Column<DateTime>(nullable: false),
                    Attachments = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    HomeBaseId = table.Column<Guid>(nullable: true),
                    HomePhoneNumber = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    IsCandidate = table.Column<bool>(nullable: false),
                    IsContractor = table.Column<bool>(nullable: false),
                    IsFormerEricsson = table.Column<bool>(nullable: true),
                    IsUser = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Martial = table.Column<int>(nullable: true),
                    MobilePhoneNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Nationality = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    PlaceOfBirth = table.Column<string>(nullable: true),
                    RequestById = table.Column<int>(nullable: true),
                    VacancyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_UserProfile_AccountId",
                        column: x => x.AccountId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_UserProfile_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_City_HomeBaseId",
                        column: x => x.HomeBaseId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_UserProfile_RequestById",
                        column: x => x.RequestById,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateInfo_VacancyList_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "VacancyList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ActivityCodeId = table.Column<Guid>(nullable: true),
                    AddDate = table.Column<DateTime>(nullable: false),
                    AgencyId = table.Column<int>(nullable: true),
                    AllowanceOption = table.Column<int>(nullable: false),
                    ApprovedDateOne = table.Column<DateTime>(nullable: false),
                    ApprovedDateTwo = table.Column<DateTime>(nullable: false),
                    ApproverOne = table.Column<int>(nullable: false),
                    ApproverOneNotes = table.Column<string>(nullable: true),
                    ApproverTwo = table.Column<int>(nullable: false),
                    ApproverTwoNotes = table.Column<string>(nullable: true),
                    ClaimApproverOneId = table.Column<int>(nullable: true),
                    ClaimApproverTwoId = table.Column<int>(nullable: true),
                    ClaimCategoryId = table.Column<Guid>(nullable: true),
                    ClaimDate = table.Column<DateTime>(nullable: false),
                    ClaimForId = table.Column<int>(nullable: true),
                    ClaimStatus = table.Column<int>(nullable: false),
                    ClaimType = table.Column<int>(nullable: false),
                    ContractorId = table.Column<int>(nullable: true),
                    ContractorProfileId = table.Column<Guid>(nullable: true),
                    CostCenterId = table.Column<Guid>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DayType = table.Column<int>(nullable: false),
                    DepartureId = table.Column<Guid>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DestinationId = table.Column<Guid>(nullable: true),
                    Domallo1 = table.Column<decimal>(nullable: false),
                    Domallo2 = table.Column<decimal>(nullable: false),
                    Domallo3 = table.Column<decimal>(nullable: false),
                    Domallo4 = table.Column<decimal>(nullable: false),
                    Domallo5 = table.Column<decimal>(nullable: false),
                    Domallo6 = table.Column<decimal>(nullable: false),
                    EmployeeLevel = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Files = table.Column<string>(nullable: true),
                    Intallo1 = table.Column<decimal>(nullable: false),
                    Intallo2 = table.Column<decimal>(nullable: false),
                    Intallo3 = table.Column<decimal>(nullable: false),
                    Intallo4 = table.Column<decimal>(nullable: false),
                    Intallo5 = table.Column<decimal>(nullable: false),
                    Intallo6 = table.Column<decimal>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    NetworkNumberId = table.Column<Guid>(nullable: true),
                    OnCallShift = table.Column<decimal>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    ProjectId = table.Column<Guid>(nullable: true),
                    RedeemForId = table.Column<int>(nullable: true),
                    Schedule = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    StatusOne = table.Column<int>(nullable: false),
                    StatusTwo = table.Column<int>(nullable: false),
                    TripType = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claim_ActivityCode_ActivityCodeId",
                        column: x => x.ActivityCodeId,
                        principalTable: "ActivityCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_ClaimApproverOneId",
                        column: x => x.ClaimApproverOneId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_ClaimApproverTwoId",
                        column: x => x.ClaimApproverTwoId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_ClaimCategory_ClaimCategoryId",
                        column: x => x.ClaimCategoryId,
                        principalTable: "ClaimCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_ClaimForId",
                        column: x => x.ClaimForId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_CandidateInfo_ContractorProfileId",
                        column: x => x.ContractorProfileId,
                        principalTable: "CandidateInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_City_DepartureId",
                        column: x => x.DepartureId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_City_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_NetworkNumber_NetworkNumberId",
                        column: x => x.NetworkNumberId,
                        principalTable: "NetworkNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claim_UserProfile_RedeemForId",
                        column: x => x.RedeemForId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SrfRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    ActivityId = table.Column<Guid>(nullable: true),
                    ApproveFiveId = table.Column<int>(nullable: true),
                    ApproveFourId = table.Column<int>(nullable: true),
                    ApproveOneId = table.Column<int>(nullable: true),
                    ApproveSixId = table.Column<int>(nullable: true),
                    ApproveStatusFive = table.Column<int>(nullable: false),
                    ApproveStatusFour = table.Column<int>(nullable: false),
                    ApproveStatusOne = table.Column<int>(nullable: false),
                    ApproveStatusSix = table.Column<int>(nullable: false),
                    ApproveStatusThree = table.Column<int>(nullable: false),
                    ApproveStatusTwo = table.Column<int>(nullable: false),
                    ApproveThreeId = table.Column<int>(nullable: true),
                    ApproveTwoId = table.Column<int>(nullable: true),
                    CandidateId = table.Column<Guid>(nullable: false),
                    CostCenterId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: false),
                    DepartmentSubId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExtendFrom = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsExtended = table.Column<bool>(nullable: false),
                    IsHrms = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    IsManager = table.Column<bool>(nullable: false),
                    IsOps = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    LineManagerId = table.Column<int>(nullable: false),
                    NetworkId = table.Column<Guid>(nullable: false),
                    NotesFirst = table.Column<string>(nullable: true),
                    NotesLast = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    ProjectManagerId = table.Column<int>(nullable: false),
                    RateType = table.Column<int>(nullable: false),
                    RequestBy = table.Column<string>(nullable: true),
                    ServiceLevel = table.Column<int>(nullable: false),
                    ServicePackId = table.Column<Guid>(nullable: false),
                    SpectValue = table.Column<decimal>(nullable: false),
                    SrfBegin = table.Column<DateTime>(nullable: true),
                    SrfEnd = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TeriminateNote = table.Column<string>(nullable: true),
                    TerimnatedBy = table.Column<string>(nullable: true),
                    TerminatedDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    isCommunication = table.Column<bool>(nullable: false),
                    isWorkstation = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrfRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SrfRequest_AccountName_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_ActivityCode_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "ActivityCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveFiveId",
                        column: x => x.ApproveFiveId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveFourId",
                        column: x => x.ApproveFourId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveOneId",
                        column: x => x.ApproveOneId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveSixId",
                        column: x => x.ApproveSixId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveThreeId",
                        column: x => x.ApproveThreeId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ApproveTwoId",
                        column: x => x.ApproveTwoId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_CandidateInfo_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "CandidateInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_DepartementSub_DepartmentSubId",
                        column: x => x.DepartmentSubId,
                        principalTable: "DepartementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_SrfRequest_ExtendFrom",
                        column: x => x.ExtendFrom,
                        principalTable: "SrfRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_LineManagerId",
                        column: x => x.LineManagerId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_NetworkNumber_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "NetworkNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_UserProfile_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfRequest_ServicePack_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClaimId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketInfo_Claim_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claim",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketInfo_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketInfo_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SrfEscalationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ApproveStatusOne = table.Column<int>(nullable: false),
                    ApproveStatusThree = table.Column<int>(nullable: false),
                    ApproveStatusTwo = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    Files = table.Column<string>(nullable: true),
                    IsCommnunication = table.Column<bool>(nullable: false),
                    IsWorkstation = table.Column<bool>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    OtLevel = table.Column<int>(nullable: false),
                    OtherInfo = table.Column<string>(nullable: true),
                    ServicePackId = table.Column<Guid>(nullable: false),
                    SparateValue = table.Column<decimal>(nullable: false),
                    SrfId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrfEscalationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SrfEscalationRequest_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfEscalationRequest_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SrfEscalationRequest_ServicePack_ServicePackId",
                        column: x => x.ServicePackId,
                        principalTable: "ServicePack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SrfEscalationRequest_SrfRequest_SrfId",
                        column: x => x.SrfId,
                        principalTable: "SrfRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountName_Com",
                table: "AccountName",
                column: "Com");

            migrationBuilder.CreateIndex(
                name: "IX_AccountName_CreatedBy",
                table: "AccountName",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountName_LastEditedBy",
                table: "AccountName",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityCode_CreatedBy",
                table: "ActivityCode",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityCode_LastEditedBy",
                table: "ActivityCode",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceForm_CreatedBy",
                table: "AllowanceForm",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceForm_LastEditedBy",
                table: "AllowanceForm",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceList_CreatedBy",
                table: "AllowanceList",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceList_LastEditedBy",
                table: "AllowanceList",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AllowanceList_ServicePackId",
                table: "AllowanceList",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_AccountNameId",
                table: "AttendaceExceptionList",
                column: "AccountNameId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_ActivityId",
                table: "AttendaceExceptionList",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_AgencyId",
                table: "AttendaceExceptionList",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_ApproverOneId",
                table: "AttendaceExceptionList",
                column: "ApproverOneId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_ApproverTwoId",
                table: "AttendaceExceptionList",
                column: "ApproverTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_ContractorId",
                table: "AttendaceExceptionList",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_CostId",
                table: "AttendaceExceptionList",
                column: "CostId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_CreatedBy",
                table: "AttendaceExceptionList",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_DepartmentId",
                table: "AttendaceExceptionList",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_DepartmentSubId",
                table: "AttendaceExceptionList",
                column: "DepartmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_LastEditedBy",
                table: "AttendaceExceptionList",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_LocationId",
                table: "AttendaceExceptionList",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_NetworkId",
                table: "AttendaceExceptionList",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_ProjectId",
                table: "AttendaceExceptionList",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_SubOpsId",
                table: "AttendaceExceptionList",
                column: "SubOpsId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendaceExceptionList_TimeSheetTypeId",
                table: "AttendaceExceptionList",
                column: "TimeSheetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_CreatedBy",
                table: "AttendanceRecord",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_LastEditedBy",
                table: "AttendanceRecord",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_TimeSheetId",
                table: "AttendanceRecord",
                column: "TimeSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoGenerateVariable_CreatedBy",
                table: "AutoGenerateVariable",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AutoGenerateVariable_LastEditedBy",
                table: "AutoGenerateVariable",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BackupLog_CreatedBy",
                table: "BackupLog",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BackupLog_LastEditedBy",
                table: "BackupLog",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessNote_CreatedBy",
                table: "BusinessNote",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessNote_LastEditedBy",
                table: "BusinessNote",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_AccountId",
                table: "CandidateInfo",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_AgencyId",
                table: "CandidateInfo",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_CreatedBy",
                table: "CandidateInfo",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_HomeBaseId",
                table: "CandidateInfo",
                column: "HomeBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_LastEditedBy",
                table: "CandidateInfo",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_RequestById",
                table: "CandidateInfo",
                column: "RequestById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateInfo_VacancyId",
                table: "CandidateInfo",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_City_CreatedBy",
                table: "City",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_City_LastEditedBy",
                table: "City",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ActivityCodeId",
                table: "Claim",
                column: "ActivityCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_AgencyId",
                table: "Claim",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ClaimApproverOneId",
                table: "Claim",
                column: "ClaimApproverOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ClaimApproverTwoId",
                table: "Claim",
                column: "ClaimApproverTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ClaimCategoryId",
                table: "Claim",
                column: "ClaimCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ClaimForId",
                table: "Claim",
                column: "ClaimForId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ContractorId",
                table: "Claim",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ContractorProfileId",
                table: "Claim",
                column: "ContractorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_CostCenterId",
                table: "Claim",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_CreatedBy",
                table: "Claim",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_DepartureId",
                table: "Claim",
                column: "DepartureId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_DestinationId",
                table: "Claim",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_LastEditedBy",
                table: "Claim",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_NetworkNumberId",
                table: "Claim",
                column: "NetworkNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_ProjectId",
                table: "Claim",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_RedeemForId",
                table: "Claim",
                column: "RedeemForId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimCategory_CreatedBy",
                table: "ClaimCategory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimCategory_LastEditedBy",
                table: "ClaimCategory",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_CreatedBy",
                table: "CostCenter",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_DepartmentId",
                table: "CostCenter",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_LastEditedBy",
                table: "CostCenter",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClaim_CreatedBy",
                table: "CustomerClaim",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClaim_LastEditedBy",
                table: "CustomerClaim",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerData_BranchId",
                table: "CustomerData",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerData_CreatedBy",
                table: "CustomerData",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerData_LastEditedBy",
                table: "CustomerData",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Departement_CreatedBy",
                table: "Departement",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Departement_HeadId",
                table: "Departement",
                column: "HeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Departement_LastEditedBy",
                table: "Departement",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DepartementSub_CreatedBy",
                table: "DepartementSub",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DepartementSub_DepartmentId",
                table: "DepartementSub",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartementSub_LastEditedBy",
                table: "DepartementSub",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DepartementSub_LineManagerid",
                table: "DepartementSub",
                column: "LineManagerid");

            migrationBuilder.CreateIndex(
                name: "IX_DutySchedule_CreatedBy",
                table: "DutySchedule",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DutySchedule_LastEditedBy",
                table: "DutySchedule",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailArchieve_CreatedBy",
                table: "EmailArchieve",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailArchieve_LastEditedBy",
                table: "EmailArchieve",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FingerPrint_CreatedBy",
                table: "FingerPrint",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FingerPrint_LastEditedBy",
                table: "FingerPrint",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Fortest_CreatedBy",
                table: "Fortest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Fortest_LastEditedBy",
                table: "Fortest",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GenerateLog_CreatedBy",
                table: "GenerateLog",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GenerateLog_LastEditedBy",
                table: "GenerateLog",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobStage_CreatedBy",
                table: "JobStage",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobStage_LastEditedBy",
                table: "JobStage",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_AccountNameId",
                table: "NetworkNumber",
                column: "AccountNameId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_CreatedBy",
                table: "NetworkNumber",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_DepartmentId",
                table: "NetworkNumber",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_LastEditedBy",
                table: "NetworkNumber",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_LineManagerId",
                table: "NetworkNumber",
                column: "LineManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_ProjectId",
                table: "NetworkNumber",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkNumber_ProjectManagerId",
                table: "NetworkNumber",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageType_CreatedBy",
                table: "PackageType",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PackageType_LastEditedBy",
                table: "PackageType",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PanelCategory_CreatedBy",
                table: "PanelCategory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PanelCategory_LastEditedBy",
                table: "PanelCategory",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Position_CreatedBy",
                table: "Position",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Position_LastEditedBy",
                table: "Position",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastEditedBy",
                table: "Projects",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpareParts_CreatedBy",
                table: "RequestSpareParts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpareParts_LastEditedBy",
                table: "RequestSpareParts",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpareParts_PanelCategoryId",
                table: "RequestSpareParts",
                column: "PanelCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePack_CreatedBy",
                table: "ServicePack",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePack_LastEditedBy",
                table: "ServicePack",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePack_ServicePackCategoryId",
                table: "ServicePack",
                column: "ServicePackCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackCategory_CreatedBy",
                table: "ServicePackCategory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackCategory_LastEditedBy",
                table: "ServicePackCategory",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_CreatedBy",
                table: "SrfEscalationRequest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_LastEditedBy",
                table: "SrfEscalationRequest",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_ServicePackId",
                table: "SrfEscalationRequest",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_SrfId",
                table: "SrfEscalationRequest",
                column: "SrfId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_AccountId",
                table: "SrfRequest",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ActivityId",
                table: "SrfRequest",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveFiveId",
                table: "SrfRequest",
                column: "ApproveFiveId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveFourId",
                table: "SrfRequest",
                column: "ApproveFourId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveOneId",
                table: "SrfRequest",
                column: "ApproveOneId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveSixId",
                table: "SrfRequest",
                column: "ApproveSixId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveThreeId",
                table: "SrfRequest",
                column: "ApproveThreeId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ApproveTwoId",
                table: "SrfRequest",
                column: "ApproveTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_CandidateId",
                table: "SrfRequest",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_CostCenterId",
                table: "SrfRequest",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_CreatedBy",
                table: "SrfRequest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_DepartmentId",
                table: "SrfRequest",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_DepartmentSubId",
                table: "SrfRequest",
                column: "DepartmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ExtendFrom",
                table: "SrfRequest",
                column: "ExtendFrom");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_LastEditedBy",
                table: "SrfRequest",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_LineManagerId",
                table: "SrfRequest",
                column: "LineManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_NetworkId",
                table: "SrfRequest",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ProjectManagerId",
                table: "SrfRequest",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_SrfRequest_ServicePackId",
                table: "SrfRequest",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_Subdivision_CreatedBy",
                table: "Subdivision",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subdivision_LastEditedBy",
                table: "Subdivision",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubOps_CreatedBy",
                table: "SubOps",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubOps_LastEditedBy",
                table: "SubOps",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SystemBranch_CreatedBy",
                table: "SystemBranch",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SystemBranch_LastEditedBy",
                table: "SystemBranch",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPropertiesRecord_CreatedBy",
                table: "SystemPropertiesRecord",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPropertiesRecord_LastEditedBy",
                table: "SystemPropertiesRecord",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CreatedBy",
                table: "Ticket",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_LastEditedBy",
                table: "Ticket",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfo_ClaimId",
                table: "TicketInfo",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfo_CreatedBy",
                table: "TicketInfo",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfo_LastEditedBy",
                table: "TicketInfo",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketReply_CreatedBy",
                table: "TicketReply",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketReply_LastEditedBy",
                table: "TicketReply",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TicketReply_TicketId",
                table: "TicketReply",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetPeriod_CreatedBy",
                table: "TimeSheetPeriod",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetPeriod_LastEditedBy",
                table: "TimeSheetPeriod",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetType_CreatedBy",
                table: "TimeSheetType",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSheetType_LastEditedBy",
                table: "TimeSheetType",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_AccountNameId",
                table: "VacancyList",
                column: "AccountNameId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ApproverFourId",
                table: "VacancyList",
                column: "ApproverFourId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ApproverOneId",
                table: "VacancyList",
                column: "ApproverOneId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ApproverThreeId",
                table: "VacancyList",
                column: "ApproverThreeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ApproverTwoId",
                table: "VacancyList",
                column: "ApproverTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_CostCodeId",
                table: "VacancyList",
                column: "CostCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_CreatedBy",
                table: "VacancyList",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_DepartmentId",
                table: "VacancyList",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_DepartmentSubId",
                table: "VacancyList",
                column: "DepartmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_JobStageId",
                table: "VacancyList",
                column: "JobStageId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_LastEditedBy",
                table: "VacancyList",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_NetworkId",
                table: "VacancyList",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_PackageTypeId",
                table: "VacancyList",
                column: "PackageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_RequestById",
                table: "VacancyList",
                column: "RequestById");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ServicePackCategoryId",
                table: "VacancyList",
                column: "ServicePackCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyList_ServicePackId",
                table: "VacancyList",
                column: "ServicePackId");

            migrationBuilder.CreateIndex(
                name: "IX_WebSetting_CreatedBy",
                table: "WebSetting",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WebSetting_LastEditedBy",
                table: "WebSetting",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WoItem_CreatedBy",
                table: "WoItem",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WoItem_LastEditedBy",
                table: "WoItem",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WotList_CreatedBy",
                table: "WotList",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WotList_LastEditedBy",
                table: "WotList",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Language_CreatedBy",
                table: "Language",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Language_LastEditedBy",
                table: "Language",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LocaleResource_CreatedBy",
                table: "LocaleResource",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LocaleResource_LanguageId",
                table: "LocaleResource",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaleResource_LastEditedBy",
                table: "LocaleResource",
                column: "LastEditedBy");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowanceForm");

            migrationBuilder.DropTable(
                name: "AllowanceList");

            migrationBuilder.DropTable(
                name: "AttendaceExceptionList");

            migrationBuilder.DropTable(
                name: "AttendanceRecord");

            migrationBuilder.DropTable(
                name: "AutoGenerateVariable");

            migrationBuilder.DropTable(
                name: "BackupLog");

            migrationBuilder.DropTable(
                name: "BusinessNote");

            migrationBuilder.DropTable(
                name: "CustomerClaim");

            migrationBuilder.DropTable(
                name: "CustomerData");

            migrationBuilder.DropTable(
                name: "DutySchedule");

            migrationBuilder.DropTable(
                name: "EmailArchieve");

            migrationBuilder.DropTable(
                name: "FingerPrint");

            migrationBuilder.DropTable(
                name: "Fortest");

            migrationBuilder.DropTable(
                name: "GenerateLog");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "RequestSpareParts");

            migrationBuilder.DropTable(
                name: "SrfEscalationRequest");

            migrationBuilder.DropTable(
                name: "Subdivision");

            migrationBuilder.DropTable(
                name: "SystemPropertiesRecord");

            migrationBuilder.DropTable(
                name: "TicketInfo");

            migrationBuilder.DropTable(
                name: "TicketReply");

            migrationBuilder.DropTable(
                name: "WebSetting");

            migrationBuilder.DropTable(
                name: "WoItem");

            migrationBuilder.DropTable(
                name: "WotList");

            migrationBuilder.DropTable(
                name: "LocaleResource");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "SubOps");

            migrationBuilder.DropTable(
                name: "TimeSheetType");

            migrationBuilder.DropTable(
                name: "TimeSheetPeriod");

            migrationBuilder.DropTable(
                name: "SystemBranch");

            migrationBuilder.DropTable(
                name: "PanelCategory");

            migrationBuilder.DropTable(
                name: "SrfRequest");

            migrationBuilder.DropTable(
                name: "Claim");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ActivityCode");

            migrationBuilder.DropTable(
                name: "ClaimCategory");

            migrationBuilder.DropTable(
                name: "CandidateInfo");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "VacancyList");

            migrationBuilder.DropTable(
                name: "CostCenter");

            migrationBuilder.DropTable(
                name: "DepartementSub");

            migrationBuilder.DropTable(
                name: "JobStage");

            migrationBuilder.DropTable(
                name: "NetworkNumber");

            migrationBuilder.DropTable(
                name: "PackageType");

            migrationBuilder.DropTable(
                name: "ServicePack");

            migrationBuilder.DropTable(
                name: "AccountName");

            migrationBuilder.DropTable(
                name: "Departement");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ServicePackCategory");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
