using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleMaster.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedules_groups_GroupId",
                table: "schedules");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "schedules",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "StudioId",
                table: "schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_groups_GroupId",
                table: "schedules",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_schedules_groups_GroupId",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "StudioId",
                table: "schedules");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_schedules_groups_GroupId",
                table: "schedules",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
