using Dentor.Academy.Web.Models;

namespace Dentor.Academy.Web.DTOs.Course;

/// <summary>
/// DTO for creating a new course category
/// </summary>
public class CreateCourseCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; } = "bi bi-folder";
    public string? Color { get; set; } = "#0066CC";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for updating a course category
/// </summary>
public class UpdateCourseCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for displaying course category with course count
/// </summary>
public class CourseCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int CourseCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for course list item
/// </summary>
public class CourseListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public int? CategoryId { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public decimal EstimatedDurationHours { get; set; }
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public int EnrollmentCount { get; set; }
    public int ModuleCount { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating/updating a course
/// </summary>
public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? FullDescription { get; set; }
    public int? CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    // Image data for upload
    public byte[]? ThumbnailImageData { get; set; }
    public string? ThumbnailContentType { get; set; }
    public byte[]? CoverImageData { get; set; }
    public string? CoverImageContentType { get; set; }
    
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Beginner;
    public decimal EstimatedDurationHours { get; set; }
    public decimal Price { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? LearningObjectives { get; set; }
    public List<string>? Prerequisites { get; set; }
}

/// <summary>
/// DTO for course module
/// </summary>
public class CourseModuleDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool IsPublished { get; set; } = true;
    public int ContentCount { get; set; }
    public List<CourseContentDto> Contents { get; set; } = new();
}

/// <summary>
/// DTO for course content
/// </summary>
public class CourseContentDto
{
    public int Id { get; set; }
    public int CourseModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContentType ContentType { get; set; }
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public string? BlobUrl { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? MimeType { get; set; }
    public int? QuizId { get; set; }
    public string? ExternalUrl { get; set; }
    public bool IsFreePreview { get; set; }
    public bool IsDownloadable { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsPublished { get; set; } = true;
}

/// <summary>
/// DTO for enrollment
/// </summary>
public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Result of category operation
/// </summary>
public class CategoryOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public CourseCategoryDto? Category { get; set; }
}
