namespace Dentor.Academy.Web.DTOs.Quiz;

/// <summary>
/// DTO for displaying quiz to users during quiz taking
/// </summary>
public class QuizDisplayDto
{
    public int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal PassingScore { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int TotalQuestions { get; set; }
    public List<QuestionDisplayDto> Questions { get; set; } = new();
}

/// <summary>
/// DTO for question display
/// </summary>
public class QuestionDisplayDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string? ExplanationImageUrl { get; set; }
    public decimal Points { get; set; }
    public int OrderIndex { get; set; }
    public List<AnswerOptionDisplayDto> AnswerOptions { get; set; } = new();
}

/// <summary>
/// DTO for answer option display (without showing correct answers)
/// </summary>
public class AnswerOptionDisplayDto
{
    public int AnswerOptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

/// <summary>
/// DTO for user's answer to a question
/// </summary>
public class QuestionAnswerDto
{
    public int QuestionId { get; set; }
    public List<int> SelectedAnswerOptionIds { get; set; } = new();
    public string? ShortTextAnswer { get; set; }
}

/// <summary>
/// DTO for quiz submission and results
/// </summary>
public class QuizResultDto
{
    public int QuizAttemptId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal TotalPointsEarned { get; set; }
    public decimal TotalPointsPossible { get; set; }
    public bool Passed { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<QuestionResultDetailDto> QuestionResults { get; set; } = new();
}

/// <summary>
/// DTO for individual question result detail
/// </summary>
public class QuestionResultDetailDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public decimal PointsEarned { get; set; }
    public decimal PointsPossible { get; set; }
    public string? Explanation { get; set; }
    public string? UserTextAnswer { get; set; }
    public List<AnswerResultDto> AnswerOptions { get; set; } = new();
}

/// <summary>
/// DTO for answer result showing correct/incorrect and user selection
/// </summary>
public class AnswerResultDto
{
    public int AnswerOptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public bool WasSelected { get; set; }
}
