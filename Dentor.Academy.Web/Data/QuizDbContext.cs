using Microsoft.EntityFrameworkCore;
using Dentor.Academy.Web.Models;

namespace Dentor.Academy.Web.Data;

/// <summary>
/// Database context for the Quiz system optimized for PostgreSQL
/// </summary>
public class QuizDbContext : DbContext
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
    }
}
