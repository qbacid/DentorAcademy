using Dentor.Solutions.Academy.Data;
using Dentor.Solutions.Academy.DTOs.Course;
using Dentor.Solutions.Academy.Interfaces;
using Dentor.Solutions.Academy.Models;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Services;

/// <summary>
/// Service for managing course categories
/// </summary>
public class CourseCategoryService : ICourseCategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<CourseCategoryService> _logger;

    public CourseCategoryService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<CourseCategoryService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<List<CourseCategoryDto>> GetAllCategoriesAsync(bool includeInactive = false)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.CourseCategories
            .Include(c => c.Courses.Where(course => course.IsPublished)) // Only count published courses
                .ThenInclude(course => course.Enrollments) // Include enrollments for counting students
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var categories = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CourseCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IconClass = c.IconClass,
                Color = c.Color,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                CourseCount = c.Courses.Count(course => course.IsPublished), // Only count published courses
                EnrollmentCount = c.Courses.Sum(course => course.Enrollments.Count), // Total student enrollments across all courses in category
                QuizCount = c.Courses.SelectMany(course => course.Modules).SelectMany(m => m.Contents).Count(content => content.QuizId != null), // Total quizzes across all courses
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return categories;
    }

    public async Task<CourseCategoryDto?> GetCategoryByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var category = await context.CourseCategories
            .Where(c => c.Id == id)
            .Select(c => new CourseCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IconClass = c.IconClass,
                Color = c.Color,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                CourseCount = c.Courses.Count,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        return category;
    }

    public async Task<CategoryOperationResult> CreateCategoryAsync(CreateCourseCategoryDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            // Check for duplicate name
            var exists = await context.CourseCategories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
            {
                result.Errors.Add("A category with this name already exists");
                return result;
            }

            var category = new CourseCategory
            {
                Name = dto.Name,
                Description = dto.Description,
                IconClass = dto.IconClass ?? "bi bi-folder",
                Color = dto.Color ?? "#0066CC",
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            context.CourseCategories.Add(category);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Category created successfully";
            result.Category = await GetCategoryByIdAsync(category.Id);

            _logger.LogInformation("Created category: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {CategoryName}", dto.Name);
            result.Errors.Add($"Error creating category: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> UpdateCategoryAsync(UpdateCourseCategoryDto dto)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var category = await context.CourseCategories.FindAsync(dto.Id);

            if (category == null)
            {
                result.Errors.Add("Category not found");
                return result;
            }

            // Check for duplicate name (excluding current category)
            var exists = await context.CourseCategories
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && c.Id != dto.Id);

            if (exists)
            {
                result.Errors.Add("A category with this name already exists");
                return result;
            }

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.IconClass = dto.IconClass;
            category.Color = dto.Color;
            category.DisplayOrder = dto.DisplayOrder;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Category updated successfully";
            result.Category = await GetCategoryByIdAsync(category.Id);

            _logger.LogInformation("Updated category: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category: {CategoryId}", dto.Id);
            result.Errors.Add($"Error updating category: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> DeleteCategoryAsync(int id)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var category = await context.CourseCategories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                result.Errors.Add("Category not found");
                return result;
            }

            if (category.Courses.Any())
            {
                result.Errors.Add($"Cannot delete category with {category.Courses.Count} course(s). Please reassign or delete courses first.");
                return result;
            }

            context.CourseCategories.Remove(category);
            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Category deleted successfully";

            _logger.LogInformation("Deleted category: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
            result.Errors.Add($"Error deleting category: {ex.Message}");
        }

        return result;
    }

    public async Task<CategoryOperationResult> ReorderCategoriesAsync(List<int> categoryIds)
    {
        var result = new CategoryOperationResult();

        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var categories = await context.CourseCategories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            for (int i = 0; i < categoryIds.Count; i++)
            {
                var category = categories.FirstOrDefault(c => c.Id == categoryIds[i]);
                if (category != null)
                {
                    category.DisplayOrder = i;
                    category.UpdatedAt = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Categories reordered successfully";

            _logger.LogInformation("Reordered {Count} categories", categoryIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering categories");
            result.Errors.Add($"Error reordering categories: {ex.Message}");
        }

        return result;
    }

    public async Task<int> GetCourseCountByCategoryAsync(int categoryId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Courses
            .Where(c => c.CategoryId == categoryId)
            .CountAsync();
    }

    public async Task<List<Quiz>> GetQuizzesByCategoryAsync(int categoryId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        // Get all quizzes that are linked to courses in the specified category
        var quizzes = await context.Courses
            .Where(c => c.CategoryId == categoryId)
            .SelectMany(c => c.Modules)
            .SelectMany(m => m.Contents)
            .Where(content => content.QuizId != null)
            .Select(content => content.Quiz!)
            .Distinct()
            .OrderBy(q => q.Title)
            .ToListAsync();

        return quizzes;
    }

    public async Task<int> GetQuizCountByCategoryAsync(int categoryId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        // Count unique quizzes linked to courses in the specified category
        var count = await context.Courses
            .Where(c => c.CategoryId == categoryId)
            .SelectMany(c => c.Modules)
            .SelectMany(m => m.Contents)
            .Where(content => content.QuizId != null)
            .Select(content => content.QuizId)
            .Distinct()
            .CountAsync();

        return count;
    }
}
