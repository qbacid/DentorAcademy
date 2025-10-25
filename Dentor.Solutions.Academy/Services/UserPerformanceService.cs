using Dentor.Academy.WebApp.DTOs.User;
using Dentor.Solutions.Academy.Data;
using Dentor.Solutions.Academy.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Services;

/// <summary>
/// Service for calculating and tracking user performance analytics by category
/// </summary>
public class UserPerformanceService : IUserPerformanceService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserPerformanceService> _logger;

    public UserPerformanceService(ApplicationDbContext dbContext, ILogger<UserPerformanceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive performance summary for a user
    /// </summary>
    public async Task<UserPerformanceDto?> GetUserPerformanceAsync(string userId)
    {
        var attempts = await _dbContext.QuizAttempts
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId && qa.IsCompleted && qa.Score.HasValue)
            .OrderByDescending(qa => qa.CompletedAt)
            .ToListAsync();

        if (!attempts.Any())
        {
            return null;
        }

        var categoryPerformances = await CalculateCategoryPerformancesAsync(userId);

        var summary = new UserPerformanceDto
        {
            UserId = userId,
            UserEmail = ExtractEmailFromUserId(userId),
            TotalQuizzesTaken = attempts.Count,
            TotalQuestionsAnswered = await GetTotalQuestionsAnsweredAsync(userId),
            OverallAverageScore = attempts.Average(a => a.Score!.Value),
            CategoryPerformances = categoryPerformances,
            StrengthAreas = categoryPerformances
                .Where(cp => cp.IsStrength)
                .Select(cp => cp.Category)
                .ToList(),
            ImprovementAreas = categoryPerformances
                .Where(cp => cp.NeedsImprovement)
                .Select(cp => cp.Category)
                .ToList(),
            LastActivityDate = attempts.Max(a => a.CompletedAt)
        };

        return summary;
    }

    /// <summary>
    /// Get top performing users
    /// </summary>
    public async Task<List<UserPerformanceDto>> GetTopPerformersAsync(int count = 10)
    {
        var topUsers = await _dbContext.QuizAttempts
            .Where(qa => qa.IsCompleted && qa.Score.HasValue)
            .GroupBy(qa => qa.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                AverageScore = g.Average(qa => qa.Score!.Value),
                QuizzesTaken = g.Count()
            })
            .OrderByDescending(u => u.AverageScore)
            .Take(count)
            .ToListAsync();

        var performers = new List<UserPerformanceDto>();
        foreach (var user in topUsers)
        {
            var performance = await GetUserPerformanceAsync(user.UserId);
            if (performance != null)
            {
                performers.Add(performance);
            }
        }

        return performers;
    }

    /// <summary>
    /// Get category performance summary (category name -> quiz count)
    /// </summary>
    public async Task<Dictionary<string, int>> GetCategoryPerformanceAsync(string userId)
    {
        return await _dbContext.QuizAttempts
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId 
                && qa.IsCompleted 
                && qa.Quiz.Category != null)
            .GroupBy(qa => qa.Quiz.Category!)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count);
    }

    /// <summary>
    /// Calculate performance statistics for each category
    /// </summary>
    public async Task<List<CategoryPerformanceDto>> CalculateCategoryPerformancesAsync(string userId)
    {
        var attemptsWithCategory = await _dbContext.QuizAttempts
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId 
                && qa.IsCompleted 
                && qa.Score.HasValue 
                && qa.Quiz.Category != null)
            .GroupBy(qa => qa.Quiz.Category!)
            .Select(g => new
            {
                Category = g.Key,
                Attempts = g.OrderByDescending(qa => qa.CompletedAt).ToList()
            })
            .ToListAsync();

        var categoryPerformances = new List<CategoryPerformanceDto>();

        foreach (var categoryGroup in attemptsWithCategory)
        {
            var attempts = categoryGroup.Attempts;
            var averageScore = attempts.Average(a => a.Score!.Value);
            
            var performance = new CategoryPerformanceDto
            {
                Category = categoryGroup.Category,
                QuizzesTaken = attempts.Count,
                QuestionsAnswered = await GetQuestionsAnsweredByCategoryAsync(userId, categoryGroup.Category),
                CorrectAnswers = await GetCorrectAnswersByCategoryAsync(userId, categoryGroup.Category),
                AverageScore = Math.Round(averageScore, 2),
                BestScore = attempts.Max(a => a.Score!.Value),
                LatestScore = attempts.First().Score!.Value,
                LastAttemptDate = attempts.First().CompletedAt,
                PerformanceLevel = DeterminePerformanceLevel(averageScore),
                IsStrength = averageScore >= 80,
                NeedsImprovement = averageScore < 70
            };

            categoryPerformances.Add(performance);
        }

        return categoryPerformances.OrderByDescending(cp => cp.AverageScore).ToList();
    }

    /// <summary>
    /// Get performance for a specific category
    /// </summary>
    public async Task<CategoryPerformanceDto?> GetCategoryPerformanceDetailAsync(string userId, string category)
    {
        var performances = await CalculateCategoryPerformancesAsync(userId);
        return performances.FirstOrDefault(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get all quiz attempts for a user with summary information
    /// </summary>
    public async Task<List<QuizAttemptSummaryDto>> GetUserQuizAttemptsAsync(string userId)
    {
        var attempts = await _dbContext.QuizAttempts
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId && qa.IsCompleted)
            .OrderByDescending(qa => qa.CompletedAt)
            .Select(qa => new QuizAttemptSummaryDto
            {
                AttemptId = qa.Id,
                QuizId = qa.QuizId,
                QuizTitle = qa.Quiz.Title,
                Category = qa.Quiz.Category,
                Score = qa.Score ?? 0,
                Passed = qa.Passed ?? false,
                StartedAt = qa.StartedAt,
                CompletedAt = qa.CompletedAt,
                TotalQuestions = qa.Quiz.Questions.Count,
                CorrectAnswers = 0 // Will be calculated below
            })
            .ToListAsync();

        // Calculate correct answers for each attempt
        foreach (var attempt in attempts)
        {
            var correctCount = await _dbContext.UserResponses
                .Where(ur => ur.QuizAttemptId == attempt.AttemptId && ur.IsCorrect)
                .CountAsync();
            attempt.CorrectAnswers = correctCount;
        }

        return attempts;
    }

    /// <summary>
    /// Get quiz attempts filtered by category
    /// </summary>
    public async Task<List<QuizAttemptSummaryDto>> GetUserQuizAttemptsByCategoryAsync(string userId, string category)
    {
        var allAttempts = await GetUserQuizAttemptsAsync(userId);
        return allAttempts
            .Where(a => a.Category != null && a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Get user's strongest categories (top 3)
    /// </summary>
    public async Task<List<CategoryPerformanceDto>> GetTopStrengthAreasAsync(string userId, int topCount = 3)
    {
        var performances = await CalculateCategoryPerformancesAsync(userId);
        return performances
            .Where(p => p.QuizzesTaken >= 2) // At least 2 attempts for reliable data
            .OrderByDescending(p => p.AverageScore)
            .Take(topCount)
            .ToList();
    }

    /// <summary>
    /// Get categories that need improvement (bottom 3 with score < 80%)
    /// </summary>
    public async Task<List<CategoryPerformanceDto>> GetImprovementAreasAsync(string userId, int bottomCount = 3)
    {
        var performances = await CalculateCategoryPerformancesAsync(userId);
        return performances
            .Where(p => p.AverageScore < 80 && p.QuizzesTaken >= 1)
            .OrderBy(p => p.AverageScore)
            .Take(bottomCount)
            .ToList();
    }

    /// <summary>
    /// Check if user has improved in a category over time
    /// </summary>
    public async Task<bool> HasImprovedInCategoryAsync(string userId, string category)
    {
        var attempts = await _dbContext.QuizAttempts
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId 
                && qa.Quiz.Category == category 
                && qa.IsCompleted 
                && qa.Score.HasValue)
            .OrderBy(qa => qa.CompletedAt)
            .Select(qa => qa.Score!.Value)
            .ToListAsync();

        if (attempts.Count < 2) return false;

        // Compare first half average with second half average
        var midpoint = attempts.Count / 2;
        var firstHalfAvg = attempts.Take(midpoint).Average();
        var secondHalfAvg = attempts.Skip(midpoint).Average();

        return secondHalfAvg > firstHalfAvg;
    }

    #region Private Helper Methods

    private async Task<int> GetTotalQuestionsAnsweredAsync(string userId)
    {
        return await _dbContext.UserResponses
            .Where(ur => ur.QuizAttempt.UserId == userId)
            .CountAsync();
    }

    private async Task<int> GetQuestionsAnsweredByCategoryAsync(string userId, string category)
    {
        return await _dbContext.UserResponses
            .Where(ur => ur.QuizAttempt.UserId == userId 
                && ur.QuizAttempt.Quiz.Category == category)
            .CountAsync();
    }

    private async Task<int> GetCorrectAnswersByCategoryAsync(string userId, string category)
    {
        return await _dbContext.UserResponses
            .Where(ur => ur.QuizAttempt.UserId == userId 
                && ur.QuizAttempt.Quiz.Category == category 
                && ur.IsCorrect)
            .CountAsync();
    }

    private static string DeterminePerformanceLevel(decimal averageScore)
    {
        return averageScore switch
        {
            >= 90 => "Excellent",
            >= 80 => "Very Good",
            >= 70 => "Good",
            >= 60 => "Fair",
            _ => "Needs Improvement"
        };
    }

    private static string ExtractEmailFromUserId(string userId)
    {
        // UserId format is "email|fullname"
        var parts = userId.Split('|');
        return parts.Length > 0 ? parts[0] : userId;
    }

    #endregion
}
