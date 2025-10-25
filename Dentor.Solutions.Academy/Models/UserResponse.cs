using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a user's response to a specific question in a quiz attempt
/// </summary>
[Table("UserResponses")]
[Index(nameof(QuizAttemptId), nameof(QuestionId))]
public class UserResponse
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("QuizAttemptId")]
  public int QuizAttemptId { get; set; }

  [Required]
  [Column("QuestionId")]
  public int QuestionId { get; set; }

  [Column("IsCorrect")]
  public bool IsCorrect { get; set; }

  [Column("PointsEarned")]
  public decimal PointsEarned { get; set; }

  [Column("AnsweredAt")]
  public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

  [MaxLength(500)]
  [Column("TextAnswer")]
  public string? TextAnswer { get; set; }

  // Navigation properties
  [ForeignKey("QuizAttemptId")]
  public QuizAttempt QuizAttempt { get; set; } = null!;

  [ForeignKey("QuestionId")]
  public Question Question { get; set; } = null!;

  public ICollection<UserResponseAnswer> UserResponseAnswers { get; set; } = new List<UserResponseAnswer>();
}
