using Dentor.Solutions.Academy.Data;
using Dentor.Solutions.Academy.DTOs.Course;
using Dentor.Solutions.Academy.Interfaces;
using Dentor.Solutions.Academy.Models;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Services;

public class CourseContentService : ICourseContentService
{
    private readonly ApplicationDbContext _context;

    public CourseContentService(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Module Management

    public async Task<CourseModuleDto> GetModuleByIdAsync(int moduleId)
    {
        var module = await _context.CourseModules
            .Include(m => m.Contents.OrderBy(c => c.OrderIndex))
            .FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null)
            throw new KeyNotFoundException($"Module with ID {moduleId} not found");

        return MapToModuleDto(module);
    }

    public async Task<List<CourseModuleDto>> GetCourseModulesAsync(int courseId)
    {
        var modules = await _context.CourseModules
            .Include(m => m.Contents.OrderBy(c => c.OrderIndex))
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.OrderIndex)
            .ToListAsync();

        return modules.Select(MapToModuleDto).ToList();
    }

    public async Task<CourseModuleDto> CreateModuleAsync(CreateCourseModuleDto dto)
    {
        var module = new CourseModule
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Description = dto.Description,
            OrderIndex = dto.OrderIndex,
            EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CourseModules.Add(module);
        await _context.SaveChangesAsync();

        return MapToModuleDto(module);
    }

    public async Task<CourseModuleDto> UpdateModuleAsync(int moduleId, UpdateCourseModuleDto dto)
    {
        var module = await _context.CourseModules.FindAsync(moduleId);
        if (module == null)
            throw new KeyNotFoundException($"Module with ID {moduleId} not found");

        module.Title = dto.Title;
        module.Description = dto.Description;
        module.OrderIndex = dto.OrderIndex;
        module.EstimatedDurationMinutes = dto.EstimatedDurationMinutes;
        module.IsPublished = dto.IsPublished;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetModuleByIdAsync(moduleId);
    }

    public async Task DeleteModuleAsync(int moduleId)
    {
        var module = await _context.CourseModules
            .Include(m => m.Contents)
            .FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null)
            throw new KeyNotFoundException($"Module with ID {moduleId} not found");

        _context.CourseModules.Remove(module);
        await _context.SaveChangesAsync();
    }

    public async Task ReorderModulesAsync(int courseId, List<int> moduleIds)
    {
        var modules = await _context.CourseModules
            .Where(m => m.CourseId == courseId)
            .ToListAsync();

        for (int i = 0; i < moduleIds.Count; i++)
        {
            var module = modules.FirstOrDefault(m => m.Id == moduleIds[i]);
            if (module != null)
            {
                module.OrderIndex = i;
                module.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Content Management

    public async Task<CourseContentDto> GetContentByIdAsync(int contentId)
    {
        var content = await _context.CourseContents
            .FirstOrDefaultAsync(c => c.Id == contentId);

        if (content == null)
            throw new KeyNotFoundException($"Content with ID {contentId} not found");

        return MapToContentDto(content);
    }

    public async Task<List<CourseContentDto>> GetModuleContentsAsync(int moduleId)
    {
        var contents = await _context.CourseContents
            .Where(c => c.CourseModuleId == moduleId)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync();

        return contents.Select(MapToContentDto).ToList();
    }

    public async Task<CourseContentDto> CreateContentAsync(CreateCourseContentDto dto)
    {
        var content = new CourseContent
        {
            CourseModuleId = dto.CourseModuleId,
            Title = dto.Title,
            Description = dto.Description,
            ContentMimeType = dto.ContentMimeType,
            OrderIndex = dto.OrderIndex,
            DurationMinutes = dto.DurationMinutes,
            ExternalUrl = dto.ExternalUrl,
            QuizId = dto.QuizId,
            IsFreePreview = dto.IsFreePreview,
            IsDownloadable = dto.IsDownloadable,
            IsMandatory = dto.IsMandatory,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CourseContents.Add(content);
        await _context.SaveChangesAsync();

        return MapToContentDto(content);
    }

    public async Task<CourseContentDto> UpdateContentAsync(int contentId, UpdateCourseContentDto dto)
    {
        var content = await _context.CourseContents.FindAsync(contentId);
        if (content == null)
            throw new KeyNotFoundException($"Content with ID {contentId} not found");

        content.Title = dto.Title;
        content.Description = dto.Description;
        content.OrderIndex = dto.OrderIndex;
        content.DurationMinutes = dto.DurationMinutes;
        content.ExternalUrl = dto.ExternalUrl;
        content.QuizId = dto.QuizId;
        content.IsFreePreview = dto.IsFreePreview;
        content.IsDownloadable = dto.IsDownloadable;
        content.IsMandatory = dto.IsMandatory;
        content.IsPublished = dto.IsPublished;
        content.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToContentDto(content);
    }

    public async Task DeleteContentAsync(int contentId)
    {
        var content = await _context.CourseContents.FindAsync(contentId);
        if (content == null)
            throw new KeyNotFoundException($"Content with ID {contentId} not found");

        _context.CourseContents.Remove(content);
        await _context.SaveChangesAsync();
    }

    public async Task ReorderContentsAsync(int moduleId, List<int> contentIds)
    {
        var contents = await _context.CourseContents
            .Where(c => c.CourseModuleId == moduleId)
            .ToListAsync();

        for (int i = 0; i < contentIds.Count; i++)
        {
            var content = contents.FirstOrDefault(c => c.Id == contentIds[i]);
            if (content != null)
            {
                content.OrderIndex = i;
                content.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<string> UploadContentFileAsync(int contentId, Stream fileStream, string fileName, ContentMimeType contentType)
    {
        var content = await _context.CourseContents.FindAsync(contentId);
        if (content == null)
            throw new KeyNotFoundException($"Content with ID {contentId} not found");

        // TODO: Implement Azure Blob Storage upload
        // For now, return a placeholder URL
        var blobUrl = $"/content/{contentId}/{fileName}";
        
        content.BlobUrl = blobUrl;
        content.ContentMimeType = contentType;
        content.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return blobUrl;
    }

    #endregion

    #region Course Structure

    public async Task<CourseStructureDto> GetCourseStructureAsync(int courseId, string? userId = null)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            throw new KeyNotFoundException($"Course with ID {courseId} not found");

        var modules = await _context.CourseModules
            .Include(m => m.Contents)
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.OrderIndex)
            .ToListAsync();

        // Get user progress if userId is provided
        Dictionary<int, bool> contentProgress = new();
        // TODO: Implement progress tracking when CourseProgress table is ready
        // if (!string.IsNullOrEmpty(userId))
        // {
        //     var progress = await _context.CourseProgresses
        //         .Where(p => p.UserId == userId && p.CourseId == courseId && p.IsCompleted)
        //         .Select(p => p.ContentId)
        //         .ToListAsync();
        //     contentProgress = progress.ToDictionary(id => id, _ => true);
        // }

        var moduleDtos = modules.Select(m => 
        {
            var dto = MapToModuleDto(m);
            // Mark completed content
            foreach (var content in dto.Contents)
            {
                content.IsCompleted = contentProgress.ContainsKey(content.Id);
            }
            return dto;
        }).ToList();

        var totalDuration = modules.Sum(m => m.EstimatedDurationMinutes ?? 0);
        var totalLectures = modules.Sum(m => m.Contents.Count);

        return new CourseStructureDto
        {
            CourseId = courseId,
            CourseTitle = course.Title,
            Modules = moduleDtos,
            TotalDurationMinutes = totalDuration,
            TotalLectures = totalLectures
        };
    }

    #endregion

    #region Mapping Helpers

    private CourseModuleDto MapToModuleDto(CourseModule module)
    {
        return new CourseModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            OrderIndex = module.OrderIndex,
            EstimatedDurationMinutes = module.EstimatedDurationMinutes,
            IsPublished = module.IsPublished,
            Contents = module.Contents?.OrderBy(c => c.OrderIndex).Select(MapToContentDto).ToList() ?? new()
        };
    }

    private CourseContentDto MapToContentDto(CourseContent content)
    {
        return new CourseContentDto
        {
            Id = content.Id,
            CourseModuleId = content.CourseModuleId,
            Title = content.Title,
            Description = content.Description,
            CourseContentType = content.CourseContentType,
            ContentMimeType = content.ContentMimeType,
            OrderIndex = content.OrderIndex,
            DurationMinutes = content.DurationMinutes,
            BlobUrl = content.BlobUrl,
            ExternalUrl = content.ExternalUrl,
            QuizId = content.QuizId,
            FileSizeBytes = content.FileSizeBytes,
            IsFreePreview = content.IsFreePreview,
            IsDownloadable = content.IsDownloadable,
            IsMandatory = content.IsMandatory,
            IsPublished = content.IsPublished,
            IsCompleted = false // Will be set by GetCourseStructureAsync if needed
        };
    }

    #endregion
}
