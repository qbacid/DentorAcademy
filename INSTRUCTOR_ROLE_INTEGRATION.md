# Instructor Role Integration Guide

## Overview
The **Instructor** role has been integrated into the DentorAcademy course platform to manage course creation, content management, and student tracking. This role was previously named "Teacher" but has been renamed for consistency.

## Role Definition

### Instructor Role
- **Name:** `Instructor`
- **Purpose:** Create and manage educational content and courses
- **Scope:** Can create courses, manage own courses, and view progress of students enrolled in their courses

## Changes Made

### 1. Role Renamed from "Teacher" to "Instructor"
**Files Updated:**
- ✅ `Program.cs` - Updated role seeding
- ✅ `UserManagementService.cs` - Updated role creation
- ✅ `COURSE_PLATFORM_DATA_MODEL.md` - Updated documentation
- ✅ `USER_MANAGEMENT_GUIDE.md` - Updated role descriptions

**Previous:**
```csharp
string[] roles = { "Admin", "Student", "Teacher", "Authenticated User", "Course Manager" };
```

**Updated:**
```csharp
string[] roles = { "Admin", "Student", "Instructor", "Authenticated User", "Course Manager" };
```

## Integration with Course Platform

### CourseInstructor Model
The `CourseInstructor` entity enables many-to-many relationships between instructors and courses:

```csharp
public class CourseInstructor
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string UserId { get; set; } // Links to ApplicationUser with Instructor role
    public string Role { get; set; } = "Instructor"; // Lead Instructor, Co-Instructor, Teaching Assistant
    public int OrderIndex { get; set; }
    public DateTime AssignedAt { get; set; }
    public bool IsActive { get; set; }
}
```

### Key Features

#### 1. **Multiple Instructors Per Course**
- A course can have multiple instructors with different roles:
  - **Lead Instructor** - Primary course creator and owner
  - **Co-Instructor** - Secondary instructors assisting with course delivery
  - **Teaching Assistant** - Support staff helping with student questions

#### 2. **Course Ownership**
- Courses track their creator via `CreatedByUserId` field
- The creator is automatically assigned as the Lead Instructor
- Additional instructors can be added via the `CourseInstructor` junction table

#### 3. **Instructor Assignment Workflow**
```csharp
// Example: Assigning an instructor to a course
var courseInstructor = new CourseInstructor
{
    CourseId = course.Id,
    UserId = user.Id, // User must have "Instructor" role
    Role = "Co-Instructor",
    OrderIndex = 1,
    AssignedAt = DateTime.UtcNow,
    IsActive = true
};
```

## Permissions Matrix

| Action | Student | Instructor | Course Manager | Admin |
|--------|---------|------------|----------------|-------|
| View Published Courses | ✓ | ✓ | ✓ | ✓ |
| Enroll in Courses | ✓ | ✓ | ✓ | ✓ |
| Create Courses | ✗ | ✓ | ✓ | ✓ |
| Edit Own Courses | ✗ | ✓ | ✓ | ✓ |
| Edit All Courses | ✗ | ✗ | ✓ | ✓ |
| Publish Courses | ✗ | ✗ | ✓ | ✓ |
| Delete Own Courses | ✗ | ✓ | ✓ | ✓ |
| Delete All Courses | ✗ | ✗ | ✓ | ✓ |
| Upload Content to Azure | ✗ | ✓ | ✓ | ✓ |
| View Own Course Progress | ✗ | ✓ | ✓ | ✓ |
| View All Course Progress | ✗ | ✗ | ✓ | ✓ |
| Issue Certificates | ✗ | Own Courses | ✓ | ✓ |
| Manage Course Modules | ✗ | Own Courses | ✓ | ✓ |
| Add/Remove Instructors | ✗ | ✗ | ✓ | ✓ |

## Database Schema

### Related Tables

