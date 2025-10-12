using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login_date",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_password_change_date",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "must_change_password",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "last_login_date",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "last_password_change_date",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "must_change_password",
                table: "AspNetUsers");
        }
    }
}
