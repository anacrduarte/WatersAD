using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpadateClientII : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_LocalityId",
                table: "Clients",
                column: "LocalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Localities_LocalityId",
                table: "Clients",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Localities_LocalityId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_LocalityId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "Clients");
        }
    }
}
