using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a student's enrollment in a course
/// </summary>
[Table("course_enrollments")]
[Index(nameof(UserId), nameof(CourseId), IsUnique = true)]
[Index(nameof(EnrolledAt))]
[Index(nameof(Status))]
public class CourseEnrollment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("course_id")]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("enrolled_at")]
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Completed, Dropped, Expired

    [Column("progress_percentage")]
    [Precision(5, 2)]
    public decimal ProgressPercentage { get; set; } = 0;

    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("certificate_issued")]
    public bool CertificateIssued { get; set; } = false;

    [Column("certificate_issued_at")]
    public DateTime? CertificateIssuedAt { get; set; }

    [MaxLength(500)]
    [Column("certificate_url")]
    public string? CertificateUrl { get; set; }

    // Payment tracking (for paid courses)
    [Column("payment_amount")]
    [Precision(10, 2)]
    public decimal? PaymentAmount { get; set; }

    [MaxLength(100)]
    [Column("payment_transaction_id")]
    public string? PaymentTransactionId { get; set; }

    [Column("payment_date")]
    public DateTime? PaymentDate { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; } // For time-limited access

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    public ICollection<CourseProgress> ContentProgress { get; set; } = new List<CourseProgress>();
    public ICollection<CourseModuleProgress> ModuleProgress { get; set; } = new List<CourseModuleProgress>();
}

