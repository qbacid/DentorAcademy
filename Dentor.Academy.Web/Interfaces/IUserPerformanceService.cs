using Dentor.Academy.Web.DTOs.User;

namespace Dentor.Academy.Web.Interfaces;

/// <summary>
/// Interface for user performance tracking and analytics
/// </summary>
public interface IUserPerformanceService
{
    Task<UserPerformanceDto?> GetUserPerformanceAsync(string userId);
    Task<List<UserPerformanceDto>> GetTopPerformersAsync(int count = 10);
    Task<Dictionary<string, int>> GetCategoryPerformanceAsync(string userId);
    Task<List<CategoryPerformanceDto>> CalculateCategoryPerformancesAsync(string userId);
    Task<CategoryPerformanceDto?> GetCategoryPerformanceDetailAsync(string userId, string category);
}
