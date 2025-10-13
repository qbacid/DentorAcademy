using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Tracks student progress through individual course content items
/// </summary>
[Table("course_progress")]
[Index(nameof(EnrollmentId), nameof(CourseContentId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseContentId))]
[Index(nameof(CompletedAt))]
public class CourseProgress
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [Required]
    [Column("course_content_id")]
    public int CourseContentId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    // For quiz content - track if student passed the quiz
    [Column("quiz_passed")]
    public bool? QuizPassed { get; set; }

    [Column("quiz_score")]
    [Precision(5, 2)]
    public decimal? QuizScore { get; set; }

    [Column("quiz_attempt_id")]
    public int? QuizAttemptId { get; set; }

    [Column("progress_percentage")]
    [Precision(5, 2)]
    public decimal ProgressPercentage { get; set; } = 0; // For video playback progress

    [Column("time_spent_seconds")]
    public int TimeSpentSeconds { get; set; } = 0;

    [Column("last_position_seconds")]
    public int? LastPositionSeconds { get; set; } // For resuming videos

    [Column("first_accessed_at")]
    public DateTime? FirstAccessedAt { get; set; }

    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("access_count")]
    public int AccessCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EnrollmentId")]
    public CourseEnrollment Enrollment { get; set; } = null!;

    [ForeignKey("CourseContentId")]
    public CourseContent CourseContent { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;
}
