using Microsoft.AspNetCore.Identity;

namespace Dentor.Academy.Web.Models;

public class ApplicationUser : IdentityUser
{
    public bool MustChangePassword { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
}
