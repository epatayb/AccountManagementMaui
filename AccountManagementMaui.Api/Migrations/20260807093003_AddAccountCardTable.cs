using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountCardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, computedColumnSql: "('HSP' + RIGHT('00000000' + CONVERT(varchar(20), [Id]), 8))", stored: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AccountCardTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountCardKindId = table.Column<int>(type: "int", nullable: false),
                    AccountCardGroupId = table.Column<int>(type: "int", nullable: true),
                    AccountCardSubGroupId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    TaxOfficeId = table.Column<int>(type: "int", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountCards_AccountCardGroups_AccountCardGroupId",
                        column: x => x.AccountCardGroupId,
                        principalTable: "AccountCardGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_AccountCardKinds_AccountCardKindId",
                        column: x => x.AccountCardKindId,
                        principalTable: "AccountCardKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_AccountCardSubGroups_AccountCardSubGroupId",
                        column: x => x.AccountCardSubGroupId,
                        principalTable: "AccountCardSubGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_AccountCardTypes_AccountCardTypeId",
                        column: x => x.AccountCardTypeId,
                        principalTable: "AccountCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_TaxOffices_TaxOfficeId",
                        column: x => x.TaxOfficeId,
                        principalTable: "TaxOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCards_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountCards_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_AccountCardGroupId",
                table: "AccountCards",
                column: "AccountCardGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_AccountCardKindId",
                table: "AccountCards",
                column: "AccountCardKindId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_AccountCardSubGroupId",
                table: "AccountCards",
                column: "AccountCardSubGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_AccountCardTypeId",
                table: "AccountCards",
                column: "AccountCardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_AccountCode",
                table: "AccountCards",
                column: "AccountCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_CityId",
                table: "AccountCards",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_CreatedByUserId",
                table: "AccountCards",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_DistrictId",
                table: "AccountCards",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_ModifiedByUserId",
                table: "AccountCards",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCards_TaxOfficeId",
                table: "AccountCards",
                column: "TaxOfficeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCards");
        }
    }
}
