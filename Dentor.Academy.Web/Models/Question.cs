using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents a question in flashcard format within a quiz
/// </summary>
[Table("questions")]
public class Question
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("quiz_id")]
    public int QuizId { get; set; }

    [Required]
    [MaxLength(1000)]
    [Column("question_text")]
    public string QuestionText { get; set; } = string.Empty;

    [Column("question_type")]
    public QuestionType QuestionType { get; set; }

    [MaxLength(2000)]
    [Column("explanation")]
    public string? Explanation { get; set; } // Shown after user selects answer

    [MaxLength(500)]
    [Column("explanation_image_url")]
    public string? ExplanationImageUrl { get; set; } // Optional image to help explain the answer

    [MaxLength(500)]
    [Column("question_image_url")]
    public string? QuestionImageUrl { get; set; } // Optional image as part of the question
    
    [MaxLength(500)]
    [Column("question_audio_url")]
    public string? QuestionAudioUrl { get; set; } // Optional audio as part of the question

    [Column("points")]
    public decimal Points { get; set; } = 1.0m;

    [Column("order_index")]
    public int OrderIndex { get; set; } // For ordering questions in the quiz

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("QuizId")]
    public Quiz Quiz { get; set; } = null!;
    
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    public ICollection<UserResponse> UserResponses { get; set; } = new List<UserResponse>();
}
