using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a module/section within a course
/// </summary>
[Table("course_modules")]
[Index(nameof(CourseId), nameof(OrderIndex))]
public class CourseModule
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("course_id")]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("order_index")]
    public int OrderIndex { get; set; } = 0;

    [Column("estimated_duration_minutes")]
    public int? EstimatedDurationMinutes { get; set; }

    [Column("is_published")]
    public bool IsPublished { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    public ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
    public ICollection<CourseModuleProgress> ModuleProgress { get; set; } = new List<CourseModuleProgress>();
}

