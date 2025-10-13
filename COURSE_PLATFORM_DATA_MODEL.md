# Course Platform Data Model Documentation

## Overview
This document describes the comprehensive data model for the DentorAcademy course platform, supporting courses, student enrollments, content management with Azure Blob Storage, and progress tracking.

## Entity Relationship Diagram (ERD) Summary

```
ApplicationUser (Identity)
    ↓ (1:Many)
CourseEnrollment ←→ Course
    ↓ (1:Many)           ↓ (1:Many)
CourseProgress      CourseModule
    ↓                    ↓ (1:Many)
CourseContent ←─────────┘
    ↓ (optional)
Quiz (standalone or course-linked)
```

## Core Entities

### 1. **Course**
Main entity representing a complete course offering.

**Key Features:**
- Course metadata (title, descriptions, category)
- Pricing support (free or paid)
- Publication status and featured flag
- Difficulty levels (Beginner, Intermediate, Advanced)
- Creator tracking (teacher/instructor)
- Thumbnail and cover images

**Table:** `courses`

**Relationships:**
- `1:Many` with CourseModule (a course has many modules)
- `1:Many` with CourseEnrollment (a course has many enrollments)
- `1:Many` with Quiz (a course can have many quizzes)
- `Many:1` with ApplicationUser (created by instructor)

**Key Fields:**
```csharp
- Id (PK)
- Title
- ShortDescription (500 chars)
- FullDescription (unlimited)
- Category
- ThumbnailUrl, CoverImageUrl
- DifficultyLevel (Beginner/Intermediate/Advanced)
- EstimatedDurationHours
- Price (decimal, 0 for free)
- IsPublished, IsFeatured
- CreatedByUserId (FK to ApplicationUser)
- CreatedAt, UpdatedAt, PublishedAt
```

---

### 2. **CourseModule**
Represents sections/modules within a course for organization.

**Key Features:**
- Hierarchical organization with order index
- Module-level descriptions and duration
- Publication control per module

**Table:** `course_modules`

**Relationships:**
- `Many:1` with Course
- `1:Many` with CourseContent
- `1:Many` with CourseModuleProgress

**Key Fields:**
```csharp
- Id (PK)
- CourseId (FK)
- Title
- Description
- OrderIndex (for sequencing)
- EstimatedDurationMinutes
- IsPublished
- CreatedAt, UpdatedAt
```

---

### 3. **CourseContent**
Individual content items within modules (videos, documents, PDFs, etc.).

**Key Features:**
- **Azure Blob Storage Integration** with Shared Key
- Multiple content types (Video, Document, PDF, Image, Audio, Quiz)
- External content support (YouTube, Vimeo)
- Free preview capability
- Download control
- Video playback position tracking

**Table:** `course_contents`

**Relationships:**
- `Many:1` with CourseModule
- `Many:1` with Quiz (optional, for quiz content type)
- `1:Many` with CourseProgress

**Azure Blob Storage Fields:**
```csharp
- BlobContainerName (storage container)
- BlobName (blob identifier)
- BlobUrl (full URL with SAS token)
- FileSizeBytes
- MimeType
```

**Key Fields:**
```csharp
- Id (PK)
- CourseModuleId (FK)
- Title, Description
- ContentType (Video/Document/PDF/Image/Audio/Quiz)
- OrderIndex
- DurationMinutes
- BlobContainerName, BlobName, BlobUrl (Azure Blob)
- FileSizeBytes, MimeType
- QuizId (FK, optional)
- ExternalUrl (for external content)
- IsFreePreview (allow preview without enrollment)
- IsDownloadable
- IsMandatory (if true, must complete before continuing)
- IsPublished
- CreatedAt, UpdatedAt
```

**Quiz Integration:**
When `ContentType = "Quiz"`:
- `QuizId` links to the Quiz entity
- `IsMandatory = true` - Student must pass the quiz to proceed to next content
- `IsMandatory = false` - Student can skip the quiz (optional assessment)
- Progress is marked complete only when quiz is passed (for mandatory quizzes)

---

### 4. **CourseEnrollment**
Tracks student enrollment in courses.

**Key Features:**
- Unique constraint (one enrollment per user per course)
- Enrollment status tracking (Active, Completed, Dropped, Expired)
- Progress percentage calculation
- Certificate generation support
- Payment tracking for paid courses
- Time-limited access support

**Table:** `course_enrollments`

**Relationships:**
- `Many:1` with Course
- `Many:1` with ApplicationUser
- `1:Many` with CourseProgress
- `1:Many` with CourseModuleProgress

**Key Fields:**
```csharp
- Id (PK)
- CourseId (FK)
- UserId (FK)
- EnrolledAt
- Status (Active/Completed/Dropped/Expired)
- ProgressPercentage (calculated)
- LastAccessedAt
- CompletedAt
- CertificateIssued, CertificateIssuedAt, CertificateUrl
- PaymentAmount, PaymentTransactionId, PaymentDate
- ExpiresAt (for time-limited access)
- CreatedAt, UpdatedAt
```

**Indexes:**
- Unique index on (UserId, CourseId)
- Index on EnrolledAt
- Index on Status

