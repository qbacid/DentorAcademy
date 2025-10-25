using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a student's enrollment in a course
/// </summary>
[Table("CourseEnrollments")]
[Index(nameof(UserId), nameof(CourseId), IsUnique = true)]
[Index(nameof(EnrolledAt))]
[Index(nameof(Status))]
public class CourseEnrollment
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("CourseId")]
  public int CourseId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty;

  [Column("EnrolledAt")]
  public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

  [Column("Status")]
  [MaxLength(50)]
  public string Status { get; set; } = "Active"; // Active, Completed, Dropped, Expired

  [Column("ProgressPercentage")]
  [Precision(5, 2)]
  public decimal ProgressPercentage { get; set; } = 0;

  [Column("LastAccessedAt")]
  public DateTime? LastAccessedAt { get; set; }

  [Column("CompletedAt")]
  public DateTime? CompletedAt { get; set; }

  [Column("CertificateIssued")]
  public bool CertificateIssued { get; set; } = false;

  [Column("CertificateIssuedAt")]
  public DateTime? CertificateIssuedAt { get; set; }

  [MaxLength(500)]
  [Column("CertificateUrl")]
  public string? CertificateUrl { get; set; }

  // Payment tracking (for paid courses)
  [Column("PaymentAmount")]
  [Precision(10, 2)]
  public decimal? PaymentAmount { get; set; }

  [MaxLength(100)]
  [Column("PaymentTransactionId")]
  public string? PaymentTransactionId { get; set; }

  [Column("PaymentDate")]
  public DateTime? PaymentDate { get; set; }

  [Column("ExpiresAt")]
  public DateTime? ExpiresAt { get; set; } // For time-limited access

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course Course { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;

  public ICollection<CourseProgress> ContentProgress { get; set; } = new List<CourseProgress>();
  public ICollection<CourseModuleProgress> ModuleProgress { get; set; } = new List<CourseModuleProgress>();
}
