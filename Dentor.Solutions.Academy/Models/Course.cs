using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a course that students can enroll in
/// </summary>
[Table("Courses")]
[Index(nameof(Category))]
[Index(nameof(IsPublished))]
[Index(nameof(CreatedAt))]
public class Course
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [MaxLength(200)]
  [Column("Title")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(500)]
  [Column("ShortDescription")]
  public string? ShortDescription { get; set; }

  [Column("FullDescription")]
  public string? FullDescription { get; set; }

  [MaxLength(100)]
  [Column("Category")]
  public string? Category { get; set; }

  [Column("CategoryId")]
  public int? CategoryId { get; set; }

  [MaxLength(500)]
  [Column("ThumbnailUrl")]
  public string? ThumbnailUrl { get; set; }

  [MaxLength(500)]
  [Column("CoverImageUrl")]
  public string? CoverImageUrl { get; set; }


  [Column("ThumbnailImage")]
  public byte[]? ThumbnailImage { get; set; }

  [MaxLength(100)]
  [Column("ThumbnailContentType")]
  public string? ThumbnailContentType { get; set; }

  [Column("CoverImage")]
  public byte[]? CoverImage { get; set; }

  [MaxLength(100)]
  [Column("CoverImageContentType")]
  public string? CoverImageContentType { get; set; }

  [Column("DifficultyLevel")]
  public string DifficultyLevel { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced

  [Column("EstimatedDurationHours")]
  public int? EstimatedDurationHours { get; set; }

  [Column("Price")]
  [Precision(10, 2)]
  public decimal Price { get; set; } = 0; // 0 for free courses

  [Column("IsPublished")]
  public bool IsPublished { get; set; } = false;

  [Column("IsFeatured")]
  public bool? IsFeatured { get; set; } = false;

  [MaxLength(450)]
  [Column("CreatedByUserId")]
  public string? CreatedByUserId { get; set; }

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

  [Column("PublishedAt")]
  public DateTime? PublishedAt { get; set; }

  // Additional course metadata
  [Column("Tags")]
  public string? Tags { get; set; } // Comma-separated tags

  [Column("LearningObjectives")]
  public string? LearningObjectives { get; set; } // Comma-separated objectives

  [Column("Prerequisites")]
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
