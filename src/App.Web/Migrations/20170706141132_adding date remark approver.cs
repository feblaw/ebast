using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Web.Migrations
{
    public partial class addingdateremarkapprover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusFive",
                table: "SrfRequest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusFour",
                table: "SrfRequest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusOne",
                table: "SrfRequest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusSix",
                table: "SrfRequest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusThree",
                table: "SrfRequest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApproveStatusTwo",
                table: "SrfRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateApproveStatusFive",
                table: "SrfRequest");

            migrationBuilder.DropColumn(
                name: "DateApproveStatusFour",
                table: "SrfRequest");

            migrationBuilder.DropColumn(
                name: "DateApproveStatusOne",
                table: "SrfRequest");

            migrationBuilder.DropColumn(
                name: "DateApproveStatusSix",
                table: "SrfRequest");

            migrationBuilder.DropColumn(
                name: "DateApproveStatusThree",
                table: "SrfRequest");

            migrationBuilder.DropColumn(
                name: "DateApproveStatusTwo",
                table: "SrfRequest");
        }
    }
}
