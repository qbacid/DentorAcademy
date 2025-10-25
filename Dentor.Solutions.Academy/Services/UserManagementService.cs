using System.Security.Cryptography;
using System.Text;
using Dentor.Academy.WebApp.DTOs.User;
using Dentor.Solutions.Academy.Data;
using Dentor.Solutions.Academy.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dentor.Solutions.Academy.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                MustChangePassword = user.MustChangePassword,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLoginDate
            });
        }

        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            MustChangePassword = user.MustChangePassword,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLoginDate
        };
    }

    public async Task<UserManagementResult> CreateUserAsync(CreateUserDto dto)
    {
        var result = new UserManagementResult();

        // Check if user exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            result.Success = false;
            result.Message = "User with this email already exists.";
            result.Errors.Add("Email already in use.");
            return result;
        }

        // Generate a password if needed
        string password;
        if (dto.GenerateRandomPassword)
        {
            password = GenerateRandomPassword();
            result.GeneratedPassword = password;
        }
        else
        {
            password = dto.Password ?? GenerateRandomPassword();
            if (string.IsNullOrEmpty(dto.Password))
            {
                result.GeneratedPassword = password;
            }
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = true,
            MustChangePassword = dto.GenerateRandomPassword,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            result.Success = false;
            result.Message = "Failed to create user.";
            result.Errors = createResult.Errors.Select(e => e.Description).ToList();
            return result;
        }

        // Assign roles
        foreach (var role in dto.Roles)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        result.Success = true;
        result.Message = "User created successfully.";
        result.User = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            EmailConfirmed = user.EmailConfirmed,
            MustChangePassword = user.MustChangePassword,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLoginDate
        };

        return result;
    }

    public async Task<UserManagementResult> UpdateUserAsync(UpdateUserDto dto)
    {
        var result = new UserManagementResult();
        var user = await _userManager.FindByIdAsync(dto.Id);

        if (user == null)
        {
            result.Success = false;
            result.Message = "User not found.";
            return result;
        }

        // Update basic info
        user.Email = dto.Email;
        user.UserName = dto.UserName;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            result.Success = false;
            result.Message = "Failed to update user.";
            result.Errors = updateResult.Errors.Select(e => e.Description).ToList();
            return result;
        }

        // Update roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(dto.Roles).ToList();
        var rolesToAdd = dto.Roles.Except(currentRoles).ToList();

        if (rolesToRemove.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        foreach (var role in rolesToAdd)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        var updatedRoles = await _userManager.GetRolesAsync(user);
        result.Success = true;
        result.Message = "User updated successfully.";
        result.User = new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            MustChangePassword = user.MustChangePassword,
            Roles = updatedRoles.ToList(),
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLoginDate
        };

        return result;
    }

    public async Task<UserManagementResult> DeleteUserAsync(string userId)
    {
        var result = new UserManagementResult();
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            result.Success = false;
            result.Message = "User not found.";
            return result;
        }

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            result.Success = false;
            result.Message = "Failed to delete user.";
            result.Errors = deleteResult.Errors.Select(e => e.Description).ToList();
            return result;
        }

        result.Success = true;
        result.Message = "User deleted successfully.";
        return result;
    }

    public async Task<UserManagementResult> ResetPasswordAsync(string userId)
    {
        var result = new UserManagementResult();
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            result.Success = false;
            result.Message = "User not found.";
            return result;
        }

        var newPassword = GenerateRandomPassword();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!resetResult.Succeeded)
        {
            result.Success = false;
            result.Message = "Failed to reset password.";
            result.Errors = resetResult.Errors.Select(e => e.Description).ToList();
            return result;
        }

        user.MustChangePassword = true;
        user.LastPasswordChangeDate = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        result.Success = true;
        result.Message = "Password reset successfully.";
        result.GeneratedPassword = newPassword;
        return result;
    }

    public string GenerateRandomPassword()
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        
        var password = new StringBuilder();
        
        // Ensure at least one of each required character type
        password.Append(lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)]);
        password.Append(uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)]);
        password.Append(digits[RandomNumberGenerator.GetInt32(digits.Length)]);
        password.Append(special[RandomNumberGenerator.GetInt32(special.Length)]);
        
        // Fill the rest with random characters
        const string allChars = lowercase + uppercase + digits + special;
        for (int i = 4; i < 12; i++)
        {
            password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
        }
        
        // Shuffle the password
        return new string(password.ToString().OrderBy(x => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
    }

    public async Task EnsureRolesExistAsync()
    {
        string[] roles = { "Admin", "Student", "Instructor", "Authenticated User", "Course Manager" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
