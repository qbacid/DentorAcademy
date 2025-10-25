using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents a question in flashcard format within a quiz
/// </summary>
[Table("Questions")]
public class Question
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("QuizId")]
  public int QuizId { get; set; }

  [Required]
  [MaxLength(1000)]
  [Column("QuestionText")]
  public string QuestionText { get; set; } = string.Empty;

  [Column("QuestionType")]
  public QuestionType QuestionType { get; set; }

  [MaxLength(2000)]
  [Column("Explanation")]
  public string? Explanation { get; set; } // Shown after user selects answer

  [MaxLength(500)]
  [Column("ExplanationImageUrl")]
  public string? ExplanationImageUrl { get; set; } // Optional image to help explain the answer

  [MaxLength(500)]
  [Column("QuestionImageUrl")]
  public string? QuestionImageUrl { get; set; } // Optional image as part of the question

  [MaxLength(500)]
  [Column("QuestionAudioUrl")]
  public string? QuestionAudioUrl { get; set; } // Optional audio as part of the question

  [Column("Points")]
  public decimal Points { get; set; } = 1.0m;

  [Column("OrderIndex")]
  public int OrderIndex { get; set; } // For ordering questions in the quiz

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("UpdatedAt")]
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("QuizId")]
  public Quiz Quiz { get; set; } = null!;

  public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
  public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
}
