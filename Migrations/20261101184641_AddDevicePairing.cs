using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cortex_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDevicePairing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DevicePairingAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PairingId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Challenge = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicePairingAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevicePairingAttempts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DevicePairingAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevicePairingAttempts_DeviceId",
                table: "DevicePairingAttempts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DevicePairingAttempts_PairingId",
                table: "DevicePairingAttempts",
                column: "PairingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DevicePairingAttempts_UserId",
                table: "DevicePairingAttempts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevicePairingAttempts");

            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "Devices");
        }
    }
}
