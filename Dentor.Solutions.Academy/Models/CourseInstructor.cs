using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Junction table linking instructors to courses (many-to-many)
/// Allows multiple instructors per course
/// </summary>
[Table("CourseInstructors")]
[Index(nameof(CourseId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public class CourseInstructor
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("CourseId")]
  public int CourseId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty;

  [Required]
  [MaxLength(50)]
  [Column("Role")]
  public string Role { get; set; } = "Instructor"; // Lead Instructor, Co-Instructor, Teaching Assistant

  [Column("OrderIndex")]
  public int OrderIndex { get; set; } = 0;

  [Column("AssignedAt")]
  public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

  [Column("IsActive")]
  public bool IsActive { get; set; } = true;

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course Course { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;
}
