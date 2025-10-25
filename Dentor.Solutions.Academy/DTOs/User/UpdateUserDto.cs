namespace Dentor.Academy.WebApp.DTOs.User;

/// <summary>
/// DTO for updating existing users
/// </summary>
public class UpdateUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
