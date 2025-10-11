using System.Text.Json.Serialization;

namespace Dentor.Academy.Web.DTOs;

/// <summary>
/// DTO for importing a complete quiz from JSON
/// </summary>
public class QuizImportDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("passingScore")]
    public decimal PassingScore { get; set; } = 70.0m;

    [JsonPropertyName("timeLimitMinutes")]
    public int? TimeLimitMinutes { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("questions")]
    public List<QuestionImportDto> Questions { get; set; } = new();
}

/// <summary>
/// DTO for importing a question
/// </summary>
public class QuestionImportDto
{
    [JsonPropertyName("questionText")]
    public string QuestionText { get; set; } = string.Empty;

    [JsonPropertyName("questionType")]
    public string QuestionType { get; set; } = string.Empty; // "MultipleChoice", "MultipleCheckbox", "TrueFalse", or "ShortAnswer"

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    [JsonPropertyName("explanationImageUrl")]
    public string? ExplanationImageUrl { get; set; }

    [JsonPropertyName("points")]
    public decimal Points { get; set; } = 1.0m;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }

    [JsonPropertyName("answerOptions")]
    public List<AnswerOptionImportDto> AnswerOptions { get; set; } = new();
}

/// <summary>
/// DTO for importing an answer option
/// </summary>
public class AnswerOptionImportDto
{
    [JsonPropertyName("optionText")]
    public string OptionText { get; set; } = string.Empty;

    [JsonPropertyName("isCorrect")]
    public bool IsCorrect { get; set; }

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}
