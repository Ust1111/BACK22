using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticulationAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeftText = table.Column<string>(type: "text", nullable: false),
                    LeftLanguage = table.Column<string>(type: "text", nullable: false),
                    LeftTranscription = table.Column<string>(type: "text", nullable: true),
                    RightText = table.Column<string>(type: "text", nullable: false),
                    RightLanguage = table.Column<string>(type: "text", nullable: false),
                    RightTranscription = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoritePairs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePairs_UserId",
                table: "FavoritePairs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritePairs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
