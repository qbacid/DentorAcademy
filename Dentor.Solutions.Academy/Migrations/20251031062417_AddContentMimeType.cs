using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentor.Solutions.Academy.Migrations
{
    /// <inheritdoc />
    public partial class AddContentMimeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseContents_ContentType",
                table: "CourseContents");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "CourseContents");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "CourseContents",
                newName: "CourseContentType");

            migrationBuilder.AddColumn<string>(
                name: "ContentMimeType",
                table: "CourseContents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CourseContents_ContentMimeType",
                table: "CourseContents",
                column: "ContentMimeType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseContents_ContentMimeType",
                table: "CourseContents");

            migrationBuilder.DropColumn(
                name: "ContentMimeType",
                table: "CourseContents");

            migrationBuilder.RenameColumn(
                name: "CourseContentType",
                table: "CourseContents",
                newName: "ContentType");

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "CourseContents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseContents_ContentType",
                table: "CourseContents",
                column: "ContentType");
        }
    }
}
