using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCoursePlatformEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "course_id",
                table: "quizzes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    full_description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    thumbnail_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cover_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    difficulty_level = table.Column<string>(type: "text", nullable: false),
                    estimated_duration_hours = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false),
                    created_by_user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                    table.ForeignKey(
                        name: "fk_courses_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "course_enrollments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    enrolled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    progress_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    last_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    certificate_issued = table.Column<bool>(type: "boolean", nullable: false),
                    certificate_issued_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    certificate_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    payment_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    payment_transaction_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_enrollments_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_course_enrollments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    estimated_duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_modules", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_modules_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_contents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_module_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    content_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    blob_container_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    blob_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    blob_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    quiz_id = table.Column<int>(type: "integer", nullable: true),
                    external_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_free_preview = table.Column<bool>(type: "boolean", nullable: false),
                    is_downloadable = table.Column<bool>(type: "boolean", nullable: false),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_contents", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_contents_course_modules_course_module_id",
                        column: x => x.course_module_id,
                        principalTable: "course_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_contents_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "course_module_progress",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enrollment_id = table.Column<int>(type: "integer", nullable: false),
                    course_module_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    progress_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    first_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_module_progress", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_module_progress_course_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "course_enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_module_progress_course_modules_course_module_id",
                        column: x => x.course_module_id,
                        principalTable: "course_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_module_progress_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_progress",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enrollment_id = table.Column<int>(type: "integer", nullable: false),
                    course_content_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    quiz_passed = table.Column<bool>(type: "boolean", nullable: true),
                    quiz_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    quiz_attempt_id = table.Column<int>(type: "integer", nullable: true),
                    progress_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    time_spent_seconds = table.Column<int>(type: "integer", nullable: false),
                    last_position_seconds = table.Column<int>(type: "integer", nullable: true),
                    first_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_accessed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    access_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_progress", x => x.id);
                    table.ForeignKey(
                        name: "fk_course_progress_course_contents_course_content_id",
                        column: x => x.course_content_id,
                        principalTable: "course_contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_progress_course_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "course_enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_course_progress_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_course_id",
                table: "quizzes",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_contents_content_type",
                table: "course_contents",
                column: "content_type");

            migrationBuilder.CreateIndex(
                name: "ix_course_contents_course_module_id_order_index",
                table: "course_contents",
                columns: new[] { "course_module_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "ix_course_contents_quiz_id",
                table: "course_contents",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_enrollments_course_id",
                table: "course_enrollments",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_enrollments_enrolled_at",
                table: "course_enrollments",
                column: "enrolled_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_enrollments_status",
                table: "course_enrollments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_course_enrollments_user_id_course_id",
                table: "course_enrollments",
                columns: new[] { "user_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_module_progress_course_module_id",
                table: "course_module_progress",
                column: "course_module_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_module_progress_enrollment_id_course_module_id",
                table: "course_module_progress",
                columns: new[] { "enrollment_id", "course_module_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_module_progress_user_id_course_module_id",
                table: "course_module_progress",
                columns: new[] { "user_id", "course_module_id" });

            migrationBuilder.CreateIndex(
                name: "ix_course_modules_course_id_order_index",
                table: "course_modules",
                columns: new[] { "course_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "ix_course_progress_completed_at",
                table: "course_progress",
                column: "completed_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_progress_course_content_id",
                table: "course_progress",
                column: "course_content_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_progress_enrollment_id_course_content_id",
                table: "course_progress",
                columns: new[] { "enrollment_id", "course_content_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_progress_user_id_course_content_id",
                table: "course_progress",
                columns: new[] { "user_id", "course_content_id" });

            migrationBuilder.CreateIndex(
                name: "ix_courses_category",
                table: "courses",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_courses_created_at",
                table: "courses",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_courses_created_by_user_id",
                table: "courses",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_is_published",
                table: "courses",
                column: "is_published");

            migrationBuilder.AddForeignKey(
                name: "fk_quizzes_courses_course_id",
                table: "quizzes",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_quizzes_courses_course_id",
                table: "quizzes");

            migrationBuilder.DropTable(
                name: "course_module_progress");

            migrationBuilder.DropTable(
                name: "course_progress");

            migrationBuilder.DropTable(
                name: "course_contents");

            migrationBuilder.DropTable(
                name: "course_enrollments");

            migrationBuilder.DropTable(
                name: "course_modules");

            migrationBuilder.DropTable(
                name: "courses");

            migrationBuilder.DropIndex(
                name: "ix_quizzes_course_id",
                table: "quizzes");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "quizzes");
        }
    }
}
