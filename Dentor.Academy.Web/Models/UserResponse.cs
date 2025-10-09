using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a user's response to a specific question in a quiz attempt
/// </summary>
[Table("user_responses")]
[Index(nameof(QuizAttemptId), nameof(QuestionId))]
public class UserResponse
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [Column("quiz_attempt_id")]
  public int QuizAttemptId { get; set; }

  [Required]
  [Column("question_id")]
  public int QuestionId { get; set; }

  [Column("is_correct")]
  public bool IsCorrect { get; set; }

  [Column("points_earned")]
  public decimal PointsEarned { get; set; }

  [Column("answered_at")]
  public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

  [MaxLength(500)]
  [Column("text_answer")]
  public string? TextAnswer { get; set; }

  // Navigation properties
  [ForeignKey("QuizAttemptId")]
  public QuizAttempt QuizAttempt { get; set; } = null!;

  [ForeignKey("QuestionId")]
  public Question Question { get; set; } = null!;

  public ICollection<UserResponseAnswer> UserResponseAnswers { get; set; } = new List<UserResponseAnswer>();
}
