using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Web.Migrations
{
    public partial class TRCP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketInfo_ClaimId",
                table: "TicketInfo");

            migrationBuilder.CreateTable(
                name: "TacticalResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Approved = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateSrf = table.Column<DateTime>(nullable: true),
                    DepartmentId = table.Column<Guid>(nullable: true),
                    DepartmentSubId = table.Column<Guid>(nullable: true),
                    Forecast = table.Column<int>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacticalResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TacticalResource_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TacticalResource_Departement_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TacticalResource_DepartementSub_DepartmentSubId",
                        column: x => x.DepartmentSubId,
                        principalTable: "DepartementSub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TacticalResource_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TacticalResource_CreatedBy",
                table: "TacticalResource",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TacticalResource_DepartmentId",
                table: "TacticalResource",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TacticalResource_DepartmentSubId",
                table: "TacticalResource",
                column: "DepartmentSubId");

            migrationBuilder.CreateIndex(
                name: "IX_TacticalResource_LastEditedBy",
                table: "TacticalResource",
                column: "LastEditedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TacticalResource");

            migrationBuilder.DropIndex(
                name: "IX_TicketInfo_ClaimId",
                table: "TicketInfo");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInfo_ClaimId",
                table: "TicketInfo",
                column: "ClaimId");
        }
    }
}
