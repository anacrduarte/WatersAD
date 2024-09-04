using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class MakeImageUrlNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "ImageUrl",
           table: "Clients",
           type: "nvarchar(max)",
           nullable: true, // Agora permite valores nulos
           oldClrType: typeof(string),
           oldType: "nvarchar(max)",
           oldNullable: false); // Era NOT NULL antes

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "ImageUrl",
           table: "Clients",
           type: "nvarchar(max)",
           nullable: false, // Reverte para NOT NULL se necessário
           oldClrType: typeof(string),
           oldType: "nvarchar(max)",
           oldNullable: true);


        }
    }
}
