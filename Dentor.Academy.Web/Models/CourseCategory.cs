using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a course category for organizing courses
/// </summary>
[Table("course_categories")]
[Index(nameof(Name), IsUnique = true)]
[Index(nameof(DisplayOrder))]
[Index(nameof(IsActive))]
public class CourseCategory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(50)]
    [Column("icon_class")]
    public string? IconClass { get; set; }

    [MaxLength(20)]
    [Column("color")]
    public string? Color { get; set; }

    [Column("display_order")]
    public int DisplayOrder { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
