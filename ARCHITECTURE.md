# DentorAcademy Architecture Documentation

## Overview
DentorAcademy follows a **Clean Architecture** pattern with **Blazor Server** as the presentation layer, implementing Domain-Driven Design principles for a scalable educational platform.

## Architectural Pattern

### Core Pattern: Layered Architecture with Service Layer

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│              (Blazor Components & Pages)                     │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                    Service Layer                             │
│         (Business Logic & Application Services)              │
│    ┌─────────────┐  ┌──────────────┐  ┌─────────────┐      │
│    │ Quiz        │  │ User Mgmt    │  │ Course      │      │
│    │ Services    │  │ Services     │  │ Services    │      │
│    └─────────────┘  └──────────────┘  └─────────────┘      │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                    Data Layer                                │
│              (EF Core & DbContext)                           │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                   Database (PostgreSQL)                      │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

```
Dentor.Academy.Web/
├── Components/               # Blazor UI Components
│   ├── Layout/              # Layout components (NavMenu, MainLayout)
│   ├── Pages/               # Routable pages
│   └── Shared/              # Shared/reusable components
│
├── Services/                # Business Logic Layer
│   ├── Interfaces/          # Service interfaces (DI abstraction)
│   │   ├── IQuizScoringService.cs
│   │   ├── IQuizImportService.cs
│   │   ├── IQuizTakingService.cs
│   │   ├── IUserManagementService.cs
│   │   └── IUserPerformanceService.cs
│   │
│   ├── QuizScoringService.cs
│   ├── QuizImportService.cs
│   ├── QuizTakingService.cs
│   ├── UserManagementService.cs
│   └── UserPerformanceService.cs
│
├── DTOs/                    # Data Transfer Objects (organized by domain)
│   ├── Quiz/               # Quiz-related DTOs
│   │   ├── QuizCardDto.cs
│   │   ├── QuizDisplayDto.cs
│   │   ├── QuizImportDto.cs
│   │   └── ImportResult.cs
│   │
│   ├── User/               # User-related DTOs
│   │   ├── UserDto.cs
│   │   ├── CreateUserDto.cs
│   │   ├── UpdateUserDto.cs
│   │   ├── UserManagementResult.cs
│   │   └── UserPerformanceDto.cs
│   │
│   └── Course/             # Course-related DTOs (future)
│
├── Models/                 # Domain Entities
│   ├── Quiz/              # Quiz domain
│   │   ├── Quiz.cs
│   │   ├── Question.cs
│   │   ├── AnswerOption.cs
│   │   ├── QuizAttempt.cs
│   │   ├── UserResponse.cs
│   │   └── UserResponseAnswer.cs
│   │
│   ├── Course/            # Course domain
│   │   ├── Course.cs
│   │   ├── CourseModule.cs
│   │   ├── CourseContent.cs
│   │   ├── CourseEnrollment.cs
│   │   ├── CourseProgress.cs
│   │   ├── CourseModuleProgress.cs
│   │   ├── CourseCategory.cs
│   │   ├── CourseReview.cs
│   │   ├── CourseInstructor.cs
│   │   └── CourseCertificate.cs
│   │
│   └── ApplicationUser.cs  # Identity user
│
├── Data/                   # Data Access Layer
│   ├── QuizDbContext.cs   # EF Core DbContext
│   └── SeedData.cs        # Database seeding
│
├── Migrations/             # EF Core migrations
├── Properties/             # App configuration
├── wwwroot/               # Static files
└── Program.cs             # App startup & DI configuration
```

## Design Principles Applied

### 1. **Dependency Inversion Principle (SOLID)**
✅ **Implemented**: All services use interfaces
```csharp
// Service registration in Program.cs
builder.Services.AddScoped<IQuizScoringService, QuizScoringService>();
builder.Services.AddScoped<IQuizImportService, QuizImportService>();
builder.Services.AddScoped<IQuizTakingService, QuizTakingService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IUserPerformanceService, UserPerformanceService>();
```

**Benefits:**
- Easy to mock services for unit testing
- Loose coupling between layers
- Can swap implementations without breaking dependents
- Better testability

### 2. **Separation of Concerns**
✅ **Implemented**: Clear separation between layers
- **Presentation**: Blazor components handle UI only
- **Business Logic**: Services contain all business rules
- **Data Access**: DbContext handles database operations
- **DTOs**: Separate objects for data transfer (no entity exposure)

### 3. **Single Responsibility Principle**
✅ **Implemented**: Each service has one responsibility
- `QuizScoringService`: Quiz evaluation and scoring only
- `QuizImportService`: Quiz import operations only
- `QuizTakingService`: Quiz-taking workflow only
- `UserManagementService`: User CRUD operations only
- `UserPerformanceService`: Analytics and reporting only

### 4. **Domain-Driven Design (DDD)**
✅ **Implemented**: Organized by domain aggregates
- **Quiz Aggregate**: Quiz → Question → AnswerOption
- **Course Aggregate**: Course → CourseModule → CourseContent
- **User Aggregate**: ApplicationUser with roles
- **Enrollment Aggregate**: CourseEnrollment → Progress tracking

