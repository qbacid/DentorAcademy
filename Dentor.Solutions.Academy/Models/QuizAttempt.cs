using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a user's attempt at completing a quiz
/// </summary>
[Table("QuizAttempts")]
[Index(nameof(UserId), nameof(QuizId))]
[Index(nameof(StartedAt))]
public class QuizAttempt
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("QuizId")]
  public int QuizId { get; set; }

  [Required]
  [MaxLength(450)]
  [Column("UserId")]
  public string UserId { get; set; } = string.Empty; // Can be linked to Identity user

  [Column("StartedAt")]
  public DateTime StartedAt { get; set; } = DateTime.UtcNow;

  [Column("CompletedAt")]
  public DateTime? CompletedAt { get; set; }

  [Column("Score")]
  public decimal? Score { get; set; } // Calculated score (0-100)

  [Column("TotalPointsEarned")]
  public decimal? TotalPointsEarned { get; set; }

  [Column("TotalPointsPossible")]
  public decimal? TotalPointsPossible { get; set; }

  [Column("Passed")]
  public bool? Passed { get; set; }

  [Column("IsCompleted")]
  public bool IsCompleted { get; set; } = false;

  // Navigation properties
  [ForeignKey("QuizId")]
  public Quiz Quiz { get; set; } = null!;

  public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
}
