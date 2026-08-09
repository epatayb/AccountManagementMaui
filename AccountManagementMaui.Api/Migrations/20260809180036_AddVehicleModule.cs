using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountManagementMaui.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleKinds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KindName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleKinds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleKinds_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleKinds_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleTypes_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NormalizedPlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false),
                    VehicleKindId = table.Column<int>(type: "int", nullable: false),
                    TrailerPlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DriverAccountCardId = table.Column<int>(type: "int", nullable: false),
                    DriverIsLicenseOwner = table.Column<bool>(type: "bit", nullable: false),
                    ReferenceAccountCardId = table.Column<int>(type: "int", nullable: true),
                    LicenseAccountCardId = table.Column<int>(type: "int", nullable: false),
                    InvoiceAccountCardId = table.Column<int>(type: "int", nullable: false),
                    ReferenceIsInvoiceAccount = table.Column<bool>(type: "bit", nullable: false),
                    LicenseOwnerIsInvoiceAccount = table.Column<bool>(type: "bit", nullable: false),
                    LicenseOwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicenseOwnerTaxNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LicenseOwnerIdentityNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    LicenseOwnerAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LicenseOwnerCityId = table.Column<int>(type: "int", nullable: false),
                    LicenseOwnerTaxOfficeId = table.Column<int>(type: "int", nullable: true),
                    AuthorizedName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AuthorizedPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InsuranceExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleteReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_AccountCards_DriverAccountCardId",
                        column: x => x.DriverAccountCardId,
                        principalTable: "AccountCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_AccountCards_InvoiceAccountCardId",
                        column: x => x.InvoiceAccountCardId,
                        principalTable: "AccountCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_AccountCards_LicenseAccountCardId",
                        column: x => x.LicenseAccountCardId,
                        principalTable: "AccountCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_AccountCards_ReferenceAccountCardId",
                        column: x => x.ReferenceAccountCardId,
                        principalTable: "AccountCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_Cities_LicenseOwnerCityId",
                        column: x => x.LicenseOwnerCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_TaxOffices_LicenseOwnerTaxOfficeId",
                        column: x => x.LicenseOwnerTaxOfficeId,
                        principalTable: "TaxOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleKinds_VehicleKindId",
                        column: x => x.VehicleKindId,
                        principalTable: "VehicleKinds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleKinds_CreatedByUserId",
                table: "VehicleKinds",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleKinds_KindName",
                table: "VehicleKinds",
                column: "KindName",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleKinds_ModifiedByUserId",
                table: "VehicleKinds",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CreatedByUserId",
                table: "Vehicles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DriverAccountCardId",
                table: "Vehicles",
                column: "DriverAccountCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InspectionExpiryDate",
                table: "Vehicles",
                column: "InspectionExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InsuranceExpiryDate",
                table: "Vehicles",
                column: "InsuranceExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_InvoiceAccountCardId",
                table: "Vehicles",
                column: "InvoiceAccountCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_IsActive",
                table: "Vehicles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicenseAccountCardId",
                table: "Vehicles",
                column: "LicenseAccountCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicenseOwnerCityId",
                table: "Vehicles",
                column: "LicenseOwnerCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicenseOwnerTaxOfficeId",
                table: "Vehicles",
                column: "LicenseOwnerTaxOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModifiedByUserId",
                table: "Vehicles",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_NormalizedPlate",
                table: "Vehicles",
                column: "NormalizedPlate",
                unique: true,
                filter: "[IsDeleted] = 0 AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ReferenceAccountCardId",
                table: "Vehicles",
                column: "ReferenceAccountCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleKindId",
                table: "Vehicles",
                column: "VehicleKindId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleTypeId",
                table: "Vehicles",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_CreatedByUserId",
                table: "VehicleTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_ModifiedByUserId",
                table: "VehicleTypes",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_TypeName",
                table: "VehicleTypes",
                column: "TypeName",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleKinds");

            migrationBuilder.DropTable(
                name: "VehicleTypes");
        }
    }
}
