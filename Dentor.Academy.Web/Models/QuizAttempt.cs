using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a user's attempt at completing a quiz
/// </summary>
[Table("quiz_attempts")]
[Index(nameof(UserId), nameof(QuizId))]
[Index(nameof(StartedAt))]
public class QuizAttempt
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("quiz_id")]
    public int QuizId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty; // Can be linked to Identity user

    [Column("started_at")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("score")]
    public decimal? Score { get; set; } // Calculated score (0-100)

    [Column("total_points_earned")]
    public decimal? TotalPointsEarned { get; set; }

    [Column("total_points_possible")]
    public decimal? TotalPointsPossible { get; set; }

    [Column("passed")]
    public bool? Passed { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; } = false;

    // Navigation properties
    [ForeignKey("QuizId")]
    public Quiz Quiz { get; set; } = null!;
    
    public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
}

