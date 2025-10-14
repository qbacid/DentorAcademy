using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.DTOs.Course;
using Dentor.Academy.Web.Interfaces;
using Dentor.Academy.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

namespace Dentor.Academy.Web.Services;

/// <summary>
/// Service for managing courses, modules, content, and enrollments
/// </summary>
public class CourseManagementService : ICourseManagementService
{
    private readonly IDbContextFactory<QuizDbContext> _contextFactory;
    private readonly ILogger<CourseManagementService> _logger;

    public CourseManagementService(IDbContextFactory<QuizDbContext> contextFactory, ILogger<CourseManagementService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    #region Course CRUD

    public async Task<List<CourseListDto>> GetAllCoursesAsync(int? categoryId = null, bool? isPublished = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.Courses
            .Include(c => c.CourseCategory)
            .Include(c => c.CreatedBy)
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        if (isPublished.HasValue)
        {
            query = query.Where(c => c.IsPublished == isPublished.Value);
        }

        var courses = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return courses.Select(c => new CourseListDto
        {
            Id = c.Id,
            Title = c.Title,
            ShortDescription = c.ShortDescription,
            ThumbnailUrl = c.ThumbnailUrl,
            Category = c.CourseCategory != null ? c.CourseCategory.Name : c.Category,
            CategoryId = c.CategoryId,
            DifficultyLevel = Enum.Parse<DifficultyLevel>(c.DifficultyLevel, true),
            EstimatedDurationHours = c.EstimatedDurationHours ?? 0,
            Price = c.Price,
            IsPublished = c.IsPublished,
            IsFeatured = c.IsFeatured,
            EnrollmentCount = c.Enrollments.Count,
            ModuleCount = c.Modules.Count,
            CreatedByUserName = c.CreatedBy != null ? c.CreatedBy.UserName : null,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var course = await context.Courses
            .Include(c => c.CourseCategory)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            ShortDescription = course.ShortDescription,
            FullDescription = course.FullDescription,
            CategoryId = course.CategoryId,
            ThumbnailUrl = course.ThumbnailUrl,
            CoverImageUrl = course.CoverImageUrl,
            DifficultyLevel = Enum.Parse<DifficultyLevel>(course.DifficultyLevel),
            EstimatedDurationHours = course.EstimatedDurationHours ?? 0,
            Price = course.Price,
            IsPublished = course.IsPublished,
            IsFeatured = course.IsFeatured,
            Tags = course.Tags != null ? course.Tags.Split(',').ToList() : new List<string>(),
            LearningObjectives = course.LearningObjectives != null ? course.LearningObjectives.Split(',').ToList() : new List<string>(),
            Prerequisites = course.Prerequisites != null ? course.Prerequisites.Split(',').ToList() : new List<string>()
        };
    }

    public async Task<CategoryOperationResult> CreateCourseAsync(CourseDto dto, string userId)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = new Course
            {
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                CategoryId = dto.CategoryId,
                ThumbnailUrl = dto.ThumbnailUrl,
                CoverImageUrl = dto.CoverImageUrl,
                // Store uploaded images
                ThumbnailImage = dto.ThumbnailImageData,
                ThumbnailContentType = dto.ThumbnailContentType,
                CoverImage = dto.CoverImageData,
                CoverImageContentType = dto.CoverImageContentType,
                DifficultyLevel = dto.DifficultyLevel.ToString(),
                EstimatedDurationHours = (int)dto.EstimatedDurationHours,
                Price = dto.Price,
                IsPublished = dto.IsPublished,
                IsFeatured = dto.IsFeatured,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null,
                LearningObjectives = dto.LearningObjectives != null ? string.Join(",", dto.LearningObjectives) : null,
                Prerequisites = dto.Prerequisites != null ? string.Join(",", dto.Prerequisites) : null
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Course created successfully";
            _logger.LogInformation("Created course: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course: {CourseTitle}", dto.Title);
            result.Errors.Add($"Error creating course: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UpdateCourseAsync(CourseDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses.FindAsync(dto.Id);
            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            course.Title = dto.Title;
            course.ShortDescription = dto.ShortDescription;
            course.FullDescription = dto.FullDescription;
            course.CategoryId = dto.CategoryId;
            course.ThumbnailUrl = dto.ThumbnailUrl;
            course.CoverImageUrl = dto.CoverImageUrl;
            
            // Update images if new ones are provided
            if (dto.ThumbnailImageData != null)
            {
                course.ThumbnailImage = dto.ThumbnailImageData;
                course.ThumbnailContentType = dto.ThumbnailContentType;
            }
            if (dto.CoverImageData != null)
            {
                course.CoverImage = dto.CoverImageData;
                course.CoverImageContentType = dto.CoverImageContentType;
            }
            
            course.DifficultyLevel = dto.DifficultyLevel.ToString();
            course.EstimatedDurationHours = (int)dto.EstimatedDurationHours;
            course.Price = dto.Price;
            course.IsPublished = dto.IsPublished;
            course.IsFeatured = dto.IsFeatured;
            course.UpdatedAt = DateTime.UtcNow;
            course.Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null;
            course.LearningObjectives = dto.LearningObjectives != null ? string.Join(",", dto.LearningObjectives) : null;
            course.Prerequisites = dto.Prerequisites != null ? string.Join(",", dto.Prerequisites) : null;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Course updated successfully";
            _logger.LogInformation("Updated course: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course: {CourseId}", dto.Id);
            result.Errors.Add($"Error updating course: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> DeleteCourseAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            if (course.Enrollments.Any())
            {
                result.Errors.Add($"Cannot delete course with active enrollments. Please unenroll all students first.");
                return result;
            }

            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Course deleted successfully";
            _logger.LogInformation("Deleted course: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course: {CourseId}", id);
            result.Errors.Add($"Error deleting course: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> PublishCourseAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses.FindAsync(id);
            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            course.IsPublished = true;
            course.PublishedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Course published successfully";
            _logger.LogInformation("Published course: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing course: {CourseId}", id);
            result.Errors.Add($"Error publishing course: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UnpublishCourseAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses.FindAsync(id);
            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            course.IsPublished = false;
            course.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Course unpublished successfully";
            _logger.LogInformation("Unpublished course: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing course: {CourseId}", id);
            result.Errors.Add($"Error unpublishing course: {ex.Message}");
        }

        return result;
    }

    #endregion

    #region Module Management

    public async Task<List<CourseModuleDto>> GetCourseModulesAsync(int courseId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var modules = await context.CourseModules
            .Where(m => m.CourseId == courseId)
            .Include(m => m.Contents)
            .OrderBy(m => m.OrderIndex)
            .ToListAsync();

        return modules.Select(m => new CourseModuleDto
        {
            Id = m.Id,
            CourseId = m.CourseId,
            Title = m.Title,
            Description = m.Description,
            OrderIndex = m.OrderIndex,
            EstimatedDurationMinutes = m.EstimatedDurationMinutes ?? 0,
            IsPublished = m.IsPublished,
            ContentCount = m.Contents.Count,
            Contents = m.Contents.OrderBy(c => c.OrderIndex).Select(c => new CourseContentDto
            {
                Id = c.Id,
                CourseModuleId = c.CourseModuleId,
                Title = c.Title,
                Description = c.Description,
                ContentType = Enum.Parse<ContentType>(c.ContentType, true),
                OrderIndex = c.OrderIndex,
                DurationMinutes = c.DurationMinutes ?? 0,
                BlobUrl = c.BlobUrl,
                FileSizeBytes = c.FileSizeBytes,
                MimeType = c.MimeType,
                QuizId = c.QuizId,
                ExternalUrl = c.ExternalUrl,
                IsFreePreview = c.IsFreePreview,
                IsDownloadable = c.IsDownloadable,
                IsMandatory = c.IsMandatory,
                IsPublished = c.IsPublished
            }).ToList()
        }).ToList();
    }

    public async Task<CourseModuleDto?> GetModuleByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var module = await context.CourseModules
            .Include(m => m.Contents)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module == null) return null;

        return new CourseModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            OrderIndex = module.OrderIndex,
            EstimatedDurationMinutes = module.EstimatedDurationMinutes ?? 0,
            IsPublished = module.IsPublished,
            ContentCount = module.Contents.Count,
            Contents = module.Contents.OrderBy(c => c.OrderIndex).Select(c => new CourseContentDto
            {
                Id = c.Id,
                CourseModuleId = c.CourseModuleId,
                Title = c.Title,
                ContentType = Enum.Parse<ContentType>(c.ContentType),
                OrderIndex = c.OrderIndex,
                IsPublished = c.IsPublished
            }).ToList()
        };
    }

    public async Task<CategoryOperationResult> CreateModuleAsync(CourseModuleDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var module = new CourseModule
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                OrderIndex = dto.OrderIndex,
                EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow
            };

            context.CourseModules.Add(module);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Module created successfully";
            _logger.LogInformation("Created module: {ModuleTitle} for course {CourseId}", module.Title, module.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating module: {ModuleTitle}", dto.Title);
            result.Errors.Add($"Error creating module: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UpdateModuleAsync(CourseModuleDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var module = await context.CourseModules.FindAsync(dto.Id);
            if (module == null)
            {
                result.Errors.Add("Module not found");
                return result;
            }

            module.Title = dto.Title;
            module.Description = dto.Description;
            module.OrderIndex = dto.OrderIndex;
            module.EstimatedDurationMinutes = dto.EstimatedDurationMinutes;
            module.IsPublished = dto.IsPublished;
            module.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Module updated successfully";
            _logger.LogInformation("Updated module: {ModuleTitle} (ID: {ModuleId})", module.Title, module.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating module: {ModuleId}", dto.Id);
            result.Errors.Add($"Error updating module: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> DeleteModuleAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var module = await context.CourseModules
                .Include(m => m.Contents)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (module == null)
            {
                result.Errors.Add("Module not found");
                return result;
            }

            context.CourseModules.Remove(module);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Module deleted successfully";
            _logger.LogInformation("Deleted module: {ModuleTitle} (ID: {ModuleId})", module.Title, module.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting module: {ModuleId}", id);
            result.Errors.Add($"Error deleting module: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> ReorderModulesAsync(int courseId, List<int> moduleIds)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var modules = await context.CourseModules
                .Where(m => m.CourseId == courseId && moduleIds.Contains(m.Id))
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

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Modules reordered successfully";
            _logger.LogInformation("Reordered {Count} modules for course {CourseId}", moduleIds.Count, courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering modules for course {CourseId}", courseId);
            result.Errors.Add($"Error reordering modules: {ex.Message}");
        }

        return result;
    }

    #endregion

    #region Content Management

    public async Task<List<CourseContentDto>> GetModuleContentsAsync(int moduleId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var contents = await context.CourseContents
            .Where(c => c.CourseModuleId == moduleId)
            .OrderBy(c => c.OrderIndex)
            .Select(c => new CourseContentDto
            {
                Id = c.Id,
                CourseModuleId = c.CourseModuleId,
                Title = c.Title,
                Description = c.Description,
                ContentType = Enum.Parse<ContentType>(c.ContentType),
                OrderIndex = c.OrderIndex,
                DurationMinutes = c.DurationMinutes ?? 0,
                BlobUrl = c.BlobUrl,
                FileSizeBytes = c.FileSizeBytes,
                MimeType = c.MimeType,
                QuizId = c.QuizId,
                ExternalUrl = c.ExternalUrl,
                IsFreePreview = c.IsFreePreview,
                IsDownloadable = c.IsDownloadable,
                IsMandatory = c.IsMandatory,
                IsPublished = c.IsPublished
            })
            .ToListAsync();

        return contents;
    }

    public async Task<CourseContentDto?> GetContentByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var content = await context.CourseContents.FindAsync(id);
        if (content == null) return null;

        return new CourseContentDto
        {
            Id = content.Id,
            CourseModuleId = content.CourseModuleId,
            Title = content.Title,
            Description = content.Description,
            ContentType = Enum.Parse<ContentType>(content.ContentType),
            OrderIndex = content.OrderIndex,
            DurationMinutes = content.DurationMinutes ?? 0,
            BlobUrl = content.BlobUrl,
            FileSizeBytes = content.FileSizeBytes,
            MimeType = content.MimeType,
            QuizId = content.QuizId,
            ExternalUrl = content.ExternalUrl,
            IsFreePreview = content.IsFreePreview,
            IsDownloadable = content.IsDownloadable,
            IsMandatory = content.IsMandatory,
            IsPublished = content.IsPublished
        };
    }

    public async Task<CategoryOperationResult> CreateContentAsync(CourseContentDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var content = new CourseContent
            {
                CourseModuleId = dto.CourseModuleId,
                Title = dto.Title,
                Description = dto.Description,
                ContentType = dto.ContentType.ToString(),
                OrderIndex = dto.OrderIndex,
                DurationMinutes = dto.DurationMinutes,
                BlobContainerName = null, // Set when uploading to Azure
                BlobName = null,
                BlobUrl = dto.BlobUrl,
                FileSizeBytes = dto.FileSizeBytes,
                MimeType = dto.MimeType,
                QuizId = dto.QuizId,
                ExternalUrl = dto.ExternalUrl,
                IsFreePreview = dto.IsFreePreview,
                IsDownloadable = dto.IsDownloadable,
                IsMandatory = dto.IsMandatory,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow
            };

            context.CourseContents.Add(content);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Content created successfully";
            _logger.LogInformation("Created content: {ContentTitle} for module {ModuleId}", content.Title, content.CourseModuleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating content: {ContentTitle}", dto.Title);
            result.Errors.Add($"Error creating content: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UpdateContentAsync(CourseContentDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var content = await context.CourseContents.FindAsync(dto.Id);
            if (content == null)
            {
                result.Errors.Add("Content not found");
                return result;
            }

            content.Title = dto.Title;
            content.Description = dto.Description;
            content.ContentType = dto.ContentType.ToString();
            content.OrderIndex = dto.OrderIndex;
            content.DurationMinutes = dto.DurationMinutes;
            content.QuizId = dto.QuizId;
            content.ExternalUrl = dto.ExternalUrl;
            content.IsFreePreview = dto.IsFreePreview;
            content.IsDownloadable = dto.IsDownloadable;
            content.IsMandatory = dto.IsMandatory;
            content.IsPublished = dto.IsPublished;
            content.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Content updated successfully";
            _logger.LogInformation("Updated content: {ContentTitle} (ID: {ContentId})", content.Title, content.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content: {ContentId}", dto.Id);
            result.Errors.Add($"Error updating content: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> DeleteContentAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var content = await context.CourseContents.FindAsync(id);
            if (content == null)
            {
                result.Errors.Add("Content not found");
                return result;
            }

            context.CourseContents.Remove(content);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Content deleted successfully";
            _logger.LogInformation("Deleted content: {ContentTitle} (ID: {ContentId})", content.Title, content.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting content: {ContentId}", id);
            result.Errors.Add($"Error deleting content: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> ReorderContentsAsync(int moduleId, List<int> contentIds)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var contents = await context.CourseContents
                .Where(c => c.CourseModuleId == moduleId && contentIds.Contains(c.Id))
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

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Contents reordered successfully";
            _logger.LogInformation("Reordered {Count} contents for module {ModuleId}", contentIds.Count, moduleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering contents for module {ModuleId}", moduleId);
            result.Errors.Add($"Error reordering contents: {ex.Message}");
        }

        return result;
    }

    #endregion

    #region Enrollment Management

    public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var enrollments = await context.CourseEnrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            CourseId = e.CourseId,
            CourseName = e.Course.Title,
            UserId = e.UserId,
            UserName = e.User.UserName ?? string.Empty,
            UserEmail = e.User.Email ?? string.Empty,
            EnrolledAt = e.EnrolledAt,
            Status = Enum.Parse<EnrollmentStatus>(e.Status, true),
            ProgressPercentage = e.ProgressPercentage,
            LastAccessedAt = e.LastAccessedAt,
            CompletedAt = e.CompletedAt
        }).ToList();
    }

    public async Task<CategoryOperationResult> EnrollUserAsync(int courseId, string userId)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            // Check if already enrolled
            var existingEnrollment = await context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (existingEnrollment != null)
            {
                result.Errors.Add("User is already enrolled in this course");
                return result;
            }

            var enrollment = new CourseEnrollment
            {
                CourseId = courseId,
                UserId = userId,
                EnrolledAt = DateTime.UtcNow,
                Status = EnrollmentStatus.Active.ToString(),
                ProgressPercentage = 0,
                CreatedAt = DateTime.UtcNow
            };

            context.CourseEnrollments.Add(enrollment);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "User enrolled successfully";
            _logger.LogInformation("Enrolled user {UserId} in course {CourseId}", userId, courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling user {UserId} in course {CourseId}", userId, courseId);
            result.Errors.Add($"Error enrolling user: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> BulkEnrollUsersAsync(int courseId, List<string> userIds)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var existingEnrollments = await context.CourseEnrollments
                .Where(e => e.CourseId == courseId && userIds.Contains(e.UserId))
                .Select(e => e.UserId)
                .ToListAsync();

            var newUserIds = userIds.Except(existingEnrollments).ToList();

            if (!newUserIds.Any())
            {
                result.Errors.Add("All selected users are already enrolled");
                return result;
            }

            var enrollments = newUserIds.Select(userId => new CourseEnrollment
            {
                CourseId = courseId,
                UserId = userId,
                EnrolledAt = DateTime.UtcNow,
                Status = EnrollmentStatus.Active.ToString(),
                ProgressPercentage = 0,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            context.CourseEnrollments.AddRange(enrollments);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully enrolled {newUserIds.Count} user(s). {existingEnrollments.Count} already enrolled.";
            _logger.LogInformation("Bulk enrolled {Count} users in course {CourseId}", newUserIds.Count, courseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk enrolling users in course {CourseId}", courseId);
            result.Errors.Add($"Error enrolling users: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UnenrollUserAsync(int enrollmentId)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var enrollment = await context.CourseEnrollments.FindAsync(enrollmentId);
            if (enrollment == null)
            {
                result.Errors.Add("Enrollment not found");
                return result;
            }

            context.CourseEnrollments.Remove(enrollment);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "User unenrolled successfully";
            _logger.LogInformation("Unenrolled user from enrollment {EnrollmentId}", enrollmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unenrolling enrollment {EnrollmentId}", enrollmentId);
            result.Errors.Add($"Error unenrolling user: {ex.Message}");
        }

        return result;
    }

    public async Task<List<int>> GetUserEnrolledCourseIdsAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.CourseEnrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.CourseId)
            .ToListAsync();
    }

    #endregion

    #region Image Management

    // Allowed image extensions and MIME types per Microsoft best practices
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedImageMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

    public async Task<CategoryOperationResult> UploadThumbnailImageAsync(int courseId, IBrowserFile file)
    {
        var result = new CategoryOperationResult();

        try
        {
            // Validate file size
            if (file.Size > MaxImageSizeBytes)
            {
                result.Errors.Add($"File size must be less than {MaxImageSizeBytes / (1024 * 1024)} MB");
                return result;
            }

            // Validate content type
            if (!AllowedImageMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                result.Errors.Add("File must be a valid image (JPEG, PNG, GIF, or WebP)");
                return result;
            }

            // Validate file extension
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                result.Errors.Add("Invalid file extension. Allowed: " + string.Join(", ", AllowedImageExtensions));
                return result;
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses.FindAsync(courseId);
            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            // Read file data with size limit enforcement
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: MaxImageSizeBytes).CopyToAsync(memoryStream);
            
            // Verify the stream actually contains image data (basic check)
            if (memoryStream.Length == 0)
            {
                result.Errors.Add("File appears to be empty");
                return result;
            }

            course.ThumbnailImage = memoryStream.ToArray();
            course.ThumbnailContentType = file.ContentType;
            course.ThumbnailUrl = $"/api/course-images/thumbnail/{courseId}";
            course.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Thumbnail uploaded successfully";
            _logger.LogInformation("Uploaded thumbnail for course {CourseId}, size: {Size} bytes", courseId, file.Size);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading thumbnail for course {CourseId}", courseId);
            result.Errors.Add($"Error uploading thumbnail: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UploadCoverImageAsync(int courseId, IBrowserFile file)
    {
        var result = new CategoryOperationResult();

        try
        {
            // Validate file size
            if (file.Size > MaxImageSizeBytes)
            {
                result.Errors.Add($"File size must be less than {MaxImageSizeBytes / (1024 * 1024)} MB");
                return result;
            }

            // Validate content type
            if (!AllowedImageMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                result.Errors.Add("File must be a valid image (JPEG, PNG, GIF, or WebP)");
                return result;
            }

            // Validate file extension
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                result.Errors.Add("Invalid file extension. Allowed: " + string.Join(", ", AllowedImageExtensions));
                return result;
            }

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var course = await context.Courses.FindAsync(courseId);
            if (course == null)
            {
                result.Errors.Add("Course not found");
                return result;
            }

            // Read file data with size limit enforcement
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: MaxImageSizeBytes).CopyToAsync(memoryStream);
            
            // Verify the stream actually contains image data (basic check)
            if (memoryStream.Length == 0)
            {
                result.Errors.Add("File appears to be empty");
                return result;
            }

            course.CoverImage = memoryStream.ToArray();
            course.CoverImageContentType = file.ContentType;
            course.CoverImageUrl = $"/api/course-images/cover/{courseId}";
            course.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Cover image uploaded successfully";
            _logger.LogInformation("Uploaded cover image for course {CourseId}, size: {Size} bytes", courseId, file.Size);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading cover image for course {CourseId}", courseId);
            result.Errors.Add($"Error uploading cover image: {ex.Message}");
        }

        return result;
    }

    #endregion
}
