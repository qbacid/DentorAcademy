using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a course category for organizing courses
/// </summary>
[Table("CourseCategories")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(DisplayOrder))]
[Index(nameof(IsActive))]
public class CourseCategory
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(100)]
  [Column("Name")]
  public string Name { get; set; } = string.Empty;

  [MaxLength(500)]
  [Column("Description")]
  public string? Description { get; set; }

  [MaxLength(50)]
  [Column("IconClass")]
  public string? IconClass { get; set; }

  [MaxLength(20)]
  [Column("Color")]
  public string? Color { get; set; }

  [Column("DisplayOrder")]
  public int DisplayOrder { get; set; }

  [Column("IsActive")]
  public bool IsActive { get; set; } = true;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime? UpdatedAt { get; set; }

  // Navigation properties
  public ICollection<Course> Courses { get; set; } = new List<Course>();
}
