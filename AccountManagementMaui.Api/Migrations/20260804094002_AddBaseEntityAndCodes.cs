using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityAndCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Districts_CityId_Name",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Name",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Districts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Districts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeleteReason",
                table: "Districts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "Districts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Districts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedByUserId",
                table: "Districts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Districts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "Cities",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Cities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeleteReason",
                table: "Cities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedByUserId",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Cities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId_Name",
                table: "Districts",
                columns: new[] { "CityId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_DistrictCode",
                table: "Districts",
                column: "DistrictCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CityCode",
                table: "Cities",
                column: "CityCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Districts_CityId_Name",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Districts_DistrictCode",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CityCode",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_Name",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Cities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId_Name",
                table: "Districts",
                columns: new[] { "CityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);
        }
    }
}