---

### 5. **CourseProgress**
Granular tracking of student progress through individual content items.

**Key Features:**
- Tracks completion status per content item
- Video playback position for resumption
- Time spent tracking
- Access count and patterns
- Progress percentage for long content

**Table:** `course_progress`

**Relationships:**
- `Many:1` with CourseEnrollment
- `Many:1` with CourseContent
- `Many:1` with ApplicationUser

**Key Fields:**
```csharp
- Id (PK)
- EnrollmentId (FK)
- CourseContentId (FK)
- UserId (FK)
- IsCompleted
- CompletedAt
- QuizPassed (for quiz content)
- QuizScore (for quiz content)
- QuizAttemptId (links to QuizAttempt)
- ProgressPercentage (for video playback)
- TimeSpentSeconds
- LastPositionSeconds (for resuming videos)
- FirstAccessedAt, LastAccessedAt
- AccessCount
- CreatedAt, UpdatedAt
```

**Quiz Progress Tracking:**
When content is a quiz (`ContentType = "Quiz"`):
- `QuizPassed` - Indicates if student passed the quiz
- `QuizScore` - The score achieved (0-100)
- `QuizAttemptId` - Links to the specific QuizAttempt record
- `IsCompleted` - Set to true only if quiz is passed OR quiz is optional

**Indexes:**
- Unique index on (EnrollmentId, CourseContentId)
- Index on (UserId, CourseContentId)
- Index on CompletedAt

---

### 6. **CourseModuleProgress**
Tracks completion of entire modules.

**Key Features:**
- Module-level completion tracking
- Calculated from CourseProgress items
- Access pattern tracking

**Table:** `course_module_progress`

**Relationships:**
- `Many:1` with CourseEnrollment
- `Many:1` with CourseModule
- `Many:1` with ApplicationUser

**Key Fields:**
```csharp
- Id (PK)
- EnrollmentId (FK)
- CourseModuleId (FK)
- UserId (FK)
- IsCompleted
- CompletedAt
- ProgressPercentage
- FirstAccessedAt, LastAccessedAt
- CreatedAt, UpdatedAt
```

**Indexes:**
- Unique index on (EnrollmentId, CourseModuleId)
- Index on (UserId, CourseModuleId)

---

### 7. **Quiz** (Updated)
Enhanced to support both standalone quizzes and course-integrated quizzes.

**New Features:**
- Optional course relationship
- Can be used as course content or standalone assessment

**Key Fields (New):**
```csharp
- CourseId (FK, nullable)
```

**Relationships (New):**
- `Many:1` with Course (optional)
- `1:Many` with CourseContent (quiz can be referenced by multiple content items)

---

## Azure Blob Storage Integration

### Content Storage Strategy

**Storage Structure:**
```
Container: course-content
├── courses/
│   ├── {course-id}/
│   │   ├── modules/
│   │   │   ├── {module-id}/
│   │   │   │   ├── videos/
│   │   │   │   │   └── {content-id}.mp4
│   │   │   │   ├── documents/
│   │   │   │   │   └── {content-id}.pdf
│   │   │   │   └── images/
│   │   │       └── {content-id}.jpg
```

**Shared Access Signature (SAS) Token Strategy:**
- Generate time-limited SAS tokens for enrolled students
- Read-only access for content viewing
- Download permissions based on `IsDownloadable` flag
- Token expiration aligned with enrollment validity

**Implementation Example:**
```csharp
public string GenerateContentUrl(CourseContent content, TimeSpan validity)
{
    // Generate SAS token for the blob
    var sasBuilder = new BlobSasBuilder
    {
        BlobContainerName = content.BlobContainerName,
        BlobName = content.BlobName,
        Resource = "b",
        StartsOn = DateTimeOffset.UtcNow,
        ExpiresOn = DateTimeOffset.UtcNow.Add(validity)
    };
    
    sasBuilder.SetPermissions(
        content.IsDownloadable 
            ? BlobSasPermissions.Read 
            : BlobSasPermissions.Read
    );
    
    // Generate the full URL with SAS token
    return $"{content.BlobUrl}?{sasBuilder.ToSasQueryParameters(storageSharedKey)}";
}
```

---

## Progress Calculation Logic

### Course Progress Calculation
```csharp
// Calculate overall course progress
var totalContentItems = course.Modules
    .SelectMany(m => m.Contents)
    .Count(c => c.IsPublished);
    
var completedItems = enrollment.ContentProgress
    .Count(p => p.IsCompleted);
    
enrollment.ProgressPercentage = (completedItems * 100.0m) / totalContentItems;
```

### Module Progress Calculation
```csharp
// Calculate module progress
var totalContentInModule = module.Contents.Count(c => c.IsPublished);
var completedInModule = enrollment.ContentProgress
    .Where(p => p.CourseContent.CourseModuleId == module.Id)
    .Count(p => p.IsCompleted);
    
moduleProgress.ProgressPercentage = (completedInModule * 100.0m) / totalContentInModule;
moduleProgress.IsCompleted = (moduleProgress.ProgressPercentage >= 100.0m);
```

---

