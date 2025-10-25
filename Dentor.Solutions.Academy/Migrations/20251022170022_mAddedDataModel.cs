using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dentor.Solutions.Academy.Migrations
{
    /// <inheritdoc />
    public partial class mAddedDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IconClass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FullDescription = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ThumbnailImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    ThumbnailContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CoverImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    CoverImageContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DifficultyLevel = table.Column<string>(type: "text", nullable: false),
                    EstimatedDurationHours = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    LearningObjectives = table.Column<string>(type: "text", nullable: true),
                    Prerequisites = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_CourseCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CourseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificateIssued = table.Column<bool>(type: "boolean", nullable: false),
                    CertificateIssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificateUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseInstructors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInstructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseInstructors_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseInstructors_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseModules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Rating = table.Column<int>(type: "integer", precision: 3, scale: 2, nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReviewText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    HelpfulCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseReviews_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseReviews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PassingScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    StudentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CourseTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CertificatePdfBlobName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevocationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RevokedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    VerificationCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VerifiedCount = table.Column<int>(type: "integer", nullable: false),
                    LastVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CourseDurationHours = table.Column<int>(type: "integer", nullable: true),
                    Grade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCertificates_AspNetUsers_RevokedByUserId",
                        column: x => x.RevokedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseCertificates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCertificates_CourseEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "CourseEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCertificates_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseModuleProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FirstAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModuleProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseModuleProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseModuleProgress_CourseEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "CourseEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseModuleProgress_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseModuleId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: true),
                    BlobContainerName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BlobName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BlobUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuizId = table.Column<int>(type: "integer", nullable: true),
                    ExternalUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsFreePreview = table.Column<bool>(type: "boolean", nullable: false),
                    IsDownloadable = table.Column<bool>(type: "boolean", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseContents_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseContents_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    Explanation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExplanationImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    QuestionImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    QuestionAudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Points = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    TotalPointsEarned = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TotalPointsPossible = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Passed = table.Column<bool>(type: "boolean", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    CourseContentId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QuizPassed = table.Column<bool>(type: "boolean", nullable: true),
                    QuizScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    QuizAttemptId = table.Column<int>(type: "integer", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "integer", nullable: false),
                    LastPositionSeconds = table.Column<int>(type: "integer", nullable: true),
                    FirstAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AccessCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseProgress_CourseContents_CourseContentId",
                        column: x => x.CourseContentId,
                        principalTable: "CourseContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseProgress_CourseEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "CourseEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    OptionText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizAttemptId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    PointsEarned = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TextAnswer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserResponses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserResponses_QuizAttempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResponseAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserResponseId = table.Column<int>(type: "integer", nullable: false),
                    AnswerOptionId = table.Column<int>(type: "integer", nullable: false),
                    SelectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponseAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserResponseAnswers_AnswerOptions_AnswerOptionId",
                        column: x => x.AnswerOptionId,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserResponseAnswers_UserResponses_UserResponseId",
                        column: x => x.UserResponseId,
                        principalTable: "UserResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_IsCorrect",
                table: "AnswerOptions",
                column: "IsCorrect");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId_OrderIndex",
                table: "AnswerOptions",
                columns: new[] { "QuestionId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_DisplayOrder",
                table: "CourseCategories",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_IsActive",
                table: "CourseCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_Name",
                table: "CourseCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_CertificateNumber",
                table: "CourseCertificates",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_CourseId",
                table: "CourseCertificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_EnrollmentId",
                table: "CourseCertificates",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_IssuedAt",
                table: "CourseCertificates",
                column: "IssuedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_RevokedByUserId",
                table: "CourseCertificates",
                column: "RevokedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_UserId_CourseId",
                table: "CourseCertificates",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseContents_ContentType",
                table: "CourseContents",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_CourseContents_CourseModuleId_OrderIndex",
                table: "CourseContents",
                columns: new[] { "CourseModuleId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseContents_QuizId",
                table: "CourseContents",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId",
                table: "CourseEnrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_EnrolledAt",
                table: "CourseEnrollments",
                column: "EnrolledAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_Status",
                table: "CourseEnrollments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_UserId_CourseId",
                table: "CourseEnrollments",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructors_CourseId_UserId",
                table: "CourseInstructors",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructors_UserId",
                table: "CourseInstructors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModuleProgress_CourseModuleId",
                table: "CourseModuleProgress",
                column: "CourseModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModuleProgress_EnrollmentId_CourseModuleId",
                table: "CourseModuleProgress",
                columns: new[] { "EnrollmentId", "CourseModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseModuleProgress_UserId_CourseModuleId",
                table: "CourseModuleProgress",
                columns: new[] { "UserId", "CourseModuleId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId_OrderIndex",
                table: "CourseModules",
                columns: new[] { "CourseId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_CompletedAt",
                table: "CourseProgress",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_CourseContentId",
                table: "CourseProgress",
                column: "CourseContentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_EnrollmentId_CourseContentId",
                table: "CourseProgress",
                columns: new[] { "EnrollmentId", "CourseContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_UserId_CourseContentId",
                table: "CourseProgress",
                columns: new[] { "UserId", "CourseContentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_ApprovedByUserId",
                table: "CourseReviews",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_CourseId_Rating",
                table: "CourseReviews",
                columns: new[] { "CourseId", "Rating" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_CourseId_UserId",
                table: "CourseReviews",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_CreatedAt",
                table: "CourseReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_IsApproved",
                table: "CourseReviews",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_UserId",
                table: "CourseReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Category",
                table: "Courses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedAt",
                table: "Courses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedByUserId",
                table: "Courses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsPublished",
                table: "Courses",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId_OrderIndex",
                table: "Questions",
                columns: new[] { "QuizId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_CompletedAt",
                table: "QuizAttempts",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_IsCompleted",
                table: "QuizAttempts",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId",
                table: "QuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StartedAt",
                table: "QuizAttempts",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_QuizId",
                table: "QuizAttempts",
                columns: new[] { "UserId", "QuizId" });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatedAt",
                table: "Quizzes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_IsActive",
                table: "Quizzes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Title",
                table: "Quizzes",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponseAnswers_AnswerOptionId",
                table: "UserResponseAnswers",
                column: "AnswerOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponseAnswers_UserResponseId_AnswerOptionId",
                table: "UserResponseAnswers",
                columns: new[] { "UserResponseId", "AnswerOptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserResponses_AnsweredAt",
                table: "UserResponses",
                column: "AnsweredAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponses_IsCorrect",
                table: "UserResponses",
                column: "IsCorrect");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponses_QuestionId",
                table: "UserResponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponses_QuizAttemptId_QuestionId",
                table: "UserResponses",
                columns: new[] { "QuizAttemptId", "QuestionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCertificates");

            migrationBuilder.DropTable(
                name: "CourseInstructors");

            migrationBuilder.DropTable(
                name: "CourseModuleProgress");

            migrationBuilder.DropTable(
                name: "CourseProgress");

            migrationBuilder.DropTable(
                name: "CourseReviews");

            migrationBuilder.DropTable(
                name: "UserResponseAnswers");

            migrationBuilder.DropTable(
                name: "CourseContents");

            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropTable(
                name: "UserResponses");

            migrationBuilder.DropTable(
                name: "CourseModules");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "CourseCategories");
        }
    }
}
