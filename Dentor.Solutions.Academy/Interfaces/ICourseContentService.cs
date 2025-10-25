using Dentor.Solutions.Academy.DTOs.Course;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Service for managing course content structure (modules and content items)
/// </summary>
public interface ICourseContentService
{
    // Module Management
    Task<CourseModuleDto> GetModuleByIdAsync(int moduleId);
    Task<List<CourseModuleDto>> GetCourseModulesAsync(int courseId);
    Task<CourseModuleDto> CreateModuleAsync(CreateCourseModuleDto dto);
    Task<CourseModuleDto> UpdateModuleAsync(int moduleId, UpdateCourseModuleDto dto);
    Task DeleteModuleAsync(int moduleId);
    Task ReorderModulesAsync(int courseId, List<int> moduleIds);

    // Content Management
    Task<CourseContentDto> GetContentByIdAsync(int contentId);
    Task<List<CourseContentDto>> GetModuleContentsAsync(int moduleId);
    Task<CourseContentDto> CreateContentAsync(CreateCourseContentDto dto);
    Task<CourseContentDto> UpdateContentAsync(int contentId, UpdateCourseContentDto dto);
    Task DeleteContentAsync(int contentId);
    Task ReorderContentsAsync(int moduleId, List<int> contentIds);
    Task<string> UploadContentFileAsync(int contentId, Stream fileStream, string fileName, string contentType);
    
    // Course Structure
    Task<CourseStructureDto> GetCourseStructureAsync(int courseId, string? userId = null);
}

