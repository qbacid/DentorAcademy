namespace Dentor.Academy.WebApp.DTOs.Quiz;

/// <summary>
/// DTO for question management operations
/// </summary>
public class QuestionManagementDto
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "MultipleChoice";
    public string? Explanation { get; set; }
    public string? ExplanationImageUrl { get; set; }
    
    // New fields for rich media
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    
    public decimal Points { get; set; } = 1.0m;
    public int OrderIndex { get; set; }
    public List<AnswerOptionDto> AnswerOptions { get; set; } = new();
}

/// <summary>
/// DTO for creating/updating question
/// </summary>
public class QuestionUpdateDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "MultipleChoice";
    public string? Explanation { get; set; }
    public string? ExplanationImageUrl { get; set; }
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public decimal Points { get; set; } = 1.0m;
    public int OrderIndex { get; set; }
    public List<AnswerOptionUpdateDto> AnswerOptions { get; set; } = new();
}

/// <summary>
/// DTO for answer option in management context
/// </summary>
public class AnswerOptionDto
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// DTO for creating/updating answer option
/// </summary>
public class AnswerOptionUpdateDto
{
    public int? Id { get; set; } // Null for new options
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// DTO for media upload validation result
/// </summary>
public class MediaUploadResult
{
    public bool Success { get; set; }
    public string? Url { get; set; }
    public string? ErrorMessage { get; set; }
}
