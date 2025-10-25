namespace Dentor.Academy.WebApp.DTOs.User;

/// <summary>
/// Result of a user management operation
/// </summary>
public class UserManagementResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? GeneratedPassword { get; set; }
    public UserDto? User { get; set; }
    public List<string> Errors { get; set; } = new();
}
