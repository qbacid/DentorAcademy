using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents student reviews and ratings for courses
/// </summary>
[Table("course_reviews")]
[Index(nameof(CourseId), nameof(UserId), IsUnique = true)]
[Index(nameof(CourseId), nameof(Rating))]
[Index(nameof(CreatedAt))]
[Index(nameof(IsApproved))]
public class CourseReview
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
    [Range(1, 5)]
    [Column("rating")]
    public int Rating { get; set; }

    [MaxLength(100)]
    [Column("title")]
    public string? Title { get; set; }

    [Column("review_text")]
    public string? ReviewText { get; set; }

    [Column("is_approved")]
    public bool IsApproved { get; set; } = false;

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [MaxLength(450)]
    [Column("approved_by_user_id")]
    public string? ApprovedByUserId { get; set; }

    [Column("is_featured")]
    public bool IsFeatured { get; set; } = false;

    [Column("helpful_count")]
    public int HelpfulCount { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey("ApprovedByUserId")]
    public ApplicationUser? ApprovedBy { get; set; }
}

