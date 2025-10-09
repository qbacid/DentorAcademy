using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUserResponseTextAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "text_answer",
                table: "user_responses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "text_answer",
                table: "user_responses");
        }
    }
}
