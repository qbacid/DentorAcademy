using Dentor.Solutions.Academy.Models;
using Dentor.Solutions.Academy.Services;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Interface for quiz scoring and evaluation operations
/// </summary>
public interface IQuizScoringService
{
    Task<bool> EvaluateResponse(int questionId, List<int> selectedAnswerOptionIds);
    Task<decimal> CalculateQuizScore(int quizAttemptId);
    Task<bool> HasPassedQuiz(int quizAttemptId);
    Task<decimal> CalculateQuestionScore(int questionId, List<int> selectedAnswerOptionIds);
    Task<QuizAttempt> CalculateFinalScore(int quizAttemptId);
    Task<QuizAttemptResult> GetAttemptResults(int quizAttemptId);
}
