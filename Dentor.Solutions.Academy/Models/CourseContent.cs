using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents individual content items within a course module (videos, documents, etc.)
/// Content files are stored in Azure Blob Storage with Shared Key authentication
/// </summary>
[Table("CourseContents")]
[Index(nameof(CourseModuleId), nameof(OrderIndex))]
[Index(nameof(ContentType))]
public class CourseContent
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("CourseModuleId")]
  public int CourseModuleId { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("Title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  [Column("Description")]
  public string? Description { get; set; }

  [Required]
  [MaxLength(50)]
  [Column("ContentType")]
  public string ContentType { get; set; } = string.Empty; // Video, Document, PDF, Image, Audio, Quiz, etc.

  [Column("OrderIndex")]
  public int OrderIndex { get; set; } = 0;

  [Column("DurationMinutes")]
  public int? DurationMinutes { get; set; }

  // Azure Blob Storage properties
  [MaxLength(500)]
  [Column("BlobContainerName")]
  public string? BlobContainerName { get; set; }

  [MaxLength(500)]
  [Column("BlobName")]
  public string? BlobName { get; set; }

  [MaxLength(1000)]
  [Column("BlobUrl")]
  public string? BlobUrl { get; set; }

  [Column("FileSizeBytes")]
  public long? FileSizeBytes { get; set; }

  [MaxLength(100)]
  [Column("MimeType")]
  public string? MimeType { get; set; }

  // For quiz content type
  [Column("QuizId")]
  public int? QuizId { get; set; }

  // For external content (YouTube, Vimeo, etc.)
  [MaxLength(1000)]
  [Column("ExternalUrl")]
  public string? ExternalUrl { get; set; }

  [Column("IsFreePreview")]
  public bool IsFreePreview { get; set; } = false;

  [Column("IsDownloadable")]
  public bool IsDownloadable { get; set; } = false;

  [Column("IsMandatory")]
  public bool IsMandatory { get; set; } = false; // If true, student must complete before continuing

  [Column("IsPublished")]
  public bool IsPublished { get; set; } = true;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("CourseModuleId")]
  public CourseModule CourseModule { get; set; } = null!;

  [ForeignKey("QuizId")]
  public Quiz? Quiz { get; set; }

  public ICollection<CourseProgress> Progress { get; set; } = new List<CourseProgress>();
}
