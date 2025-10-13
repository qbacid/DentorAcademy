using Dentor.Academy.Web.DTOs.Quiz;

namespace Dentor.Academy.Web.Interfaces;

/// <summary>
/// Interface for quiz import operations
/// </summary>
public interface IQuizImportService
{
    Task<ImportResult> ImportFromJsonAsync(string jsonContent);
    Task<ImportResult> ImportQuizFromCsvAsync(Stream csvStream, string fileName);
    Task<ImportResult> ValidateQuizDataAsync(QuizImportDto quizData);
}
