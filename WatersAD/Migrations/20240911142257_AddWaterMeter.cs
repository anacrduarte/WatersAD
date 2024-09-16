using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WaterMeterServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterMeterServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaterMeters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HouseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocalityId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaterMeterServiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterMeters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterMeters_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterMeters_Localities_LocalityId",
                        column: x => x.LocalityId,
                        principalTable: "Localities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaterMeters_WaterMeterServices_WaterMeterServiceId",
                        column: x => x.WaterMeterServiceId,
                        principalTable: "WaterMeterServices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeters_ClientId",
                table: "WaterMeters",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeters_LocalityId",
                table: "WaterMeters",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeters_WaterMeterServiceId",
                table: "WaterMeters",
                column: "WaterMeterServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterMeters");

            migrationBuilder.DropTable(
                name: "WaterMeterServices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Clients");
        }
    }
}
