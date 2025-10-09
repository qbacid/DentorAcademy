using System.Text.Json;
using System.Text.Json.Serialization;
using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.DTOs;
using Dentor.Academy.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for importing quizzes from JSON file format
/// </summary>
public class QuizImportService
{
    private readonly QuizDbContext _context;
    private readonly ILogger<QuizImportService> _logger;

    public QuizImportService(QuizDbContext context, ILogger<QuizImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Import quiz from JSON string
    /// </summary>
    public async Task<ImportResult> ImportFromJsonAsync(string jsonContent)
    {
        var result = new ImportResult();

        try
        {
            var quizDto = JsonSerializer.Deserialize<QuizImportDto>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (quizDto == null)
            {
                result.Errors.Add("Failed to parse JSON content");
                return result;
            }

            return await ProcessQuizImportAsync(quizDto);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error");
            result.Errors.Add($"JSON parsing error: {ex.Message}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing quiz from JSON");
            result.Errors.Add($"Import error: {ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// Process the quiz import DTO and save to database
    /// </summary>
    private async Task<ImportResult> ProcessQuizImportAsync(QuizImportDto quizDto)
    {
        var result = new ImportResult();

        // Validate quiz first
        var validationErrors = ValidateQuizDto(quizDto);
        if (validationErrors.Any())
        {
            result.Errors.AddRange(validationErrors);
            return result;
        }

        // Use the execution strategy to handle retries with transactions
        var strategy = _context.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(async (ct) =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                // Create quiz
                var quiz = new Quiz
                {
                    Title = quizDto.Title,
                    Description = quizDto.Description,
                    PassingScore = quizDto.PassingScore,
                    TimeLimitMinutes = quizDto.TimeLimitMinutes,
                    IsActive = quizDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync(ct);

                // Create questions
                foreach (var questionDto in quizDto.Questions)
                {
                    if (!TryParseQuestionType(questionDto.QuestionType, out var questionType))
                    {
                        result.Warnings.Add($"Invalid question type '{questionDto.QuestionType}' for question: {questionDto.QuestionText}");
                        continue;
                    }

                    var question = new Question
                    {
                        QuizId = quiz.Id,
                        QuestionText = questionDto.QuestionText,
                        QuestionType = questionType,
                        Explanation = questionDto.Explanation,
                        ExplanationImageUrl = questionDto.ExplanationImageUrl,
                        Points = questionDto.Points,
                        OrderIndex = questionDto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync(ct);

                    // Create answer options (skip for short-answer)
                    if (questionType != QuestionType.UserShortAnwswer)
                    {
                        foreach (var optionDto in questionDto.AnswerOptions)
                        {
                            var option = new AnswerOption
                            {
                                QuestionId = question.Id,
                                OptionText = optionDto.OptionText,
                                IsCorrect = optionDto.IsCorrect,
                                OrderIndex = optionDto.DisplayOrder,
                                CreatedAt = DateTime.UtcNow
                            };

                            _context.AnswerOptions.Add(option);
                        }

                        await _context.SaveChangesAsync(ct);
                    }
                }

                await transaction.CommitAsync(ct);

                result.Success = true;
                result.QuizId = quiz.Id;
                result.QuestionsImported = quizDto.Questions.Count;
                result.Message = $"Successfully imported quiz '{quiz.Title}' with {result.QuestionsImported} questions";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error processing quiz import");
                result.Errors.Add($"Database error: {ex.Message}");
                throw; // Re-throw to allow retry strategy to work
            }
        }, CancellationToken.None);

        return result;
    }

    // Parses question type with support for common aliases (case-insensitive)
    private static bool TryParseQuestionType(string? type, out QuestionType qType)
    {
        qType = default;
        if (string.IsNullOrWhiteSpace(type)) return false;
        var norm = type.Trim().Replace(" ", string.Empty).ToLowerInvariant();
        // Accept friendly aliases
        if (norm is "shortanswer" or "usershortanswer" or "usershortanwswer")
        {
            qType = QuestionType.UserShortAnwswer;
            return true;
        }
        if (Enum.TryParse<QuestionType>(type, true, out var parsed))
        {
            qType = parsed;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Validate quiz DTO before import
    /// </summary>
    private List<string> ValidateQuizDto(QuizImportDto quizDto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(quizDto.Title))
        {
            errors.Add("Quiz title is required");
        }

        if (quizDto.PassingScore < 0 || quizDto.PassingScore > 100)
        {
            errors.Add("Passing score must be between 0 and 100");
        }

        if (quizDto.Questions == null || quizDto.Questions.Count == 0)
        {
            errors.Add("Quiz must have at least one question");
            return errors;
        }

        foreach (var question in quizDto.Questions)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                errors.Add($"Question at display order {question.DisplayOrder} has no text");
            }

            // Validate type and adapt rules for short answers
            if (!TryParseQuestionType(question.QuestionType, out var qType))
            {
                errors.Add($"Invalid question type '{question.QuestionType}' for question: {question.QuestionText}");
                // If type invalid, skip further checks for this item
                continue;
            }

            // For TrueFalse, exactly two options are recommended but not required; we don't block import here.
            // Guidance is provided in the documentation/UI.
            // if (qType == QuestionType.TrueFalse && question.AnswerOptions.Count != 2) { /* advisory only */ }

            if (qType == QuestionType.UserShortAnwswer)
            {
                // Short-answer: no options required, no correct answers required
                continue;
            }

            if (!question.AnswerOptions.Any())
            {
                errors.Add($"Question '{question.QuestionText}' has no answer options");
            }

            if (!question.AnswerOptions.Any(a => a.IsCorrect))
            {
                errors.Add($"Question '{question.QuestionText}' has no correct answer marked");
            }
        }

        return errors;
    }
}

/// <summary>
/// DTO for importing a complete quiz from JSON
/// </summary>
public class QuizImportDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

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
