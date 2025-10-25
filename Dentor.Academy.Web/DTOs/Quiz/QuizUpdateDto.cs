namespace Dentor.Academy.Web.DTOs.Quiz;

/// <summary>
/// DTO for creating or updating quiz basic information
/// </summary>
public class QuizUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PassingScore { get; set; } = 80.0m;
    public int? TimeLimitMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public int? CourseId { get; set; }
}