## Database Indexes Strategy

**Performance Optimizations:**

1. **CourseEnrollment:**
   - Unique index on (UserId, CourseId) - prevents duplicate enrollments
   - Index on EnrolledAt - for reporting
   - Index on Status - for filtering active enrollments

2. **CourseProgress:**
   - Unique index on (EnrollmentId, CourseContentId) - one progress record per content
   - Index on (UserId, CourseContentId) - quick user lookups
   - Index on CompletedAt - completion reports

3. **CourseModule:**
   - Index on (CourseId, OrderIndex) - ordered module retrieval

4. **CourseContent:**
   - Index on (CourseModuleId, OrderIndex) - ordered content retrieval
   - Index on ContentType - filtering by type

5. **Course:**
   - Index on Category - category filtering
   - Index on IsPublished - published course queries
   - Index on CreatedAt - newest courses first

---

## User Roles Integration

**Existing Roles:**
- `Admin` - Full system access
- `Student` - Enroll in courses, take quizzes
- `Instructor` - Create and manage courses
- `Authenticated User` - Basic authenticated access
- `Course Manager` - Manage course catalog

**Role Permissions (Recommended):**

| Action | Student | Instructor | Course Manager | Admin |
|--------|---------|------------|----------------|-------|
| View Published Courses | ✓ | ✓ | ✓ | ✓ |
| Enroll in Courses | ✓ | ✓ | ✓ | ✓ |
| Create Courses | ✗ | ✓ | ✓ | ✓ |
| Publish Courses | ✗ | ✗ | ✓ | ✓ |
| Manage All Courses | ✗ | ✗ | ✓ | ✓ |
| View Student Progress | ✗ | Own Courses | ✓ | ✓ |
| Issue Certificates | ✗ | Own Courses | ✓ | ✓ |
| Upload Content to Azure | ✗ | ✓ | ✓ | ✓ |

---

## Migration Strategy

### Step 1: Create Migration
```bash
dotnet ef migrations add AddCourseModels
```

### Step 2: Update DbContext
Add DbSet properties in QuizDbContext.cs:
```csharp
public DbSet<Course> Courses { get; set; }
public DbSet<CourseModule> CourseModules { get; set; }
public DbSet<CourseContent> CourseContents { get; set; }
public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
public DbSet<CourseProgress> CourseProgress { get; set; }
public DbSet<CourseModuleProgress> CourseModuleProgress { get; set; }
```

### Step 3: Apply Migration
```bash
dotnet ef database update
```

---

## API Endpoints (Recommended)

### Course Management
- `GET /api/courses` - List published courses
- `GET /api/courses/{id}` - Get course details
- `POST /api/courses` - Create course (Teacher+)
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Delete course

### Enrollment
- `POST /api/courses/{id}/enroll` - Enroll in course
- `GET /api/my-courses` - List user's enrolled courses
- `GET /api/enrollments/{id}/progress` - Get enrollment progress

### Content Access
- `GET /api/content/{id}/url` - Get SAS URL for content
- `POST /api/content/{id}/progress` - Update progress
- `GET /api/modules/{id}/contents` - List module contents

### Progress Tracking
- `POST /api/progress/content/{contentId}` - Update content progress
- `POST /api/progress/complete/{contentId}` - Mark content complete
- `GET /api/progress/course/{courseId}` - Get course progress

---

## Next Implementation Steps

1. **Update DbContext** - Add new DbSet properties
2. **Create Migration** - Generate database migration
3. **Azure Blob Service** - Implement content upload/download service
4. **Course Service** - CRUD operations for courses
5. **Enrollment Service** - Handle enrollment logic
6. **Progress Service** - Track and calculate progress
7. **UI Components** - Course catalog, course player, progress dashboard
8. **Video Player** - HTML5 player with progress tracking
9. **Certificate Generator** - PDF certificate generation
10. **Payment Integration** - For paid courses (future)

---

## Security Considerations

1. **Content Access:** Only enrolled students can access course content
2. **SAS Tokens:** Time-limited, read-only access
3. **Progress Integrity:** Server-side validation of progress updates
4. **Enrollment Verification:** Check enrollment status before granting access
5. **Role-Based Access:** Enforce permissions at API level
6. **Data Privacy:** Students can only see their own progress

---

## Performance Optimization

1. **Eager Loading:** Use `.Include()` for related entities
2. **Caching:** Cache published courses and module structures
3. **Pagination:** Paginate course lists and content
4. **CDN:** Use Azure CDN for frequently accessed content
5. **Compression:** Enable video compression and adaptive streaming
6. **Lazy Loading:** Load content URLs only when needed

---

## Future Enhancements

1. **Discussion Forums** - Per course/module discussions
2. **Live Sessions** - Integration with video conferencing
3. **Assignments** - Submittable assignments with grading
4. **Peer Review** - Student-to-student assessment
5. **Gamification** - Badges, points, leaderboards
6. **Mobile App** - Offline content download
7. **Certificates** - Automated certificate generation
8. **Course Reviews** - Student ratings and reviews
9. **Prerequisites** - Course dependency management
10. **Learning Paths** - Curated course sequences
