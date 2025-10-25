using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Represents an answer option for a question
/// For MultipleChoice: one option has IsCorrect = true
/// For MultipleCheckbox: multiple options can have IsCorrect = true
/// For TrueFalse: two options (True/False), one has IsCorrect = true
/// </summary>
[Table("AnswerOptions")]
[Index(nameof(QuestionId), nameof(OrderIndex))]
public class AnswerOption
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("QuestionId")]
  public int QuestionId { get; set; }

  [Required]
  [MaxLength(500)]
  [Column("OptionText")]
  public string OptionText { get; set; } = string.Empty;

  [Column("IsCorrect")]
  public bool IsCorrect { get; set; }

  [Column("OrderIndex")]
  public int OrderIndex { get; set; }

  [Column("CreatedAt")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  [ForeignKey("QuestionId")]
  public Question Question { get; set; } = null!;

  public ICollection<UserResponseAnswer> UserResponseAnswers { get; set; } = new List<UserResponseAnswer>();
}
