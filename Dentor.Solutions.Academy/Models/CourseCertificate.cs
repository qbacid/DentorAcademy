using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents certificates issued to students upon course completion
/// </summary>
[Table("CourseCertificates")]
[Index(nameof(CertificateNumber), IsUnique = true)]
[Index(nameof(EnrollmentId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseId))]
[Index(nameof(IssuedAt))]
public class CourseCertificate
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(50)]
  [Column("CertificateNumber")]
  public string CertificateNumber { get; set; } = string.Empty; // Unique identifier like "CERT-2024-00001"

  [Required]
  [Column("EnrollmentId")]
  public int EnrollmentId { get; set; }

  [Required]
  [Column("CourseId")]
  public int CourseId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty;

  [Required]
  [MaxLength(200)]
  [Column("StudentName")]
  public string StudentName { get; set; } = string.Empty;

  [Required]
  [MaxLength(200)]
  [Column("CourseTitle")]
  public string CourseTitle { get; set; } = string.Empty;

  [Column("CompletionDate")]
  public DateTime CompletionDate { get; set; }

  [Column("IssuedAt")]
  public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

  [MaxLength(500)]
  [Column("CertificateUrl")]
  public string? CertificateUrl { get; set; }

  [MaxLength(500)]
  [Column("CertificatePdfBlobName")]
  public string? CertificatePdfBlobName { get; set; }

  [Column("IsValid")]
  public bool IsValid { get; set; } = true;

  [Column("RevokedAt")]
  public DateTime? RevokedAt { get; set; }

  [MaxLength(500)]
  [Column("RevocationReason")]
  public string? RevocationReason { get; set; }

  [MaxLength(450)]
  [Column("RevokedByUserId")]
  public string? RevokedByUserId { get; set; }

  // Verification
  [MaxLength(100)]
  [Column("VerificationCode")]
  public string? VerificationCode { get; set; }

  [Column("VerifiedCount")]
  public int VerifiedCount { get; set; }

  [Column("LastVerifiedAt")]
  public DateTime? LastVerifiedAt { get; set; }

  // Certificate details
  [Column("CourseDurationHours")]
  public int? CourseDurationHours { get; set; }

  [MaxLength(100)]
  [Column("Grade")]
  public string? Grade { get; set; }

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("EnrollmentId")]
  public CourseEnrollment Enrollment { get; set; } = null!;

  [ForeignKey("CourseId")]
  public Course Course { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;

  [ForeignKey("RevokedByUserId")]
  public ApplicationUser? RevokedBy { get; set; }
}