## Data Transfer Objects (DTOs)

### Purpose
DTOs prevent direct exposure of domain entities to the presentation layer, providing:
- **Security**: Hide sensitive data
- **Flexibility**: API contracts independent of database schema
- **Versioning**: Can maintain multiple DTO versions
- **Performance**: Optimize data transfer

### Organization by Domain
```
DTOs/
├── Quiz/              # Quiz subdomain
├── User/              # User subdomain  
└── Course/            # Course subdomain (future)
```

### Naming Convention
- `{Entity}Dto`: Display data (e.g., `UserDto`)
- `Create{Entity}Dto`: Create operations (e.g., `CreateUserDto`)
- `Update{Entity}Dto`: Update operations (e.g., `UpdateUserDto`)
- `{Entity}Result`: Operation results (e.g., `ImportResult`)

## Service Layer Pattern

### Interface-Based Services
All services implement interfaces for dependency injection:

```csharp
public interface IQuizScoringService
{
    Task<bool> EvaluateResponse(int questionId, List<int> selectedAnswerOptionIds);
    Task<decimal> CalculateQuizScore(int quizAttemptId);
    // ... other methods
}

public class QuizScoringService : IQuizScoringService
{
    private readonly QuizDbContext _context;
    
    public QuizScoringService(QuizDbContext context)
    {
        _context = context;
    }
    
    // Implementation...
}
```

### Service Responsibilities

#### Quiz Services
- **QuizScoringService**: Scoring logic, evaluation algorithms
- **QuizImportService**: JSON/CSV import, validation
- **QuizTakingService**: Quiz workflow, attempt management

#### User Services
- **UserManagementService**: User CRUD, role management, password operations
- **UserPerformanceService**: Analytics, performance tracking, reporting

#### Course Services (Planned)
- **CourseManagementService**: Course CRUD operations
- **EnrollmentService**: Enrollment workflow
- **ProgressTrackingService**: Progress calculation
- **CertificateService**: Certificate generation

## Database Design

### Naming Conventions (PostgreSQL)
- **Tables**: Snake_case, plural (e.g., `quiz_attempts`)
- **Columns**: Snake_case (e.g., `created_at`)
- **Foreign Keys**: `{table}_id` (e.g., `quiz_id`)

### Key Features
- **Snake case naming**: Follows PostgreSQL conventions
- **Indexes**: Strategic indexes on frequently queried columns
- **Soft deletes**: Not implemented (using hard deletes with cascade rules)
- **Audit fields**: `created_at`, `updated_at` on all entities
- **Optimistic concurrency**: Not currently implemented

### Entity Relationships
```
Quiz 1───* Question 1───* AnswerOption
  │                │
  │                └───* UserResponse 1───* UserResponseAnswer
  │
  └───* QuizAttempt

Course 1───* CourseModule 1───* CourseContent
   │            │                    │
   │            └───* CourseModuleProgress
   │
   └───* CourseEnrollment 1───* CourseProgress
```

## Authentication & Authorization

### Identity Framework
- **ASP.NET Core Identity**: User authentication
- **Role-Based Access Control (RBAC)**: Authorization

### Roles
1. **Admin**: Full system access
2. **Student**: Course enrollment, quiz taking
3. **Instructor**: Course creation, content management
4. **Course Manager**: Course catalog management
5. **Authenticated User**: Basic access

### Authorization Patterns
```csharp
// Policy-based authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// In components
[Authorize(Roles = "Admin,Instructor")]
public class CourseManagement : ComponentBase { }
```

## Dependency Injection

### Lifetime Scopes
- **Scoped**: Services tied to HTTP request
  - All application services (Quiz, User, Course services)
  - DbContext
  
- **Singleton**: Application-wide instances
  - Configuration services
  - Caching services (future)
  
- **Transient**: New instance per injection
  - Currently not used

### Registration Pattern
```csharp
// Interface-based registration
builder.Services.AddScoped<IQuizScoringService, QuizScoringService>();

// Allows constructor injection
public class QuizTakingService
{
    private readonly IQuizScoringService _scoringService;
    
    public QuizTakingService(IQuizScoringService scoringService)
    {
        _scoringService = scoringService;
    }
}
```

## Error Handling Strategy

### Current Approach
- **Exceptions**: Used for exceptional situations
- **Result Objects**: DTOs include success/error indicators
- **Logging**: ILogger injected into services

### Result Pattern
```csharp
public class ImportResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
```

## Testing Strategy (Planned)

### Unit Testing
- **xUnit**: Test framework
- **Moq**: Mocking framework
- **Service tests**: Mock DbContext and dependencies

### Integration Testing
- **In-memory database**: For integration tests
- **TestContainers**: PostgreSQL test containers

