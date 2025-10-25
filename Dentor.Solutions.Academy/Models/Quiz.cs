using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a Quiz that contains multiple questions
/// </summary>
[Table("Quizzes")]
public class Quiz
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("Title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  [Column("Description")]
  public string? Description { get; set; }

  [MaxLength(100)]
  [Column("Category")]
  public string? Category { get; set; }

  [Column("PassingScore")]
  public decimal PassingScore { get; set; } = 70.0m; // Percentage

  [Column("TimeLimitMinutes")]
  public int? TimeLimitMinutes { get; set; }

  [Column("IsActive")]
  public bool IsActive { get; set; } = true;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Course relationship (optional - quiz can be standalone or part of a course)
  [Column("CourseId")]
  public int? CourseId { get; set; }

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course? Course { get; set; }

  public ICollection<Question> Questions { get; set; } = new List<Question>();
  public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
  public ICollection<CourseContent> CourseContents { get; set; } = new List<CourseContent>();
}
