using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents certificates issued to students upon course completion
/// </summary>
[Table("course_certificates")]
[Index(nameof(CertificateNumber), IsUnique = true)]
[Index(nameof(EnrollmentId), IsUnique = true)]
[Index(nameof(UserId), nameof(CourseId))]
[Index(nameof(IssuedAt))]
public class CourseCertificate
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(50)]
  [Column("certificate_number")]
  public string CertificateNumber { get; set; } = string.Empty; // Unique identifier like "CERT-2024-00001"

  [Required]
  [Column("enrollment_id")]
  public int EnrollmentId { get; set; }

  [Required]
  [Column("course_id")]
  public int CourseId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("user_id")]
  public string UserId { get; set; } = string.Empty;

  [Required]
  [MaxLength(200)]
  [Column("student_name")]
  public string StudentName { get; set; } = string.Empty;

  [Required]
  [MaxLength(200)]
  [Column("course_title")]
  public string CourseTitle { get; set; } = string.Empty;

  [Column("completion_date")]
  public DateTime CompletionDate { get; set; }

  [Column("issued_at")]
  public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

  [MaxLength(500)]
  [Column("certificate_url")]
  public string? CertificateUrl { get; set; }

  [MaxLength(500)]
  [Column("certificate_pdf_blob_name")]
  public string? CertificatePdfBlobName { get; set; }

  [Column("is_valid")]
  public bool IsValid { get; set; } = true;

  [Column("revoked_at")]
  public DateTime? RevokedAt { get; set; }

  [MaxLength(500)]
  [Column("revocation_reason")]
  public string? RevocationReason { get; set; }

  [MaxLength(450)]
  [Column("revoked_by_user_id")]
  public string? RevokedByUserId { get; set; }

  // Verification
  [MaxLength(100)]
  [Column("verification_code")]
  public string? VerificationCode { get; set; }

  [Column("verified_count")]
  public int VerifiedCount { get; set; }

  [Column("last_verified_at")]
  public DateTime? LastVerifiedAt { get; set; }

  // Certificate details
  [Column("course_duration_hours")]
  public int? CourseDurationHours { get; set; }

  [MaxLength(100)]
  [Column("grade")]
  public string? Grade { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("updated_at")]
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
