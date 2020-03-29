using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Web.Migrations
{
    public partial class addingmasterdataholidays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CustomField1 = table.Column<string>(nullable: true),
                    CustomField2 = table.Column<string>(nullable: true),
                    CustomField3 = table.Column<string>(nullable: true),
                    DateDay = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LastEditedBy = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Holidays_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Holidays_AspNetUsers_LastEditedBy",
                        column: x => x.LastEditedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_CreatedBy",
                table: "Holidays",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_LastEditedBy",
                table: "Holidays",
                column: "LastEditedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holidays");
        }
    }
}
