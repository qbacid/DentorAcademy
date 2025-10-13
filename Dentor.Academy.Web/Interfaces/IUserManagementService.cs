using Dentor.Academy.Web.DTOs.User;

namespace Dentor.Academy.Web.Interfaces;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserManagementService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserManagementResult> CreateUserAsync(CreateUserDto dto);
    Task<UserManagementResult> UpdateUserAsync(UpdateUserDto dto);
    Task<UserManagementResult> DeleteUserAsync(string userId);
    Task<UserManagementResult> ResetPasswordAsync(string userId);
    string GenerateRandomPassword();
    Task<List<string>> GetAllRolesAsync();
    Task EnsureRolesExistAsync();
}
