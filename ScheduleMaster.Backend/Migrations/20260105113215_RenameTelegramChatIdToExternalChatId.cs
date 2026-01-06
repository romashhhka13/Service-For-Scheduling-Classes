using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleMaster.Migrations
{
    /// <inheritdoc />
    public partial class RenameTelegramChatIdToExternalChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TelegramChatId",
                table: "users",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_users_TelegramChatId",
                table: "users",
                newName: "IX_users_ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "users",
                newName: "TelegramChatId");

            migrationBuilder.RenameIndex(
                name: "IX_users_ChatId",
                table: "users",
                newName: "IX_users_TelegramChatId");
        }
    }
}
