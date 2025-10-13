using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents individual content items within a course module (videos, documents, etc.)
/// Content files are stored in Azure Blob Storage with Shared Key authentication
/// </summary>
[Table("course_contents")]
[Index(nameof(CourseModuleId), nameof(OrderIndex))]
[Index(nameof(ContentType))]
public class CourseContent
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("course_module_id")]
    public int CourseModuleId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("content_type")]
    public string ContentType { get; set; } = string.Empty; // Video, Document, PDF, Image, Audio, Quiz, etc.

    [Column("order_index")]
    public int OrderIndex { get; set; } = 0;

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    // Azure Blob Storage properties
    [MaxLength(500)]
    [Column("blob_container_name")]
    public string? BlobContainerName { get; set; }

    [MaxLength(500)]
    [Column("blob_name")]
    public string? BlobName { get; set; }

    [MaxLength(1000)]
    [Column("blob_url")]
    public string? BlobUrl { get; set; }

    [Column("file_size_bytes")]
    public long? FileSizeBytes { get; set; }

    [MaxLength(100)]
    [Column("mime_type")]
    public string? MimeType { get; set; }

    // For quiz content type
    [Column("quiz_id")]
    public int? QuizId { get; set; }

    // For external content (YouTube, Vimeo, etc.)
    [MaxLength(1000)]
    [Column("external_url")]
    public string? ExternalUrl { get; set; }

    [Column("is_free_preview")]
    public bool IsFreePreview { get; set; } = false;

    [Column("is_downloadable")]
    public bool IsDownloadable { get; set; } = false;

    [Column("is_mandatory")]
    public bool IsMandatory { get; set; } = false; // If true, student must complete before continuing

    [Column("is_published")]
    public bool IsPublished { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseModuleId")]
    public CourseModule CourseModule { get; set; } = null!;

    [ForeignKey("QuizId")]
    public Quiz? Quiz { get; set; }

    public ICollection<CourseProgress> Progress { get; set; } = new List<CourseProgress>();
}
