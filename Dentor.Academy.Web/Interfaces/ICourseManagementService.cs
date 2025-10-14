using Dentor.Academy.Web.DTOs.Course;
using Microsoft.AspNetCore.Components.Forms;

namespace Dentor.Academy.Web.Interfaces;

/// <summary>
/// Interface for course category management operations
/// </summary>
public interface ICourseCategoryService
{
    Task<List<CourseCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false);
    Task<CourseCategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryOperationResult> CreateCategoryAsync(CreateCourseCategoryDto dto);
    Task<CategoryOperationResult> UpdateCategoryAsync(UpdateCourseCategoryDto dto);
    Task<CategoryOperationResult> DeleteCategoryAsync(int id);
    Task<CategoryOperationResult> ReorderCategoriesAsync(List<int> categoryIds);
    Task<int> GetCourseCountByCategoryAsync(int categoryId);
}

/// <summary>
/// Interface for course management operations
/// </summary>
public interface ICourseManagementService
{
    // Course CRUD
    Task<List<CourseListDto>> GetAllCoursesAsync(int? categoryId = null, bool? isPublished = null);
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CategoryOperationResult> CreateCourseAsync(CourseDto dto, string userId);
    Task<CategoryOperationResult> UpdateCourseAsync(CourseDto dto);
    Task<CategoryOperationResult> DeleteCourseAsync(int id);
    Task<CategoryOperationResult> PublishCourseAsync(int id);
    Task<CategoryOperationResult> UnpublishCourseAsync(int id);
    
    // Module Management
    Task<List<CourseModuleDto>> GetCourseModulesAsync(int courseId);
    Task<CourseModuleDto?> GetModuleByIdAsync(int id);
    Task<CategoryOperationResult> CreateModuleAsync(CourseModuleDto dto);
    Task<CategoryOperationResult> UpdateModuleAsync(CourseModuleDto dto);
    Task<CategoryOperationResult> DeleteModuleAsync(int id);
    Task<CategoryOperationResult> ReorderModulesAsync(int courseId, List<int> moduleIds);
    
    // Content Management
    Task<List<CourseContentDto>> GetModuleContentsAsync(int moduleId);
    Task<CourseContentDto?> GetContentByIdAsync(int id);
    Task<CategoryOperationResult> CreateContentAsync(CourseContentDto dto);
    Task<CategoryOperationResult> UpdateContentAsync(CourseContentDto dto);
    Task<CategoryOperationResult> DeleteContentAsync(int id);
    Task<CategoryOperationResult> ReorderContentsAsync(int moduleId, List<int> contentIds);
    
    // Enrollment Management
    Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId);
    Task<CategoryOperationResult> EnrollUserAsync(int courseId, string userId);
    Task<CategoryOperationResult> BulkEnrollUsersAsync(int courseId, List<string> userIds);
    Task<CategoryOperationResult> UnenrollUserAsync(int enrollmentId);
    Task<List<int>> GetUserEnrolledCourseIdsAsync(string userId);
    
    // Image Management
    Task<CategoryOperationResult> UploadThumbnailImageAsync(int courseId, IBrowserFile file);
    Task<CategoryOperationResult> UploadCoverImageAsync(int courseId, IBrowserFile file);
}
