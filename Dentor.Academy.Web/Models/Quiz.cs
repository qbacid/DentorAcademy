using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a Quiz that contains multiple questions
/// </summary>
[Table("quizzes")]
public class Quiz
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  [Column("description")]
  public string? Description { get; set; }

  [MaxLength(100)]
  [Column("category")]
  public string? Category { get; set; }

  [Column("passing_score")]
  public decimal PassingScore { get; set; } = 70.0m; // Percentage

  [Column("time_limit_minutes")]
  public int? TimeLimitMinutes { get; set; }

  [Column("is_active")]
  public bool IsActive { get; set; } = true;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Course relationship (optional - quiz can be standalone or part of a course)
  [Column("course_id")]
  public int? CourseId { get; set; }

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course? Course { get; set; }

  public ICollection<Question> Questions { get; set; } = new List<Question>();
  public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
  public ICollection<CourseContent> CourseContents { get; set; } = new List<CourseContent>();
}
