using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatersAD.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceAndRequestWaterMeter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_Tiers_TierId",
                table: "Consumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_WaterMeters_WaterMeterId",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "Edit",
                table: "Consumptions");

            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "RequestWaterMeters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocalityWaterMeterId",
                table: "RequestWaterMeters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaterMeterLocalityId",
                table: "RequestWaterMeters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LimitDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "WaterMeterId",
                table: "Consumptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TierId",
                table: "Consumptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestWaterMeters_LocalityId",
                table: "RequestWaterMeters",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestWaterMeters_WaterMeterLocalityId",
                table: "RequestWaterMeters",
                column: "WaterMeterLocalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_Tiers_TierId",
                table: "Consumptions",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_WaterMeters_WaterMeterId",
                table: "Consumptions",
                column: "WaterMeterId",
                principalTable: "WaterMeters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestWaterMeters_Localities_LocalityId",
                table: "RequestWaterMeters",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestWaterMeters_Localities_WaterMeterLocalityId",
                table: "RequestWaterMeters",
                column: "WaterMeterLocalityId",
                principalTable: "Localities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_Tiers_TierId",
                table: "Consumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_WaterMeters_WaterMeterId",
                table: "Consumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestWaterMeters_Localities_LocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestWaterMeters_Localities_WaterMeterLocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropIndex(
                name: "IX_RequestWaterMeters_LocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropIndex(
                name: "IX_RequestWaterMeters_WaterMeterLocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropColumn(
                name: "LocalityWaterMeterId",
                table: "RequestWaterMeters");

            migrationBuilder.DropColumn(
                name: "WaterMeterLocalityId",
                table: "RequestWaterMeters");

            migrationBuilder.DropColumn(
                name: "LimitDate",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "WaterMeterId",
                table: "Consumptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TierId",
                table: "Consumptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "Edit",
                table: "Consumptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_Tiers_TierId",
                table: "Consumptions",
                column: "TierId",
                principalTable: "Tiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_WaterMeters_WaterMeterId",
                table: "Consumptions",
                column: "WaterMeterId",
                principalTable: "WaterMeters",
                principalColumn: "Id");
        }
    }
}
