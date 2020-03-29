using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Web.Migrations
{
    public partial class removehourstimesheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SrfEscalationRequest_SrfId",
                table: "SrfEscalationRequest");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "AttendaceExceptionList");

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_SrfId",
                table: "SrfEscalationRequest",
                column: "SrfId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SrfEscalationRequest_SrfId",
                table: "SrfEscalationRequest");

            migrationBuilder.AddColumn<string>(
                name: "Hours",
                table: "AttendaceExceptionList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SrfEscalationRequest_SrfId",
                table: "SrfEscalationRequest",
                column: "SrfId");
        }
    }
}
