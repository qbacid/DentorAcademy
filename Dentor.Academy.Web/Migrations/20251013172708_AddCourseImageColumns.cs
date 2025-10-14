using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseImageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "cover_image",
                table: "courses",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_image_content_type",
                table: "courses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thumbnail_content_type",
                table: "courses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "thumbnail_image",
                table: "courses",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cover_image",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "cover_image_content_type",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "thumbnail_content_type",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "thumbnail_image",
                table: "courses");
        }
    }
}
