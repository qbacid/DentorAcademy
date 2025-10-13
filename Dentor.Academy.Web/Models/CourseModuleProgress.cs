using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Tracks student completion of entire course modules
/// </summary>
[Table("course_module_progress")]
[Index(nameof(EnrollmentId), nameof(CourseModuleId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseModuleId))]
public class CourseModuleProgress
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("enrollment_id")]
    public int EnrollmentId { get; set; }

    [Required]
    [Column("course_module_id")]
    public int CourseModuleId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("progress_percentage")]
    [Precision(5, 2)]
    public decimal ProgressPercentage { get; set; } = 0;

    [Column("first_accessed_at")]
    public DateTime? FirstAccessedAt { get; set; }

    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EnrollmentId")]
    public CourseEnrollment Enrollment { get; set; } = null!;

    [ForeignKey("CourseModuleId")]
    public CourseModule CourseModule { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;
}

