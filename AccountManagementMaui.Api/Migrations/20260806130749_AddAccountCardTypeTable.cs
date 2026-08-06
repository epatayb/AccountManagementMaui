using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountCardTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCardTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCardTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCardTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountCardTypes_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardTypes_CreatedByUserId",
                table: "AccountCardTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardTypes_ModifiedByUserId",
                table: "AccountCardTypes",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardTypes_TypeCode",
                table: "AccountCardTypes",
                column: "TypeCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardTypes_TypeName",
                table: "AccountCardTypes",
                column: "TypeName",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCardTypes");
        }
    }
}
