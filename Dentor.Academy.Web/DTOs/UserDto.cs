namespace Dentor.Academy.Web.DTOs;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool MustChangePassword { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}

