using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeCardTypeAndKindCodesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountCardTypes_TypeCode",
                table: "AccountCardTypes");

            migrationBuilder.DropIndex(
                name: "IX_AccountCardKinds_KindCode",
                table: "AccountCardKinds");

            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "AccountCardTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "KindCode",
                table: "AccountCardKinds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TypeCode",
                table: "AccountCardTypes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KindCode",
                table: "AccountCardKinds",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardTypes_TypeCode",
                table: "AccountCardTypes",
                column: "TypeCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardKinds_KindCode",
                table: "AccountCardKinds",
                column: "KindCode",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