### Test Organization
```
Tests/
├── UnitTests/
│   ├── Services/
│   │   ├── QuizScoringServiceTests.cs
│   │   └── UserManagementServiceTests.cs
│   └── Models/
│
└── IntegrationTests/
    ├── QuizWorkflowTests.cs
    └── EnrollmentWorkflowTests.cs
```

## Performance Considerations

### Database Optimization
- **Eager Loading**: `.Include()` for related entities
- **Async/Await**: All database operations are async
- **Connection Pooling**: Configured in PostgreSQL connection
- **Retry Logic**: Built-in retry strategy for transient failures

### Caching Strategy (Future)
- **Distributed Cache**: Redis for session data
- **Memory Cache**: Frequently accessed data (categories, roles)
- **Response Caching**: Static content caching

## Migration Strategy

### Current State
✅ Database schema defined
✅ Migrations created for Quiz and Course platforms
✅ Seed data for roles and admin user

### Pending Migrations
⏳ CourseCategory, CourseReview, CourseInstructor, CourseCertificate
⏳ Full course platform integration

### Migration Commands
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## Security Best Practices

### Implemented
✅ **Password Hashing**: ASP.NET Core Identity
✅ **Role-Based Authorization**: Policy-based access control
✅ **HTTPS Enforcement**: Production configuration
✅ **Anti-Forgery Tokens**: Blazor Server built-in
✅ **SQL Injection Prevention**: EF Core parameterized queries

### Planned
⏳ **Rate Limiting**: API endpoint throttling
⏳ **CORS Policy**: For future API endpoints
⏳ **Data Encryption**: Sensitive field encryption
⏳ **Audit Logging**: Track user actions

## Scalability Considerations

### Horizontal Scaling (Future)
- **Session State**: Move to Redis/SQL Server
- **Load Balancing**: Multiple Blazor Server instances
- **Database Read Replicas**: Separate read/write operations

### Vertical Scaling
- **Database Indexing**: Strategic indexes on hot paths
- **Query Optimization**: Efficient LINQ queries
- **Connection Pooling**: Optimized connection settings

## Monitoring & Logging

### Logging Levels
- **Trace**: Detailed diagnostic information
- **Debug**: Development debugging
- **Information**: General informational messages
- **Warning**: Warning messages (recoverable issues)
- **Error**: Error messages (handled exceptions)
- **Critical**: Critical failures (app crash)

### Current Implementation
```csharp
private readonly ILogger<QuizImportService> _logger;

_logger.LogError(ex, "Error importing quiz from JSON");
_logger.LogInformation("Successfully imported quiz '{QuizTitle}'", quiz.Title);
```

## Code Style & Conventions

### Naming Conventions
- **Classes**: PascalCase (e.g., `QuizScoringService`)
- **Interfaces**: IPascalCase (e.g., `IQuizScoringService`)
- **Methods**: PascalCase (e.g., `CalculateScore`)
- **Parameters**: camelCase (e.g., `quizId`)
- **Private Fields**: _camelCase (e.g., `_context`)

### File Organization
- One class per file
- File name matches class name
- Organized by feature/domain

## Future Enhancements

### Phase 1 (Current)
✅ Quiz system with scoring
✅ User management with roles
✅ Course data model

### Phase 2 (In Progress)
⏳ Course management UI
⏳ Enrollment workflow
⏳ Progress tracking
⏳ Azure Blob Storage integration

### Phase 3 (Planned)
🔲 Certificate generation
🔲 Payment integration
🔲 Advanced analytics
🔲 Mobile responsive design

### Phase 4 (Future)
🔲 Real-time collaboration
🔲 Video streaming
🔲 AI-powered recommendations
🔲 Mobile app (MAUI)

## Architectural Decisions Log

### ADR-001: Blazor Server vs Blazor WebAssembly
**Decision**: Blazor Server
**Rationale**: 
- Better for educational platform with secure data
- Lower client-side resource requirements
- Easier to implement real-time features
- Better SEO (server-side rendering)

### ADR-002: PostgreSQL vs SQL Server
**Decision**: PostgreSQL
**Rationale**:
- Open-source and cost-effective
- Excellent performance for read-heavy workloads
- Strong JSON support for future features
- Better for cloud deployments

### ADR-003: Service Layer Pattern
**Decision**: Interface-based service layer
**Rationale**:
- Testability through dependency injection
- Clear separation of concerns
- Easier to maintain and extend
- Industry standard pattern

### ADR-004: DTO Organization
**Decision**: Organize DTOs by domain in subfolders
**Rationale**:
- Better organization as project grows
- Clear namespace separation
- Easier to find related DTOs
- Supports microservices migration

## Conclusion

DentorAcademy follows industry best practices with a clean, maintainable architecture that supports:
- **Scalability**: Can grow from single server to distributed system
- **Testability**: Interface-based design enables comprehensive testing
- **Maintainability**: Clear separation of concerns and SOLID principles
- **Extensibility**: Easy to add new features without breaking existing code

The architecture is designed to evolve with the platform's needs while maintaining code quality and performance.

