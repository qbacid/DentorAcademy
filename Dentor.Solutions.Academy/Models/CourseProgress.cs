using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Tracks student progress through individual course content items
/// </summary>
[Table("CourseProgress")]
[Index(nameof(EnrollmentId), nameof(CourseContentId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseContentId))]
[Index(nameof(CompletedAt))]
public class CourseProgress
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("EnrollmentId")]
  public int EnrollmentId { get; set; }

  [Required]
  [Column("CourseContentId")]
  public int CourseContentId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty;

  [Column("IsCompleted")]
  public bool IsCompleted { get; set; } = false;

  [Column("CompletedAt")]
  public DateTime? CompletedAt { get; set; }

  // For quiz content - track if student passed the quiz
  [Column("QuizPassed")]
  public bool? QuizPassed { get; set; }

  [Column("QuizScore")]
  [Precision(5, 2)]
  public decimal? QuizScore { get; set; }

  [Column("QuizAttemptId")]
  public int? QuizAttemptId { get; set; }

  [Column("ProgressPercentage")]
  [Precision(5, 2)]
  public decimal ProgressPercentage { get; set; } = 0; // For video playback progress

  [Column("TimeSpentSeconds")]
  public int TimeSpentSeconds { get; set; } = 0;

  [Column("LastPositionSeconds")]
  public int? LastPositionSeconds { get; set; } // For resuming videos

  [Column("FirstAccessedAt")]
  public DateTime? FirstAccessedAt { get; set; }

  [Column("LastAccessedAt")]
  public DateTime? LastAccessedAt { get; set; }

  [Column("AccessCount")]
  public int AccessCount { get; set; } = 0;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("EnrollmentId")]
  public CourseEnrollment Enrollment { get; set; } = null!;

  [ForeignKey("CourseContentId")]
  public CourseContent CourseContent { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;
}
