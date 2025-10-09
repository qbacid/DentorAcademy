using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dentor.Academy.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialQuizModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    passing_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    time_limit_minutes = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quizzes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quiz_id = table.Column<int>(type: "integer", nullable: false),
                    question_text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    question_type = table.Column<int>(type: "integer", nullable: false),
                    explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    explanation_image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    points = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quiz_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    total_points_earned = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    total_points_possible = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    passed = table.Column<bool>(type: "boolean", nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "answer_options",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    option_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_answer_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_answer_options_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_responses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quiz_attempt_id = table.Column<int>(type: "integer", nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    points_earned = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    answered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_responses", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_responses_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_responses_quiz_attempts_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_response_answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_response_id = table.Column<int>(type: "integer", nullable: false),
                    answer_option_id = table.Column<int>(type: "integer", nullable: false),
                    selected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_response_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_response_answers_answer_options_answer_option_id",
                        column: x => x.answer_option_id,
                        principalTable: "answer_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_response_answers_user_responses_user_response_id",
                        column: x => x.user_response_id,
                        principalTable: "user_responses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_answer_options_is_correct",
                table: "answer_options",
                column: "is_correct");

            migrationBuilder.CreateIndex(
                name: "ix_answer_options_question_id_order_index",
                table: "answer_options",
                columns: new[] { "question_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "ix_questions_quiz_id_order_index",
                table: "questions",
                columns: new[] { "quiz_id", "order_index" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_completed_at",
                table: "quiz_attempts",
                column: "completed_at");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_is_completed",
                table: "quiz_attempts",
                column: "is_completed");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_quiz_id",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_started_at",
                table: "quiz_attempts",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id_quiz_id",
                table: "quiz_attempts",
                columns: new[] { "user_id", "quiz_id" });

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_created_at",
                table: "quizzes",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_is_active",
                table: "quizzes",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_title",
                table: "quizzes",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_user_response_answers_answer_option_id",
                table: "user_response_answers",
                column: "answer_option_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_response_answers_user_response_id_answer_option_id",
                table: "user_response_answers",
                columns: new[] { "user_response_id", "answer_option_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_responses_answered_at",
                table: "user_responses",
                column: "answered_at");

            migrationBuilder.CreateIndex(
                name: "ix_user_responses_is_correct",
                table: "user_responses",
                column: "is_correct");

            migrationBuilder.CreateIndex(
                name: "ix_user_responses_question_id",
                table: "user_responses",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_responses_quiz_attempt_id_question_id",
                table: "user_responses",
                columns: new[] { "quiz_attempt_id", "question_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_response_answers");

            migrationBuilder.DropTable(
                name: "answer_options");

            migrationBuilder.DropTable(
                name: "user_responses");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "quizzes");
        }
    }
}
