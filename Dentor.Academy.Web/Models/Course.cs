using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a course that students can enroll in
/// </summary>
[Table("courses")]
[Index(nameof(Category))]
[Index(nameof(IsPublished))]
[Index(nameof(CreatedAt))]
public class Course
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(500)]
  [Column("short_description")]
  public string? ShortDescription { get; set; }

  [Column("full_description")]
  public string? FullDescription { get; set; }

  [MaxLength(100)]
  [Column("category")]
  public string? Category { get; set; }

  [Column("category_id")]
  public int? CategoryId { get; set; }

  [MaxLength(500)]
  [Column("thumbnail_url")]
  public string? ThumbnailUrl { get; set; }

  [MaxLength(500)]
  [Column("cover_image_url")]
  public string? CoverImageUrl { get; set; }

  [Column("difficulty_level")]
  public string DifficultyLevel { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced

  [Column("estimated_duration_hours")]
  public int? EstimatedDurationHours { get; set; }

  [Column("price")]
  [Precision(10, 2)]
  public decimal Price { get; set; } = 0; // 0 for free courses

  [Column("is_published")]
  public bool IsPublished { get; set; } = false;

  [Column("is_featured")]
  public bool IsFeatured { get; set; } = false;

  [MaxLength(450)]
  [Column("created_by_user_id")]
  public string? CreatedByUserId { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("updated_at")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  [Column("published_at")]
  public DateTime? PublishedAt { get; set; }

  // Additional course metadata
  [Column("tags")]
  public string? Tags { get; set; } // Comma-separated tags

  [Column("learning_objectives")]
  public string? LearningObjectives { get; set; } // Comma-separated objectives

  [Column("prerequisites")]
  public string? Prerequisites { get; set; } // Comma-separated prerequisites

  // Navigation properties
  [ForeignKey("CreatedByUserId")]
  public ApplicationUser? CreatedBy { get; set; }

  [ForeignKey("CategoryId")]
  public CourseCategory? CourseCategory { get; set; }

  public ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();
  public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
  public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
  public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
  public ICollection<CourseInstructor> Instructors { get; set; } = new List<CourseInstructor>();
  public ICollection<CourseCertificate> Certificates { get; set; } = new List<CourseCertificate>();
}
