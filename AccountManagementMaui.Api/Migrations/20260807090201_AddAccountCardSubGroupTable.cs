using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountCardSubGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCardSubGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubGroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountCardGroupId = table.Column<int>(type: "int", nullable: false),
                    AccountCardGroupId1 = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCardSubGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCardSubGroups_AccountCardGroups_AccountCardGroupId",
                        column: x => x.AccountCardGroupId,
                        principalTable: "AccountCardGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCardSubGroups_AccountCardGroups_AccountCardGroupId1",
                        column: x => x.AccountCardGroupId1,
                        principalTable: "AccountCardGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountCardSubGroups_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountCardSubGroups_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardSubGroups_AccountCardGroupId_SubGroupName",
                table: "AccountCardSubGroups",
                columns: new[] { "AccountCardGroupId", "SubGroupName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardSubGroups_AccountCardGroupId1",
                table: "AccountCardSubGroups",
                column: "AccountCardGroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardSubGroups_CreatedByUserId",
                table: "AccountCardSubGroups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardSubGroups_ModifiedByUserId",
                table: "AccountCardSubGroups",
                column: "ModifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCardSubGroups");
        }
    }
}
