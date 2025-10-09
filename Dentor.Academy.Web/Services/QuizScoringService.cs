using Microsoft.EntityFrameworkCore;
using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.Models;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for managing quiz scoring and calculations
/// </summary>
public class QuizScoringService
{
    private readonly QuizDbContext _context;

    public QuizScoringService(QuizDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Evaluates a user's response to a question and determines if it's correct
    /// </summary>
    public async Task<bool> EvaluateResponse(int questionId, List<int> selectedAnswerOptionIds)
    {
        var question = await _context.Questions
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
            throw new ArgumentException("Question not found", nameof(questionId));

        var correctAnswerIds = question.AnswerOptions
            .Where(ao => ao.IsCorrect)
            .Select(ao => ao.Id)
            .OrderBy(id => id)
            .ToList();

        var selectedIds = selectedAnswerOptionIds.OrderBy(id => id).ToList();

        // For correct answer, selected options must exactly match correct options
        return correctAnswerIds.SequenceEqual(selectedIds);
    }

    /// <summary>
    /// Records a user's response to a question
    /// </summary>
    public async Task<UserResponse> RecordUserResponse(
        int quizAttemptId,
        int questionId,
        List<int> selectedAnswerOptionIds)
    {
        var question = await _context.Questions
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
            throw new ArgumentException("Question not found", nameof(questionId));

        var isCorrect = await EvaluateResponse(questionId, selectedAnswerOptionIds);
        var pointsEarned = isCorrect ? question.Points : 0;

        var userResponse = new UserResponse
        {
            QuizAttemptId = quizAttemptId,
            QuestionId = questionId,
            IsCorrect = isCorrect,
            PointsEarned = pointsEarned,
            AnsweredAt = DateTime.UtcNow
        };

        _context.UserResponses.Add(userResponse);
        await _context.SaveChangesAsync();

        // Record selected answers
        foreach (var answerId in selectedAnswerOptionIds)
        {
            var userResponseAnswer = new UserResponseAnswer
            {
                UserResponseId = userResponse.Id,
                AnswerOptionId = answerId,
                SelectedAt = DateTime.UtcNow
            };
            _context.UserResponseAnswers.Add(userResponseAnswer);
        }

        await _context.SaveChangesAsync();

        return userResponse;
    }

    /// <summary>
    /// Calculates and saves the final score for a quiz attempt
    /// </summary>
    public async Task<QuizAttempt> CalculateFinalScore(int quizAttemptId)
    {
        var attempt = await _context.QuizAttempts
            .Include(qa => qa.Quiz)
            .Include(qa => qa.UserResponses)
            .ThenInclude(ur => ur.Question)
            .FirstOrDefaultAsync(qa => qa.Id == quizAttemptId);

        if (attempt == null)
            throw new ArgumentException("Quiz attempt not found", nameof(quizAttemptId));

        // Consider only auto-gradable questions for scoring (exclude short answers)
        var gradable = attempt.UserResponses.Where(ur => ur.Question.QuestionType != QuestionType.UserShortAnwswer).ToList();

        // Calculate total points
        var totalPointsEarned = gradable.Sum(ur => ur.PointsEarned);
        var totalPointsPossible = gradable.Sum(ur => ur.Question.Points);

        // Calculate percentage score
        var score = totalPointsPossible > 0 
            ? (totalPointsEarned / totalPointsPossible) * 100 
            : 0;

        // Update attempt
        attempt.TotalPointsEarned = totalPointsEarned;
        attempt.TotalPointsPossible = totalPointsPossible;
        attempt.Score = score;
        attempt.Passed = score >= attempt.Quiz.PassingScore;
        attempt.CompletedAt = DateTime.UtcNow;
        attempt.IsCompleted = true;

        await _context.SaveChangesAsync();

        return attempt;
    }

    /// <summary>
    /// Gets detailed results for a quiz attempt
    /// </summary>
    public async Task<QuizAttemptResult> GetAttemptResults(int quizAttemptId)
    {
        var attempt = await _context.QuizAttempts
            .Include(qa => qa.Quiz)
            .Include(qa => qa.UserResponses)
            .ThenInclude(ur => ur.Question)
            .ThenInclude(q => q.AnswerOptions)
            .Include(qa => qa.UserResponses)
            .ThenInclude(ur => ur.UserResponseAnswers)
            .ThenInclude(ura => ura.AnswerOption)
            .FirstOrDefaultAsync(qa => qa.Id == quizAttemptId);

        if (attempt == null)
            throw new ArgumentException("Quiz attempt not found", nameof(quizAttemptId));

        return new QuizAttemptResult
        {
            QuizAttemptId = attempt.Id,
            QuizTitle = attempt.Quiz.Title,
            Score = attempt.Score ?? 0,
            TotalPointsEarned = attempt.TotalPointsEarned ?? 0,
            TotalPointsPossible = attempt.TotalPointsPossible ?? 0,
            Passed = attempt.Passed ?? false,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            QuestionResults = attempt.UserResponses.Select(ur => new QuestionResult
            {
                QuestionId = ur.QuestionId,
                QuestionText = ur.Question.QuestionText,
                IsCorrect = ur.IsCorrect,
                PointsEarned = ur.PointsEarned,
                PointsPossible = ur.Question.Points,
                SelectedAnswerIds = ur.UserResponseAnswers.Select(ura => ura.AnswerOptionId).ToList(),
                CorrectAnswerIds = ur.Question.AnswerOptions.Where(ao => ao.IsCorrect).Select(ao => ao.Id).ToList(),
                Explanation = ur.Question.Explanation,
                UserTextAnswer = ur.TextAnswer
            }).ToList()
        };
    }
}

/// <summary>
/// DTO for quiz attempt results
/// </summary>
public class QuizAttemptResult
{
    public int QuizAttemptId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal TotalPointsEarned { get; set; }
    public decimal TotalPointsPossible { get; set; }
    public bool Passed { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<QuestionResult> QuestionResults { get; set; } = new();
}

/// <summary>
/// DTO for individual question results
/// </summary>
public class QuestionResult
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public decimal PointsEarned { get; set; }
    public decimal PointsPossible { get; set; }
    public List<int> SelectedAnswerIds { get; set; } = new();
    public List<int> CorrectAnswerIds { get; set; } = new();
    public string? Explanation { get; set; }
    public string? UserTextAnswer { get; set; }
}
