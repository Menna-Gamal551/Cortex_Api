using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cortex_Api.Migrations
{
    /// <inheritdoc />
    public partial class updatemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Auditlogs_Users_UserId",
                table: "Auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Users_UserId",
                table: "SecurityEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Devices_DeviceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeviceId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "OccuredAt",
                table: "SecurityEvents",
                newName: "OccurredAt");

            migrationBuilder.RenameColumn(
                name: "CheckedAT",
                table: "BreachResults",
                newName: "CheckedAt");

            migrationBuilder.RenameColumn(
                name: "OccuredAt",
                table: "Auditlogs",
                newName: "OccurredAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PairedAt",
                table: "Devices",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeviceId",
                table: "Users",
                column: "DeviceId",
                unique: true,
                filter: "[DeviceId] is not null");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditlogs_Devices_DeviceId",
                table: "Auditlogs",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Auditlogs_Users_UserId",
                table: "Auditlogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Users_UserId",
                table: "SecurityEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Devices_DeviceId",
                table: "Users",
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
                name: "FK_Auditlogs_Users_UserId",
                table: "Auditlogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityEvents_Users_UserId",
                table: "SecurityEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Devices_DeviceId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeviceId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "SecurityEvents",
                newName: "OccuredAt");

            migrationBuilder.RenameColumn(
                name: "CheckedAt",
                table: "BreachResults",
                newName: "CheckedAT");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "Auditlogs",
                newName: "OccuredAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PairedAt",
                table: "Devices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
                name: "FK_Auditlogs_Users_UserId",
                table: "Auditlogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Devices_DeviceId",
                table: "SecurityEvents",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityEvents_Users_UserId",
                table: "SecurityEvents",
                column: "UserId",
                principalTable: "Users",
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
