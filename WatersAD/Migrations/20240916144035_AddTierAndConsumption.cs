using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class AddTierAndConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TierNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    TierPrice = table.Column<double>(type: "float", nullable: false),
                    TierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpperLimit = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TierId = table.Column<int>(type: "int", nullable: true),
                    ConsumptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumptionValue = table.Column<double>(type: "float", nullable: false),
                    WaterMeterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consumptions_Tiers_TierId",
                        column: x => x.TierId,
                        principalTable: "Tiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Consumptions_WaterMeters_WaterMeterId",
                        column: x => x.WaterMeterId,
                        principalTable: "WaterMeters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_TierId",
                table: "Consumptions",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_WaterMeterId",
                table: "Consumptions",
                column: "WaterMeterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consumptions");

            migrationBuilder.DropTable(
                name: "Tiers");
        }
    }
}
