using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a module/section within a course
/// </summary>
[Table("CourseModules")]
[Index(nameof(CourseId), nameof(OrderIndex))]
public class CourseModule
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("CourseId")]
  public int CourseId { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("Title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000)]
  [Column("Description")]
  public string? Description { get; set; }

  [Column("OrderIndex")]
  public int OrderIndex { get; set; } = 0;

  [Column("EstimatedDurationMinutes")]
  public int? EstimatedDurationMinutes { get; set; }

  [Column("IsPublished")]
  public bool IsPublished { get; set; } = true;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course Course { get; set; } = null!;

  public ICollection<CourseContent> Contents { get; set; } = new List<CourseContent>();
  public ICollection<CourseModuleProgress> ModuleProgress { get; set; } = new List<CourseModuleProgress>();
}
