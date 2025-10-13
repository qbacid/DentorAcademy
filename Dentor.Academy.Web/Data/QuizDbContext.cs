using Microsoft.EntityFrameworkCore;
using Dentor.Academy.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Dentor.Academy.Web.Data;

/// <summary>
/// Database context for the Quiz system optimized for PostgreSQL
/// </summary>
public class QuizDbContext : IdentityDbContext<ApplicationUser>
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
    }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<UserResponse> UserResponses { get; set; }
    public DbSet<UserResponseAnswer> UserResponseAnswers { get; set; }

    // Course Platform Entities
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseCategory> CourseCategories { get; set; }
    public DbSet<CourseModule> CourseModules { get; set; }
    public DbSet<CourseContent> CourseContents { get; set; }
    public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    public DbSet<CourseProgress> CourseProgress { get; set; }
    public DbSet<CourseModuleProgress> CourseModuleProgress { get; set; }
    public DbSet<CourseReview> CourseReviews { get; set; }
    public DbSet<CourseInstructor> CourseInstructors { get; set; }
    public DbSet<CourseCertificate> CourseCertificates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Quiz entity
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);

            entity.Property(e => e.PassingScore)
                .HasPrecision(5, 2); // Max 999.99

            entity.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(q => q.QuizAttempts)
                .WithOne(qa => qa.Quiz)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve attempt history
        });

        // Configure Question entity
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasIndex(e => new { e.QuizId, e.OrderIndex });

            entity.Property(e => e.Points)
                .HasPrecision(5, 2);

            entity.HasMany(q => q.AnswerOptions)
                .WithOne(ao => ao.Question)
                .HasForeignKey(ao => ao.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(q => q.UserResponses)
                .WithOne(ur => ur.Question)
                .HasForeignKey(ur => ur.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve response history
        });

        // Configure AnswerOption entity
        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasIndex(e => new { e.QuestionId, e.OrderIndex });
            entity.HasIndex(e => e.IsCorrect);

            entity.HasMany(ao => ao.UserResponseAnswers)
                .WithOne(ura => ura.AnswerOption)
                .HasForeignKey(ura => ura.AnswerOptionId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve response history
        });

        // Configure QuizAttempt entity
        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.QuizId });
            entity.HasIndex(e => e.StartedAt);
            entity.HasIndex(e => e.CompletedAt);
            entity.HasIndex(e => e.IsCompleted);

            entity.Property(e => e.Score)
                .HasPrecision(5, 2);

            entity.Property(e => e.TotalPointsEarned)
                .HasPrecision(10, 2);

            entity.Property(e => e.TotalPointsPossible)
                .HasPrecision(10, 2);

            entity.HasMany(qa => qa.UserResponses)
                .WithOne(ur => ur.QuizAttempt)
                .HasForeignKey(ur => ur.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserResponse entity
        modelBuilder.Entity<UserResponse>(entity =>
        {
            entity.HasIndex(e => new { e.QuizAttemptId, e.QuestionId })
                .IsUnique(); // One response per question per attempt

            entity.HasIndex(e => e.IsCorrect);
            entity.HasIndex(e => e.AnsweredAt);

            entity.Property(e => e.PointsEarned)
                .HasPrecision(5, 2);

            entity.HasMany(ur => ur.UserResponseAnswers)
                .WithOne(ura => ura.UserResponse)
                .HasForeignKey(ura => ura.UserResponseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserResponseAnswer entity (junction table)
        modelBuilder.Entity<UserResponseAnswer>(entity =>
        {
            entity.HasIndex(e => new { e.UserResponseId, e.AnswerOptionId })
                .IsUnique(); // Prevent duplicate selections
        });

        // Configure Course entity
        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.Price)
                .HasPrecision(10, 2);

            entity.HasMany(c => c.Modules)
                .WithOne(cm => cm.Course)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Enrollments)
                .WithOne(ce => ce.Course)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve enrollment history
        });

        // Configure CourseCategory entity
        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .IsUnique(); // Prevent duplicate category names

            entity.HasMany(cc => cc.Courses)
                .WithOne(c => c.CourseCategory)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve course history
        });

        // Configure CourseModule entity
        modelBuilder.Entity<CourseModule>(entity =>
        {
            entity.HasMany(cm => cm.Contents)
                .WithOne(cc => cc.CourseModule)
                .HasForeignKey(cc => cc.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(cm => cm.ModuleProgress)
                .WithOne(cmp => cmp.CourseModule)
                .HasForeignKey(cmp => cmp.CourseModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CourseContent entity
        modelBuilder.Entity<CourseContent>(entity =>
        {
            entity.HasMany(cc => cc.Progress)
                .WithOne(cp => cp.CourseContent)
                .HasForeignKey(cp => cp.CourseContentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CourseEnrollment entity
        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2);

            entity.Property(e => e.PaymentAmount)
                .HasPrecision(10, 2);

            entity.HasMany(ce => ce.ContentProgress)
                .WithOne(cp => cp.Enrollment)
                .HasForeignKey(cp => cp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(ce => ce.ModuleProgress)
                .WithOne(cmp => cmp.Enrollment)
                .HasForeignKey(cmp => cmp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CourseProgress entity
        modelBuilder.Entity<CourseProgress>(entity =>
        {
            entity.Property(e => e.QuizScore)
                .HasPrecision(5, 2);

            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2);
        });

        // Configure CourseModuleProgress entity
        modelBuilder.Entity<CourseModuleProgress>(entity =>
        {
            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2);
        });

        // Configure CourseReview entity
        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.Property(e => e.Rating)
                .HasPrecision(3, 2); // Max 99.9

            entity.Property(e => e.ReviewText)
                .HasMaxLength(2000); // Limit review text length

            entity.HasOne(cr => cr.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cr => cr.User)
                .WithMany(u => u.CourseReviews)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CourseInstructor entity
        modelBuilder.Entity<CourseInstructor>(entity =>
        {
            entity.HasOne(ci => ci.Course)
                .WithMany(c => c.Instructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.User)
                .WithMany(u => u.CourseInstructors)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CourseCertificate entity
        modelBuilder.Entity<CourseCertificate>(entity =>
        {
            entity.Property(e => e.CertificateUrl)
                .HasMaxLength(500); // Limit URL length

            entity.HasOne(cc => cc.Course)
                .WithMany(c => c.Certificates)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cc => cc.User)
                .WithMany(u => u.CourseCertificates)
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
