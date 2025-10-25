using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Junction table storing which answer options the user selected for a response
/// For MultipleChoice and TrueFalse: one record
/// For MultipleCheckbox: multiple records (one per selected option)
/// </summary>
[Table("UserResponseAnswers")]
[Index(nameof(UserResponseId), nameof(AnswerOptionId), IsUnique = true)]
public class UserResponseAnswer
{
  [Key]
  [Column("Id")]
  public int Id { get; set; }

  [Required]
  [Column("UserResponseId")]
  public int UserResponseId { get; set; }

  [Required]
  [Column("AnswerOptionId")]
  public int AnswerOptionId { get; set; }

  [Column("SelectedAt")]
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
