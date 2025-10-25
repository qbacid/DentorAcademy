using Dentor.Academy.WebApp.DTOs.Quiz;
using Dentor.Solutions.Academy.Models;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Interface for quiz taking operations
/// </summary>
public interface IQuizTakingService
{
    Task<QuizAttempt?> StartQuizAsync(int quizId, string userId);
    Task<QuizAttempt?> GetActiveQuizAttemptAsync(int quizId, string userId);
    Task<bool> SubmitAnswerAsync(int quizAttemptId, int questionId, List<int> selectedAnswerIds, string? textAnswer = null);
    Task<QuizAttempt?> CompleteQuizAsync(int quizAttemptId);
    Task<List<QuizCardDto>> GetAvailableQuizzesAsync(string? category = null);
    Task<QuizDisplayDto?> GetQuizForTakingAsync(int quizId, string userId);
    Task<QuizDisplayDto?> GetQuizForDisplayAsync(int quizId);
    Task<int> StartQuizAttemptAsync(int quizId, string email, string fullName);
    Task SaveQuestionAnswerAsync(int quizAttemptId, QuestionAnswerDto answer);
    Task<QuizResultDto> SubmitQuizAsync(int quizAttemptId);
}