#### 1. **courses**
```sql
- id (PK)
- created_by_user_id (FK to AspNetUsers)
- category_id (FK to course_categories)
- ...
```
The `created_by_user_id` links to a user with the "Instructor" role.

#### 2. **course_instructors** (Junction Table)
```sql
- id (PK)
- course_id (FK to courses)
- user_id (FK to AspNetUsers)
- role (varchar: Lead Instructor, Co-Instructor, Teaching Assistant)
- order_index (int)
- assigned_at (timestamp)
- is_active (boolean)
```

**Indexes:**
- Unique index on (course_id, user_id) - Prevents duplicate assignments
- Index on user_id - Quick lookup of instructor's courses

### Navigation Properties

**Course Model:**
```csharp
public class Course
{
    // ...existing properties...
    
    // Creator (Lead Instructor by default)
    public string? CreatedByUserId { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    
    // All instructors (including creator)
    public ICollection<CourseInstructor> Instructors { get; set; }
}
```

**ApplicationUser Model:**
```csharp
public class ApplicationUser : IdentityUser
{
    // Courses created by this user (if Instructor role)
    public ICollection<Course> CreatedCourses { get; set; }
    
    // Courses where this user is an assigned instructor
    public ICollection<CourseInstructor> InstructorAssignments { get; set; }
}
```

## Usage Examples

### Creating a New Instructor User
```csharp
var createUserDto = new CreateUserDto
{
    UserName = "john.instructor",
    Email = "john@dentoracademy.com",
    Roles = new List<string> { "Instructor" },
    GenerateRandomPassword = true
};

var result = await userManagementService.CreateUserAsync(createUserDto);
```

### Assigning an Instructor to a Course
```csharp
// Check if user has Instructor role
var user = await userManager.FindByIdAsync(userId);
var isInstructor = await userManager.IsInRoleAsync(user, "Instructor");

if (isInstructor)
{
    var courseInstructor = new CourseInstructor
    {
        CourseId = courseId,
        UserId = userId,
        Role = "Co-Instructor",
        OrderIndex = 2,
        IsActive = true
    };
    
    await context.CourseInstructors.AddAsync(courseInstructor);
    await context.SaveChangesAsync();
}
```

### Querying Instructor's Courses
```csharp
// Get all courses where user is an instructor
var instructorCourses = await context.CourseInstructors
    .Include(ci => ci.Course)
    .Where(ci => ci.UserId == userId && ci.IsActive)
    .Select(ci => ci.Course)
    .ToListAsync();

// Get courses created by instructor
var createdCourses = await context.Courses
    .Where(c => c.CreatedByUserId == userId)
    .ToListAsync();
```

### Checking Instructor Permissions
```csharp
public async Task<bool> CanEditCourse(string userId, int courseId)
{
    // Check if user is the creator
    var course = await context.Courses.FindAsync(courseId);
    if (course?.CreatedByUserId == userId)
        return true;
    
    // Check if user is an assigned instructor
    var isAssigned = await context.CourseInstructors
        .AnyAsync(ci => ci.CourseId == courseId 
                     && ci.UserId == userId 
                     && ci.IsActive);
    
    return isAssigned;
}
```

## API Authorization Attributes

### Protecting Endpoints
```csharp
// Instructor-only endpoints
[Authorize(Roles = "Instructor,Course Manager,Admin")]
[HttpPost("api/courses")]
public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
{
    // Only instructors can create courses
}

// Check ownership before edit
[Authorize(Roles = "Instructor,Course Manager,Admin")]
[HttpPut("api/courses/{id}")]
public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var canEdit = await CanEditCourse(userId, id);
    
    if (!canEdit && !User.IsInRole("Admin") && !User.IsInRole("Course Manager"))
        return Forbid();
    
    // Proceed with update
}
```

## Migration Path

### For Existing "Teacher" Role Users

If you have existing users with the old "Teacher" role, you'll need to migrate them:

