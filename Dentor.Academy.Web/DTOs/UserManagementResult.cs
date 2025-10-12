namespace Dentor.Academy.Web.DTOs;

public class UserManagementResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? GeneratedPassword { get; set; }
    public UserDto? User { get; set; }
    public List<string> Errors { get; set; } = new();
}
