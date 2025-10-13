namespace Dentor.Academy.Web.DTOs.Quiz;

/// <summary>
/// Result of a quiz import operation
/// </summary>
public class ImportResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int QuizId { get; set; }
    public int QuestionsImported { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
