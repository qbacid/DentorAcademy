using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dentor.Solutions.Academy.Models;

/// <summary>
/// Difficulty level for courses
/// </summary>
public enum DifficultyLevel
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2
}

/// <summary>
/// Mime type of course content
/// </summary>
public enum ContentMimeType
{
    [Display(Name = "Video")] Video = 0,
    [Display(Name = "Document")] Document = 1,
    [Display(Name = "PDF")] PDF = 2,
    [Display(Name = "Image")] Image = 3,
    [Display(Name = "Audio")] Audio = 4,
    [Display(Name = "Quiz")] Quiz = 5,
    [Display(Name = "External Link")] ExternalLink = 6
}

/// <summary>
/// use to filter and control the logic the ContentMimeType enum
/// </summary>
public enum CourseContentType
{
    [Display(Name = "Lecture")] Lecture = 1, //a lecture is a video or document or just a single type of mime type of content
    [Display(Name = "Assigment")] Assigment = 2, //is a type of content that requires submission
    [Display(Name = "Quiz")] Quiz = 3, //is a type of content that requires submission and can have a passing score and time limi
    [Display(Name = "Credential")] Credential = 4, //is just a credential or badge that can be earned
    [Display(Name = "Webinar")] Webinar = 5 //is a live or recorded webinar or long-content video
}

/// <summary>
/// Enrollment status for students
/// </summary>
public enum EnrollmentStatus
{
    Active = 0,
    Completed = 1,
    Dropped = 2,
    Expired = 3,
    Pending = 4
}

public enum SaveEntityResult
{
    Success = 0,
    Failed = 1,
    Error = 2
}

public enum MessageSessions
{
    MESSAGES_TOP,
    MESSAGES_BOTTOM
}

