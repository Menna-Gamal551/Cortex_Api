using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cortex_Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeDeviceOwnedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Devices_DeviceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeviceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId",
                table: "Devices",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Users_UserId",
                table: "Devices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Users_UserId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents");

            migrationBuilder.DropIndex(
                name: "IX_Devices_UserId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeviceId",
                table: "Users",
                column: "DeviceId",
                unique: true,
                filter: "[DeviceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Devices_DeviceId",
                table: "Users",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }
    }
}
