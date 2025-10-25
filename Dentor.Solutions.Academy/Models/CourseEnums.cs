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
/// Type of course content
/// </summary>
public enum InternalContentType
{
    Video = 0,
    Document = 1,
    PDF = 2,
    Image = 3,
    Audio = 4,
    Quiz = 5,
    ExternalLink = 6
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

