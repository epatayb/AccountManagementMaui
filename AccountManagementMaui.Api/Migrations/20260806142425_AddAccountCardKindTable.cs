using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountCardKindTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCardKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KindCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KindName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountCardTypeId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCardKinds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCardKinds_AccountCardTypes_AccountCardTypeId",
                        column: x => x.AccountCardTypeId,
                        principalTable: "AccountCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCardKinds_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountCardKinds_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardKinds_AccountCardTypeId_KindName",
                table: "AccountCardKinds",
                columns: new[] { "AccountCardTypeId", "KindName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardKinds_CreatedByUserId",
                table: "AccountCardKinds",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardKinds_KindCode",
                table: "AccountCardKinds",
                column: "KindCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCardKinds_ModifiedByUserId",
                table: "AccountCardKinds",
                column: "ModifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCardKinds");
        }
    }
}
