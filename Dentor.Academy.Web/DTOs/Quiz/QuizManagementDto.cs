namespace Dentor.Academy.Web.DTOs.Quiz;

/// <summary>
/// DTO for quiz management operations (includes full quiz data)
/// </summary>
public class QuizManagementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PassingScore { get; set; } = 70.0m;
    public int? TimeLimitMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CourseId { get; set; }
    public int TotalQuestions { get; set; }
    public int QuestionCount => TotalQuestions; // Alias for UI compatibility
    public List<QuestionManagementDto> Questions { get; set; } = new();
}
