using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace App.Web.Migrations
{
    public partial class fixattendancerecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_TimeSheetPeriod_TimeSheetId",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "AttendanceDate",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "AttendanceHour",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "AttendanceType",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "AttendanceRecord");

            migrationBuilder.RenameColumn(
                name: "TimeSheetId",
                table: "AttendanceRecord",
                newName: "AttendaceExceptionListId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_TimeSheetId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_AttendaceExceptionListId");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "AttendanceRecord",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_AttendaceExceptionList_AttendaceExceptionListId",
                table: "AttendanceRecord",
                column: "AttendaceExceptionListId",
                principalTable: "AttendaceExceptionList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecord_AttendaceExceptionList_AttendaceExceptionListId",
                table: "AttendanceRecord");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "AttendanceRecord");

            migrationBuilder.RenameColumn(
                name: "AttendaceExceptionListId",
                table: "AttendanceRecord",
                newName: "TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecord_AttendaceExceptionListId",
                table: "AttendanceRecord",
                newName: "IX_AttendanceRecord_TimeSheetId");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "AttendanceRecord",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceHour",
                table: "AttendanceRecord",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceType",
                table: "AttendanceRecord",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "AttendanceRecord",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecord_TimeSheetPeriod_TimeSheetId",
                table: "AttendanceRecord",
                column: "TimeSheetId",
                principalTable: "TimeSheetPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
