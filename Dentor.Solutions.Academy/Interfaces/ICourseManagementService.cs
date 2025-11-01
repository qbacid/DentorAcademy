using Dentor.Solutions.Academy.DTOs.Course;
using Dentor.Solutions.Academy.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Dentor.Solutions.Academy.Interfaces;

/// <summary>
/// Interface for course category management operations
/// </summary>
public interface ICourseCategoryService
{
    Task<List<CourseCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false);
    Task<CourseCategoryDto?> GetCategoryByIdAsync(int id);
    Task<InternalOperationResult> CreateCategoryAsync(CreateCourseCategoryDto dto);
    Task<InternalOperationResult> UpdateCategoryAsync(UpdateCourseCategoryDto dto);
    Task<InternalOperationResult> DeleteCategoryAsync(int id);
    Task<InternalOperationResult> ReorderCategoriesAsync(List<int> categoryIds);
    Task<int> GetCourseCountByCategoryAsync(int categoryId);
    Task<List<Quiz>> GetQuizzesByCategoryAsync(int categoryId);
    Task<int> GetQuizCountByCategoryAsync(int categoryId);
}

/// <summary>
/// Interface for course management operations
/// </summary>
public interface ICourseManagementService
{
    // Course CRUD
    Task<List<CourseListDto>> GetAllCoursesAsync(int? categoryId = null, bool? isPublished = null);
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<InternalOperationResult> CreateCourseAsync(CourseDto dto, string userId);
    Task<InternalOperationResult> UpdateCourseAsync(CourseDto dto);
    Task<InternalOperationResult> DeleteCourseAsync(int id);
    Task<InternalOperationResult> PublishCourseAsync(int id);
    Task<InternalOperationResult> UnpublishCourseAsync(int id);
    
    // Module Management
    Task<List<CourseModuleDto>> GetCourseModulesAsync(int courseId);
    Task<CourseModuleDto?> GetModuleByIdAsync(int id);
    Task<InternalOperationResult> CreateModuleAsync(CourseModuleDto dto);
    Task<InternalOperationResult> UpdateModuleAsync(CourseModuleDto dto);
    Task<InternalOperationResult> DeleteModuleAsync(int id);
    Task<InternalOperationResult> ReorderModulesAsync(int courseId, List<int> moduleIds);
    
    // Content Management
    Task<List<CourseContentDto>> GetModuleContentsAsync(int moduleId);
    Task<CourseContentDto?> GetContentByIdAsync(int id);
    Task<InternalOperationResult> CreateContentAsync(CourseContentDto dto);
    Task<InternalOperationResult> UpdateContentAsync(CourseContentDto dto);
    Task<InternalOperationResult> DeleteContentAsync(int id);
    Task<InternalOperationResult> ReorderContentsAsync(int moduleId, List<int> contentIds);
    
    // Enrollment Management
    Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId);
    Task<InternalOperationResult> EnrollUserAsync(int courseId, string userId);
    Task<InternalOperationResult> BulkEnrollUsersAsync(int courseId, List<string> userIds);
    Task<InternalOperationResult> UnenrollUserAsync(int enrollmentId);
    Task<List<int>> GetUserEnrolledCourseIdsAsync(string userId);
    
    // Image Management
    Task<InternalOperationResult> UploadThumbnailImageAsync(int courseId, IBrowserFile file);
    Task<InternalOperationResult> UploadCoverImageAsync(int courseId, IBrowserFile file);
}
