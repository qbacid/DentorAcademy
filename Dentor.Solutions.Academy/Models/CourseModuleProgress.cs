using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Tracks student completion of entire course modules
/// </summary>
[Table("CourseModuleProgress")]
[Index(nameof(EnrollmentId), nameof(CourseModuleId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseModuleId))]
public class CourseModuleProgress
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("EnrollmentId")]
  public int EnrollmentId { get; set; }

  [Required]
  [Column("CourseModuleId")]
  public int CourseModuleId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty;

  [Column("IsCompleted")]
  public bool IsCompleted { get; set; } = false;

  [Column("CompletedAt")]
  public DateTime? CompletedAt { get; set; }

  [Column("ProgressPercentage")]
  [Precision(5, 2)]
  public decimal ProgressPercentage { get; set; } = 0;

  [Column("FirstAccessedAt")]
  public DateTime? FirstAccessedAt { get; set; }

  [Column("LastAccessedAt")]
  public DateTime? LastAccessedAt { get; set; }

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("EnrollmentId")]
  public CourseEnrollment Enrollment { get; set; } = null!;

  [ForeignKey("CourseModuleId")]
  public CourseModule CourseModule { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;
}
