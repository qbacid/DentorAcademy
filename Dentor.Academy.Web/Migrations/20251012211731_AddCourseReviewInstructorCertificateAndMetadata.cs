using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseReviewInstructorCertificateAndMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "learning_objectives",
                table: "courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prerequisites",
                table: "courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tags",
                table: "courses",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "course_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    icon_class = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "course_certificates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    certificate_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    enrollment_id = table.Column<int>(type: "integer", nullable: false),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    student_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    course_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    completion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    certificate_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    certificate_pdf_blob_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    revoked_by_user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    verification_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    verified_count = table.Column<int>(type: "integer", nullable: false),
                    last_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    course_duration_hours = table.Column<int>(type: "integer", nullable: true),
                    grade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_certificates", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_certificates_asp_net_users_revoked_by_user_id",
                        column: x => x.revoked_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_course_certificates_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_certificates_course_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "course_enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_certificates_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_instructors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_instructors", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_instructors_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_instructors_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_reviews",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    rating = table.Column<int>(type: "integer", precision: 3, scale: 2, nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    review_text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by_user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false),
                    helpful_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_reviews_asp_net_users_approved_by_user_id",
                        column: x => x.approved_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_course_reviews_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_reviews_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_courses_category_id",
                table: "courses",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_categories_display_order",
                table: "course_categories",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "ix_course_categories_is_active",
                table: "course_categories",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_course_categories_name",
                table: "course_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_certificate_number",
                table: "course_certificates",
                column: "certificate_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_course_id",
                table: "course_certificates",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_enrollment_id",
                table: "course_certificates",
                column: "enrollment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_issued_at",
                table: "course_certificates",
                column: "issued_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_revoked_by_user_id",
                table: "course_certificates",
                column: "revoked_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_certificates_user_id_course_id",
                table: "course_certificates",
                columns: new[] { "user_id", "course_id" });

            migrationBuilder.CreateIndex(
                name: "ix_course_instructors_course_id_user_id",
                table: "course_instructors",
                columns: new[] { "course_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_instructors_user_id",
                table: "course_instructors",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_approved_by_user_id",
                table: "course_reviews",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_course_id_rating",
                table: "course_reviews",
                columns: new[] { "course_id", "rating" });

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_course_id_user_id",
                table: "course_reviews",
                columns: new[] { "course_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_created_at",
                table: "course_reviews",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_is_approved",
                table: "course_reviews",
                column: "is_approved");

            migrationBuilder.CreateIndex(
                name: "ix_course_reviews_user_id",
                table: "course_reviews",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_courses_course_categories_category_id",
                table: "courses",
                column: "category_id",
                principalTable: "course_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_courses_course_categories_category_id",
                table: "courses");

            migrationBuilder.DropTable(
                name: "course_categories");

            migrationBuilder.DropTable(
                name: "course_certificates");

            migrationBuilder.DropTable(
                name: "course_instructors");

            migrationBuilder.DropTable(
                name: "course_reviews");

            migrationBuilder.DropIndex(
                name: "ix_courses_category_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "learning_objectives",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "prerequisites",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "courses");
        }
    }
}
