using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Models;

/// <summary>
/// Junction table storing which answer options the user selected for a response
/// For MultipleChoice and TrueFalse: one record
/// For MultipleCheckbox: multiple records (one per selected option)
/// </summary>
[Table("user_response_answers")]
[Index(nameof(UserResponseId), nameof(AnswerOptionId), IsUnique = true)]
public class UserResponseAnswer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_response_id")]
    public int UserResponseId { get; set; }

    [Required]
    [Column("answer_option_id")]
    public int AnswerOptionId { get; set; }

    [Column("selected_at")]
    public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserResponseId")]
    public UserResponse UserResponse { get; set; } = null!;

    [ForeignKey("AnswerOptionId")]
    public AnswerOption AnswerOption { get; set; } = null!;
}

/// <summary>
/// Enum representing the different types of questions supported
/// </summary>
public enum QuestionType
{
    MultipleChoice = 1,    // Single correct answer
    MultipleCheckbox = 2,  // Multiple correct answers
    TrueFalse = 3,         // True or False,
    UserShortAnwswer = 4 //short answer provided by user
}

