using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleMaster.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramChatId",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_TelegramChatId",
                table: "users",
                column: "TelegramChatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_TelegramChatId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "TelegramChatId",
                table: "users");
        }
    }
}
