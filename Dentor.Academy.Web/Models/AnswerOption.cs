using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Represents an answer option for a question
/// For MultipleChoice: one option has IsCorrect = true
/// For MultipleCheckbox: multiple options can have IsCorrect = true
/// For TrueFalse: two options (True/False), one has IsCorrect = true
/// </summary>
[Table("answer_options")]
[Index(nameof(QuestionId), nameof(OrderIndex))]
public class AnswerOption
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("question_id")]
    public int QuestionId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("option_text")]
    public string OptionText { get; set; } = string.Empty;

    [Column("is_correct")]
    public bool IsCorrect { get; set; }

    [Column("order_index")]
    public int OrderIndex { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("QuestionId")]
    public Question Question { get; set; } = null!;
    
    public ICollection<UserResponseAnswer> UserResponseAnswers { get; set; } = new List<UserResponseAnswer>();
}

