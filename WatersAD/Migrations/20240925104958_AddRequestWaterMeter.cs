using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestWaterMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TierName",
                table: "Tiers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RequestWaterMeters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    RemainPostalCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AddressWaterMeter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HouseNumberWaterMeter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCodeWaterMeter = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    RemainPostalCodeWaterMeter = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    WaterMeterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestWaterMeters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestWaterMeters_WaterMeters_WaterMeterId",
                        column: x => x.WaterMeterId,
                        principalTable: "WaterMeters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestWaterMeters_WaterMeterId",
                table: "RequestWaterMeters",
                column: "WaterMeterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestWaterMeters");

            migrationBuilder.AlterColumn<string>(
                name: "TierName",
                table: "Tiers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
