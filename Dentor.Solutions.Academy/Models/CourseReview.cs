using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dentor.Solutions.Academy.Data;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents student reviews and ratings for courses
/// </summary>
[Table("CourseReviews")]
[Index(nameof(CourseId), nameof(UserId), IsUnique = true)]
[Index(nameof(CourseId), nameof(Rating))]
[Index(nameof(CreatedAt))]
[Index(nameof(IsApproved))]
public class CourseReview
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
  [Range(1, 5)]
  [Column("Rating")]
  public int Rating { get; set; }

  [MaxLength(100)]
  [Column("Title")]
  public string? Title { get; set; }

  [Column("ReviewText")]
  public string? ReviewText { get; set; }

  [Column("IsApproved")]
  public bool IsApproved { get; set; } = false;

  [Column("ApprovedAt")]
  public DateTime? ApprovedAt { get; set; }

  [MaxLength(450)]
  [Column("ApprovedByUserId")]
  public string? ApprovedByUserId { get; set; }

  [Column("IsFeatured")]
  public bool IsFeatured { get; set; } = false;

  [Column("HelpfulCount")]
  public int HelpfulCount { get; set; } = 0;

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("CourseId")]
  public Course Course { get; set; } = null!;

  [ForeignKey("UserId")]
  public ApplicationUser User { get; set; } = null!;

  [ForeignKey("ApprovedByUserId")]
  public ApplicationUser? ApprovedBy { get; set; }
}
