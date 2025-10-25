using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.DTOs.Quiz;
using Dentor.Academy.Web.Interfaces;
using Dentor.Academy.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for managing quizzes and questions (Admin/Instructor operations)
/// </summary>
public class QuizManagementService : IQuizManagementService
{
    private readonly QuizDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<QuizManagementService> _logger;
    private const long MaxImageSizeBytes = 1 * 1024 * 1024; // 1MB
    private const int MaxImageDimension = 450; // 450x450 pixels

    public QuizManagementService(
        QuizDbContext context,
        IWebHostEnvironment environment,
        ILogger<QuizManagementService> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    #region Quiz CRUD Operations

    public async Task<List<QuizManagementDto>> GetAllQuizzesAsync(string? category = null)
    {
        var query = _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.AnswerOptions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(q => q.Category == category);
        }

        var quizzes = await query
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return quizzes.Select(q => MapToQuizManagementDto(q)).ToList();
    }

    public async Task<QuizManagementDto?> GetQuizByIdAsync(int quizId)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions.OrderBy(q => q.OrderIndex))
            .ThenInclude(q => q.AnswerOptions.OrderBy(a => a.OrderIndex))
            .FirstOrDefaultAsync(q => q.Id == quizId);

        return quiz == null ? null : MapToQuizManagementDto(quiz);
    }

    public async Task<int> CreateQuizAsync(QuizUpdateDto quiz)
    {
        var newQuiz = new Quiz
        {
            Title = quiz.Title,
            Description = quiz.Description,
            Category = quiz.Category,
            PassingScore = quiz.PassingScore,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            IsActive = quiz.IsActive,
            CourseId = quiz.CourseId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Quizzes.Add(newQuiz);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created quiz {QuizId} - {Title}", newQuiz.Id, newQuiz.Title);
        return newQuiz.Id;
    }

    public async Task<bool> UpdateQuizAsync(int quizId, QuizUpdateDto quiz)
    {
        var existingQuiz = await _context.Quizzes.FindAsync(quizId);
        if (existingQuiz == null) return false;

        existingQuiz.Title = quiz.Title;
        existingQuiz.Description = quiz.Description;
        existingQuiz.Category = quiz.Category;
        existingQuiz.PassingScore = quiz.PassingScore;
        existingQuiz.TimeLimitMinutes = quiz.TimeLimitMinutes;
        existingQuiz.IsActive = quiz.IsActive;
        existingQuiz.CourseId = quiz.CourseId;
        existingQuiz.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated quiz {QuizId} - {Title}", quizId, quiz.Title);
        return true;
    }

    public async Task<bool> DeleteQuizAsync(int quizId)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == quizId);

        if (quiz == null) return false;

        // Delete associated media files
        foreach (var question in quiz.Questions)
        {
            if (!string.IsNullOrEmpty(question.QuestionImageUrl))
                await DeleteMediaAsync(question.QuestionImageUrl);
            if (!string.IsNullOrEmpty(question.QuestionAudioUrl))
                await DeleteMediaAsync(question.QuestionAudioUrl);
            if (!string.IsNullOrEmpty(question.ExplanationImageUrl))
                await DeleteMediaAsync(question.ExplanationImageUrl);
        }

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted quiz {QuizId}", quizId);
        return true;
    }

    #endregion

    #region Question CRUD Operations

    public async Task<List<QuestionManagementDto>> GetQuestionsByQuizIdAsync(int quizId)
    {
        var questions = await _context.Questions
            .Include(q => q.AnswerOptions.OrderBy(a => a.OrderIndex))
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();

        return questions.Select(q => MapToQuestionManagementDto(q)).ToList();
    }

    public async Task<QuestionManagementDto?> GetQuestionByIdAsync(int questionId)
    {
        var question = await _context.Questions
            .Include(q => q.AnswerOptions.OrderBy(a => a.OrderIndex))
            .FirstOrDefaultAsync(q => q.Id == questionId);

        return question == null ? null : MapToQuestionManagementDto(question);
    }

    public async Task<int> CreateQuestionAsync(int quizId, QuestionUpdateDto question)
    {
        var quiz = await _context.Quizzes.FindAsync(quizId);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {quizId} not found");

        var newQuestion = new Question
        {
            QuizId = quizId,
            QuestionText = question.QuestionText,
            QuestionType = Enum.Parse<QuestionType>(question.QuestionType),
            Explanation = question.Explanation,
            ExplanationImageUrl = question.ExplanationImageUrl,
            QuestionImageUrl = question.QuestionImageUrl,
            QuestionAudioUrl = question.QuestionAudioUrl,
            Points = question.Points,
            OrderIndex = question.OrderIndex,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Questions.Add(newQuestion);
        await _context.SaveChangesAsync();

        // Add answer options
        foreach (var option in question.AnswerOptions)
        {
            var answerOption = new AnswerOption
            {
                QuestionId = newQuestion.Id,
                OptionText = option.OptionText,
                IsCorrect = option.IsCorrect,
                OrderIndex = option.OrderIndex
            };
            _context.AnswerOptions.Add(answerOption);
        }

        await _context.SaveChangesAsync();

        // Update quiz timestamp
        quiz.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created question {QuestionId} for quiz {QuizId}", newQuestion.Id, quizId);
        return newQuestion.Id;
    }

    public async Task<bool> UpdateQuestionAsync(int questionId, QuestionUpdateDto question)
    {
        var existingQuestion = await _context.Questions
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (existingQuestion == null) return false;

        existingQuestion.QuestionText = question.QuestionText;
        existingQuestion.QuestionType = Enum.Parse<QuestionType>(question.QuestionType);
        existingQuestion.Explanation = question.Explanation;
        existingQuestion.ExplanationImageUrl = question.ExplanationImageUrl;
        existingQuestion.QuestionImageUrl = question.QuestionImageUrl;
        existingQuestion.QuestionAudioUrl = question.QuestionAudioUrl;
        existingQuestion.Points = question.Points;
        existingQuestion.OrderIndex = question.OrderIndex;
        existingQuestion.UpdatedAt = DateTime.UtcNow;

        // Update answer options
        var existingOptionIds = existingQuestion.AnswerOptions.Select(a => a.Id).ToList();
        var updatedOptionIds = question.AnswerOptions.Where(a => a.Id.HasValue).Select(a => a.Id!.Value).ToList();

        // Remove deleted options
        var optionsToRemove = existingQuestion.AnswerOptions
            .Where(a => !updatedOptionIds.Contains(a.Id))
            .ToList();
        _context.AnswerOptions.RemoveRange(optionsToRemove);

        // Update or add options
        foreach (var optionDto in question.AnswerOptions)
        {
            if (optionDto.Id.HasValue)
            {
                // Update existing option
                var existingOption = existingQuestion.AnswerOptions
                    .FirstOrDefault(a => a.Id == optionDto.Id.Value);
                if (existingOption != null)
                {
                    existingOption.OptionText = optionDto.OptionText;
                    existingOption.IsCorrect = optionDto.IsCorrect;
                    existingOption.OrderIndex = optionDto.OrderIndex;
                }
            }
            else
            {
                // Add new option
                var newOption = new AnswerOption
                {
                    QuestionId = questionId,
                    OptionText = optionDto.OptionText,
                    IsCorrect = optionDto.IsCorrect,
                    OrderIndex = optionDto.OrderIndex
                };
                _context.AnswerOptions.Add(newOption);
            }
        }

        await _context.SaveChangesAsync();

        // Update quiz timestamp
        var quiz = await _context.Quizzes.FindAsync(existingQuestion.QuizId);
        if (quiz != null)
        {
            quiz.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Updated question {QuestionId}", questionId);
        return true;
    }

    public async Task<bool> DeleteQuestionAsync(int questionId)
    {
        var question = await _context.Questions
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null) return false;

        // Delete associated media files
        if (!string.IsNullOrEmpty(question.QuestionImageUrl))
            await DeleteMediaAsync(question.QuestionImageUrl);
        if (!string.IsNullOrEmpty(question.QuestionAudioUrl))
            await DeleteMediaAsync(question.QuestionAudioUrl);
        if (!string.IsNullOrEmpty(question.ExplanationImageUrl))
            await DeleteMediaAsync(question.ExplanationImageUrl);

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        // Update quiz timestamp
        var quiz = await _context.Quizzes.FindAsync(question.QuizId);
        if (quiz != null)
        {
            quiz.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Deleted question {QuestionId}", questionId);
        return true;
    }

    public async Task<bool> ReorderQuestionsAsync(int quizId, List<int> questionIds)
    {
        var questions = await _context.Questions
            .Where(q => q.QuizId == quizId)
            .ToListAsync();

        for (int i = 0; i < questionIds.Count; i++)
        {
            var question = questions.FirstOrDefault(q => q.Id == questionIds[i]);
            if (question != null)
            {
                question.OrderIndex = i;
                question.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reordered questions for quiz {QuizId}", quizId);
        return true;
    }

    #endregion

    #region Media Upload Operations

    public async Task<MediaUploadResult> UploadQuestionImageAsync(Stream imageStream, string fileName)
    {
        try
        {
            // Validate file size
            if (imageStream.Length > MaxImageSizeBytes)
            {
                return new MediaUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Image size exceeds maximum allowed size of 1MB"
                };
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "questions", "images");
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageStream.CopyToAsync(fileStream);
            }

            var url = $"/uploads/questions/images/{uniqueFileName}";
            _logger.LogInformation("Uploaded question image: {Url}", url);

            return new MediaUploadResult
            {
                Success = true,
                Url = url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading question image");
            return new MediaUploadResult
            {
                Success = false,
                ErrorMessage = "Failed to upload image"
            };
        }
    }

    public async Task<MediaUploadResult> UploadQuestionAudioAsync(Stream audioStream, string fileName)
    {
        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "questions", "audio");
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await audioStream.CopyToAsync(fileStream);
            }

            var url = $"/uploads/questions/audio/{uniqueFileName}";
            _logger.LogInformation("Uploaded question audio: {Url}", url);

            return new MediaUploadResult
            {
                Success = true,
                Url = url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading question audio");
            return new MediaUploadResult
            {
                Success = false,
                ErrorMessage = "Failed to upload audio"
            };
        }
    }

    public async Task<bool> DeleteMediaAsync(string mediaUrl)
    {
        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, mediaUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted media file: {FilePath}", filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting media file: {MediaUrl}", mediaUrl);
            return false;
        }
    }

    #endregion

    #region Private Helper Methods

    private QuizManagementDto MapToQuizManagementDto(Quiz quiz)
    {
        return new QuizManagementDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            Category = quiz.Category,
            PassingScore = quiz.PassingScore,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            IsActive = quiz.IsActive,
            CreatedAt = quiz.CreatedAt,
            UpdatedAt = quiz.UpdatedAt,
            CourseId = quiz.CourseId,
            TotalQuestions = quiz.Questions?.Count ?? 0,
            Questions = quiz.Questions?.Select(q => MapToQuestionManagementDto(q)).ToList() ?? new()
        };
    }

    private QuestionManagementDto MapToQuestionManagementDto(Question question)
    {
        return new QuestionManagementDto
        {
            Id = question.Id,
            QuizId = question.QuizId,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType.ToString(),
            Explanation = question.Explanation,
            ExplanationImageUrl = question.ExplanationImageUrl,
            QuestionImageUrl = question.QuestionImageUrl,
            QuestionAudioUrl = question.QuestionAudioUrl,
            Points = question.Points,
            OrderIndex = question.OrderIndex,
            AnswerOptions = question.AnswerOptions?.Select(a => new AnswerOptionDto
            {
                Id = a.Id,
                OptionText = a.OptionText,
                IsCorrect = a.IsCorrect,
                OrderIndex = a.OrderIndex
            }).ToList() ?? new()
        };
    }

    #endregion
}
