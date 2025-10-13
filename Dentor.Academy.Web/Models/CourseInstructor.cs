using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Junction table linking instructors to courses (many-to-many)
/// Allows multiple instructors per course
/// </summary>
[Table("course_instructors")]
[Index(nameof(CourseId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class CourseInstructor
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

    [Required]
    [MaxLength(50)]
    [Column("role")]
    public string Role { get; set; } = "Instructor"; // Lead Instructor, Co-Instructor, Teaching Assistant

    [Column("order_index")]
    public int OrderIndex { get; set; } = 0;

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;
}

