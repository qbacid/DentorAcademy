using Dentor.Solutions.Academy.Models;
using Microsoft.AspNetCore.Identity;

namespace Dentor.Solutions.Academy.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public bool MustChangePassword { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
    
    // Navigation properties
    public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
    public ICollection<CourseCertificate> CourseCertificates { get; set; } = new List<CourseCertificate>();
}