using Microsoft.EntityFrameworkCore;
using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.Models;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for managing quiz taking and display
/// </summary>
public class QuizTakingService
{
    private readonly QuizDbContext _context;
    private readonly QuizScoringService _scoringService;
    private readonly ILogger<QuizTakingService> _logger;

    public QuizTakingService(
        QuizDbContext context,
        QuizScoringService scoringService,
        ILogger<QuizTakingService> logger)
    {
        _context = context;
        _scoringService = scoringService;
        _logger = logger;
    }

    /// <summary>
    /// Get quiz details for display (without showing correct answers)
    /// </summary>
    public async Task<QuizDisplayDto?> GetQuizForDisplayAsync(int quizId)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions.OrderBy(qq => qq.OrderIndex))
            .ThenInclude(q => q.AnswerOptions.OrderBy(a => a.OrderIndex))
            .FirstOrDefaultAsync(q => q.Id == quizId && q.IsActive);

        if (quiz == null) return null;

        return new QuizDisplayDto
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            PassingScore = quiz.PassingScore,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            TotalQuestions = quiz.Questions.Count,
            Questions = quiz.Questions.Select(q => new QuestionDisplayDto
            {
                QuestionId = q.Id,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType.ToString(),
                Explanation = q.Explanation,
                ExplanationImageUrl = q.ExplanationImageUrl,
                Points = q.Points,
                OrderIndex = q.OrderIndex,
                AnswerOptions = q.AnswerOptions.Select(a => new AnswerOptionDisplayDto
                {
                    AnswerOptionId = a.Id,
                    OptionText = a.OptionText,
                    OrderIndex = a.OrderIndex
                }).ToList()
            }).ToList()
        };
    }

    /// <summary>
    /// Start a new quiz attempt
    /// </summary>
    public async Task<int> StartQuizAttemptAsync(int quizId, string email, string fullName)
    {
        var quiz = await _context.Quizzes.FindAsync(quizId);
        if (quiz == null || !quiz.IsActive)
        {
            throw new InvalidOperationException("Quiz not found or not active");
        }

        // Create a user identifier from email (you can enhance this later with proper auth)
        var userId = $"{email}|{fullName}";

        var attempt = new QuizAttempt
        {
            QuizId = quizId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        return attempt.Id;
    }

    /// <summary>
    /// Save user's answer for a question
    /// </summary>
    public async Task SaveQuestionAnswerAsync(int quizAttemptId, QuestionAnswerDto answer)
    {
        var question = await _context.Questions.FindAsync(answer.QuestionId)
            ?? throw new InvalidOperationException("Question not found while saving answer");

        // Upsert user response
        var response = await _context.UserResponses
            .FirstOrDefaultAsync(ur => ur.QuizAttemptId == quizAttemptId && ur.QuestionId == answer.QuestionId);

        if (response == null)
        {
            response = new UserResponse
            {
                QuizAttemptId = quizAttemptId,
                QuestionId = answer.QuestionId,
                AnsweredAt = DateTime.UtcNow
            };
            _context.UserResponses.Add(response);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Remove old option selections if any
            var oldAnswers = await _context.UserResponseAnswers
                .Where(ura => ura.UserResponseId == response.Id)
                .ToListAsync();
            _context.UserResponseAnswers.RemoveRange(oldAnswers);
        }

        // Persist short text answer (trim and cap at 500)
        response.TextAnswer = string.IsNullOrWhiteSpace(answer.ShortTextAnswer)
            ? null
            : (answer.ShortTextAnswer.Length > 500 ? answer.ShortTextAnswer.Substring(0, 500) : answer.ShortTextAnswer).Trim();

        if (question.QuestionType == QuestionType.UserShortAnwswer)
        {
            // Ignore option IDs for short answer; no auto-scoring
            response.IsCorrect = false;
            response.PointsEarned = 0;
        }
        else
        {
            // Save selected option IDs
            foreach (var optionId in answer.SelectedAnswerOptionIds)
            {
                var responseAnswer = new UserResponseAnswer
                {
                    UserResponseId = response.Id,
                    AnswerOptionId = optionId,
                    SelectedAt = DateTime.UtcNow
                };
                _context.UserResponseAnswers.Add(responseAnswer);
            }

            // Auto-evaluate for option-based questions
            var isCorrect = await _scoringService.EvaluateResponse(answer.QuestionId, answer.SelectedAnswerOptionIds);
            response.IsCorrect = isCorrect;
            response.PointsEarned = isCorrect ? question.Points : 0;
        }

        response.AnsweredAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Get saved answers for a quiz attempt
    /// </summary>
    public async Task<Dictionary<int, List<int>>> GetSavedAnswersAsync(int quizAttemptId)
    {
        var responses = await _context.UserResponses
            .Include(ur => ur.UserResponseAnswers)
            .Where(ur => ur.QuizAttemptId == quizAttemptId)
            .ToListAsync();

        return responses.ToDictionary(
            ur => ur.QuestionId,
            ur => ur.UserResponseAnswers.Select(ura => ura.AnswerOptionId).ToList()
        );
    }

    /// <summary>
    /// Submit quiz and get results
    /// </summary>
    public async Task<QuizResultDto> SubmitQuizAsync(int quizAttemptId)
    {
        // Calculate final score
        var attempt = await _scoringService.CalculateFinalScore(quizAttemptId);

        // Get detailed results
        var detailedResults = await _scoringService.GetAttemptResults(quizAttemptId);

        // Convert to result DTO
        var result = new QuizResultDto
        {
            QuizAttemptId = attempt.Id,
            QuizTitle = detailedResults.QuizTitle,
            Score = detailedResults.Score,
            TotalPointsEarned = detailedResults.TotalPointsEarned,
            TotalPointsPossible = detailedResults.TotalPointsPossible,
            Passed = detailedResults.Passed,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            QuestionResults = detailedResults.QuestionResults.Select(qr => new QuestionResultDetailDto
            {
                QuestionId = qr.QuestionId,
                QuestionText = qr.QuestionText,
                QuestionType = "Unknown", // Will be filled from database
                IsCorrect = qr.IsCorrect,
                PointsEarned = qr.PointsEarned,
                PointsPossible = qr.PointsPossible,
                Explanation = qr.Explanation,
                UserTextAnswer = qr.UserTextAnswer,
                AnswerOptions = new List<AnswerResultDto>() // Will be filled below
            }).ToList()
        };

        // Fill in answer options with correct/selected info
        var questions = await _context.Questions
            .Include(q => q.AnswerOptions)
            .Where(q => result.QuestionResults.Select(qr => qr.QuestionId).Contains(q.Id))
            .ToListAsync();

        foreach (var questionResult in result.QuestionResults)
        {
            var q = questions.First(qq => qq.Id == questionResult.QuestionId);
            questionResult.QuestionType = q.QuestionType.ToString();

            var detailedResult = detailedResults.QuestionResults.First(qr => qr.QuestionId == questionResult.QuestionId);

            questionResult.AnswerOptions = q.AnswerOptions.Select(ao => new AnswerResultDto
            {
                AnswerOptionId = ao.Id,
                OptionText = ao.OptionText,
                IsCorrect = ao.IsCorrect,
                WasSelected = detailedResult.SelectedAnswerIds.Contains(ao.Id)
            }).ToList();

            // Carry forward user's short text answer (if any)
            questionResult.UserTextAnswer = detailedResult.UserTextAnswer;
        }

        return result;
    }

    /// <summary>
    /// Check if quiz attempt is valid and not completed
    /// </summary>
    public async Task<bool> IsAttemptValidAsync(int quizAttemptId)
    {
        var attempt = await _context.QuizAttempts.FindAsync(quizAttemptId);
        return attempt != null && !attempt.IsCompleted;
    }
}

/// <summary>
/// DTO for quiz participant information
/// </summary>
public class QuizParticipantDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for quiz display with all questions and options
/// </summary>
public class QuizDisplayDto
{
    public int QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
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
