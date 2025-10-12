namespace Dentor.Academy.Web.DTOs;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool GenerateRandomPassword { get; set; } = true;
    public string? Password { get; set; }
    public List<string> Roles { get; set; } = new();
}

