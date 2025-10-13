namespace Dentor.Academy.Web.DTOs.Quiz;

/// <summary>
/// DTO for displaying quiz card information in lists
/// </summary>
public class QuizCardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PassingScore { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int QuestionCount { get; set; }
}
