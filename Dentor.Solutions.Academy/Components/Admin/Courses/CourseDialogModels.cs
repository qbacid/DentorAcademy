namespace Dentor.Solutions.Academy.Components.Admin.Courses;

using Dentor.Solutions.Academy.DTOs.Course;
using Microsoft.AspNetCore.Components.Forms;

// Helper classes for Course dialogs

public class CourseDialogData 
{
    public CourseDto Course { get; set; } = new();
    public List<CourseCategoryDto> Categories { get; set; } = new();
    
    public string ThumbnailPreview { get; set; } = string.Empty;
    public string CoverImagePreview { get; set; } = string.Empty;
    public IBrowserFile? ThumbnailFile { get; set; }
    public IBrowserFile? CoverImageFile { get; set; }
}

public class CourseSimpleFieldData
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldValue { get; set; } = string.Empty;
}

public class CourseAssignmentData
{
    public int CourseId { get; set; }
    public List<UserSelectionDto> Students { get; set; } = new();
    public List<UserSelectionDto> Instructors { get; set; } = new();
    public string StudentSearchTerm { get; set; } = string.Empty;
    public string InstructorSearchTerm { get; set; } = string.Empty;

    public List<UserSelectionDto> FilteredStudents =>
        Students.Where(s => string.IsNullOrEmpty(StudentSearchTerm) ||
                           s.UserName.Contains(StudentSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.Email.Contains(StudentSearchTerm, StringComparison.OrdinalIgnoreCase))
               .ToList();

    public List<UserSelectionDto> FilteredInstructors =>
        Instructors.Where(i => string.IsNullOrEmpty(InstructorSearchTerm) ||
                              i.UserName.Contains(InstructorSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                              i.Email.Contains(InstructorSearchTerm, StringComparison.OrdinalIgnoreCase))
                  .ToList();
}

public class UserSelectionDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

