using Dentor.Academy.WebApp.DTOs.Quiz;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Interface for quiz management operations (Admin/Instructor)
/// </summary>
public interface IQuizManagementService
{
    // Quiz CRUD operations
    Task<List<QuizManagementDto>> GetAllQuizzesAsync(string? category = null);
    Task<QuizManagementDto?> GetQuizByIdAsync(int quizId);
    Task<int> CreateQuizAsync(QuizUpdateDto quiz);
    Task<bool> UpdateQuizAsync(int quizId, QuizUpdateDto quiz);
    Task<bool> DeleteQuizAsync(int quizId);
    
    // Question CRUD operations
    Task<List<QuestionManagementDto>> GetQuestionsByQuizIdAsync(int quizId);
    Task<QuestionManagementDto?> GetQuestionByIdAsync(int questionId);
    Task<int> CreateQuestionAsync(int quizId, QuestionUpdateDto question);
    Task<bool> UpdateQuestionAsync(int questionId, QuestionUpdateDto question);
    Task<bool> DeleteQuestionAsync(int questionId);
    Task<bool> ReorderQuestionsAsync(int quizId, List<int> questionIds);
    
    // Media upload operations
    Task<MediaUploadResult> UploadQuestionImageAsync(Stream imageStream, string fileName);
    Task<MediaUploadResult> UploadQuestionAudioAsync(Stream audioStream, string fileName);
    Task<bool> DeleteMediaAsync(string mediaUrl);
}
