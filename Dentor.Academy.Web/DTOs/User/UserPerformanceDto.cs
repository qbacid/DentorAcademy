namespace Dentor.Academy.Web.DTOs.User;

/// <summary>
/// DTO for user performance analytics
/// </summary>
public class UserPerformanceDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int TotalQuizzesTaken { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public decimal OverallAverageScore { get; set; }
    public List<CategoryPerformanceDto> CategoryPerformances { get; set; } = new();
    public List<string> StrengthAreas { get; set; } = new(); // Categories with score >= 80%
    public List<string> ImprovementAreas { get; set; } = new(); // Categories with score < 70%
    public DateTime? LastActivityDate { get; set; }
}

/// <summary>
/// Alias for UserPerformanceDto for backward compatibility
/// </summary>
public class UserPerformanceSummaryDto : UserPerformanceDto
{
}

/// <summary>
/// DTO for user performance statistics by category
/// </summary>
public class CategoryPerformanceDto
{
    public string Category { get; set; } = string.Empty;
    public int QuizzesTaken { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public decimal AverageScore { get; set; }
    public decimal? BestScore { get; set; }
    public decimal? LatestScore { get; set; }
    public DateTime? LastAttemptDate { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty; // "Excellent", "Good", "Needs Improvement"
    public bool IsStrength { get; set; } // Score >= 80%
    public bool NeedsImprovement { get; set; } // Score < 70%
}

/// <summary>
/// DTO for quiz attempt summary
/// </summary>
public class QuizAttemptSummaryDto
{
    public int AttemptId { get; set; }
    public int QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Score { get; set; }
    public bool Passed { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
}