```sql
-- Update existing Teacher role to Instructor
UPDATE "AspNetRoles" 
SET "Name" = 'Instructor', "NormalizedName" = 'INSTRUCTOR'
WHERE "Name" = 'Teacher';

-- Update user roles
UPDATE "AspNetUserRoles" 
SET "RoleId" = (SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Instructor')
WHERE "RoleId" = (SELECT "Id" FROM "AspNetRoles" WHERE "Name" = 'Teacher');
```

**OR** manually via application code:
```csharp
var teacherRole = await roleManager.FindByNameAsync("Teacher");
if (teacherRole != null)
{
    teacherRole.Name = "Instructor";
    teacherRole.NormalizedName = "INSTRUCTOR";
    await roleManager.UpdateAsync(teacherRole);
}
```

## Best Practices

### 1. **Always Verify Role Before Assignment**
```csharp
var user = await userManager.FindByIdAsync(userId);
if (!await userManager.IsInRoleAsync(user, "Instructor"))
{
    throw new InvalidOperationException("User must have Instructor role");
}
```

### 2. **Set Lead Instructor on Course Creation**
```csharp
var course = new Course
{
    Title = "Introduction to Dentistry",
    CreatedByUserId = currentUserId // Creator becomes lead instructor
};
await context.Courses.AddAsync(course);
await context.SaveChangesAsync();

// Automatically create CourseInstructor entry
var leadInstructor = new CourseInstructor
{
    CourseId = course.Id,
    UserId = currentUserId,
    Role = "Lead Instructor",
    OrderIndex = 0
};
await context.CourseInstructors.AddAsync(leadInstructor);
await context.SaveChangesAsync();
```

### 3. **Use OrderIndex for Display**
Display instructors in a specific order using `OrderIndex`:
```csharp
var instructors = await context.CourseInstructors
    .Include(ci => ci.User)
    .Where(ci => ci.CourseId == courseId && ci.IsActive)
    .OrderBy(ci => ci.OrderIndex)
    .ToListAsync();
```

## UI Integration

### Display Instructors on Course Page
```razor
<div class="course-instructors">
    <h4>Instructors</h4>
    @foreach (var instructor in course.Instructors.Where(i => i.IsActive).OrderBy(i => i.OrderIndex))
    {
        <div class="instructor-card">
            <img src="@instructor.User.ProfileImageUrl" alt="@instructor.User.UserName" />
            <div class="instructor-info">
                <h5>@instructor.User.UserName</h5>
                <span class="badge">@instructor.Role</span>
            </div>
        </div>
    }
</div>
```

## Security Considerations

1. **Role Verification:** Always verify the user has the Instructor role before allowing course creation
2. **Ownership Checks:** Instructors should only manage their own courses unless they're Course Managers or Admins
3. **Audit Trail:** Track when instructors are added/removed from courses via `AssignedAt` field
4. **Active Status:** Use `IsActive` flag to temporarily disable instructor access without deleting the relationship

## Next Steps

1. ✅ **Role Definition** - Instructor role created and seeded
2. ✅ **Database Schema** - CourseInstructor table defined
3. ⏳ **Migration** - Add CourseInstructor to database (pending)
4. ⏳ **Services** - Create InstructorService for managing assignments
5. ⏳ **API Endpoints** - Implement instructor management APIs
6. ⏳ **UI Components** - Build instructor assignment interface
7. ⏳ **Authorization** - Implement permission checks throughout the application

## Summary

The Instructor role is now fully integrated into the DentorAcademy platform with:
- ✅ Renamed from "Teacher" to "Instructor" across all code and documentation
- ✅ Role automatically seeded on application startup
- ✅ CourseInstructor model for managing multiple instructors per course
- ✅ Clear permission boundaries defined
- ✅ Database schema designed for scalability
- ✅ Integration with existing User Management system

Instructors can now be created via the User Management interface and will be able to create and manage courses once the course management features are implemented.

