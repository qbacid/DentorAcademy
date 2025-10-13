# Complete Database Relationships and Data Model

## Overview
This document provides a comprehensive overview of the DentorAcademy database architecture, including all entities, relationships, and design decisions.

**Last Updated:** October 12, 2025

---

## Entity Summary

### Total Entities: 16

#### Quiz System (6 entities)
- Quiz
- Question
- AnswerOption
- QuizAttempt
- UserResponse
- UserResponseAnswer

#### Course Platform (10 entities)
- Course
- CourseCategory
- CourseModule
- CourseContent
- CourseEnrollment
- CourseProgress
- CourseModuleProgress
- CourseReview
- CourseInstructor
- CourseCertificate

#### Identity System
- ApplicationUser (extends IdentityUser)

---

## Detailed Entity Relationships

### 1. QUIZ SYSTEM

#### Quiz (1) → (N) Questions
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade (deleting quiz deletes all questions)
- **Foreign Key:** Question.QuizId
- **Purpose:** A quiz contains multiple questions

#### Quiz (1) → (N) QuizAttempts
- **Relationship:** One-to-Many
- **Delete Behavior:** Restrict (preserve attempt history)
- **Foreign Key:** QuizAttempt.QuizId
- **Purpose:** Tracks all attempts made on this quiz

#### Question (1) → (N) AnswerOptions
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** AnswerOption.QuestionId
- **Purpose:** Each question has multiple answer choices

#### Question (1) → (N) UserResponses
- **Relationship:** One-to-Many
- **Delete Behavior:** Restrict (preserve response history)
- **Foreign Key:** UserResponse.QuestionId
- **Purpose:** Tracks all user responses to this question

#### QuizAttempt (1) → (N) UserResponses
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** UserResponse.QuizAttemptId
- **Unique Index:** (QuizAttemptId, QuestionId) - one response per question per attempt
- **Purpose:** All responses for a specific attempt

#### UserResponse (N) ↔ (N) AnswerOptions
- **Relationship:** Many-to-Many (through UserResponseAnswer junction table)
- **Delete Behavior:** Cascade on UserResponse side, Restrict on AnswerOption side
- **Purpose:** Supports multi-select questions where users can choose multiple answers

#### Quiz (N) → (1) Course (Optional)
- **Relationship:** Many-to-One (optional)
- **Foreign Key:** Quiz.CourseId (nullable)
- **Purpose:** Quizzes can be standalone or associated with a course

---

### 2. COURSE PLATFORM SYSTEM

#### CourseCategory (1) → (N) Courses
- **Relationship:** One-to-Many
- **Delete Behavior:** Restrict (preserve course history)
- **Foreign Key:** Course.CategoryId
- **Unique Index:** Category.Name (no duplicate category names)
- **Purpose:** Organize courses into categories

#### Course (1) → (N) CourseModules
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseModule.CourseId
- **Purpose:** A course is divided into multiple modules

#### CourseModule (1) → (N) CourseContents
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseContent.CourseModuleId
- **Purpose:** Each module contains multiple content items (videos, documents, quizzes)

#### Course (1) → (N) CourseEnrollments
- **Relationship:** One-to-Many
- **Delete Behavior:** Restrict (preserve enrollment history)
- **Foreign Key:** CourseEnrollment.CourseId
- **Purpose:** Tracks all student enrollments in a course

#### CourseEnrollment (1) → (N) CourseProgress
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseProgress.EnrollmentId
- **Purpose:** Tracks progress for each content item in an enrollment

#### CourseEnrollment (1) → (N) CourseModuleProgress
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseModuleProgress.EnrollmentId
- **Purpose:** Tracks progress for each module in an enrollment

#### CourseContent (1) → (N) CourseProgress
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseProgress.CourseContentId
- **Purpose:** Links progress records to specific content items

#### CourseModule (1) → (N) CourseModuleProgress
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseModuleProgress.CourseModuleId
- **Purpose:** Links progress records to specific modules

#### Course (1) → (N) CourseReviews
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseReview.CourseId
- **Unique Index:** (CourseId, UserId) - one review per user per course
- **Purpose:** Student reviews and ratings for courses

#### Course (N) ↔ (N) ApplicationUsers (Instructors)
- **Relationship:** Many-to-Many (through CourseInstructor junction table)
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseInstructor.CourseId, CourseInstructor.UserId
- **Unique Index:** (CourseId, UserId) - prevents duplicate instructor assignments
- **Purpose:** Multiple instructors can teach a course

#### Course (1) → (N) CourseCertificates
- **Relationship:** One-to-Many
- **Delete Behavior:** Cascade
- **Foreign Key:** CourseCertificate.CourseId
- **Unique Index:** CertificateNumber, EnrollmentId
- **Purpose:** Certificates issued upon course completion

---

### 3. USER RELATIONSHIPS

#### ApplicationUser (1) → (N) QuizAttempts
- **Purpose:** All quiz attempts by a user

#### ApplicationUser (1) → (N) CourseEnrollments
- **Purpose:** All courses a user is enrolled in

#### ApplicationUser (1) → (N) CourseReviews
- **Purpose:** All reviews written by a user

#### ApplicationUser (1) → (N) CourseInstructors
- **Purpose:** All courses where user is an instructor

#### ApplicationUser (1) → (N) CourseCertificates
- **Purpose:** All certificates earned by a user

#### ApplicationUser (1) → (N) Courses (as Creator)
- **Foreign Key:** Course.CreatedByUserId
- **Purpose:** Tracks who created each course

---

## Delete Behavior Strategy

### Cascade Delete
Used when child data should be automatically deleted with parent:
- Quiz → Questions → AnswerOptions
- Course → Modules → Contents
- Enrollment → Progress records

### Restrict Delete
Used to preserve historical/audit data:
- Quiz → QuizAttempts (preserve attempt history)
- Question → UserResponses (preserve response history)
- AnswerOption → UserResponseAnswers (preserve selection history)
- Course → Enrollments (preserve enrollment history)
- CourseCategory → Courses (prevent category deletion if courses exist)

---

## Indexing Strategy

### Performance Indexes
1. **Quiz System:**
   - Quiz: Title, IsActive, CreatedAt
   - Question: (QuizId, OrderIndex) composite
   - AnswerOption: (QuestionId, OrderIndex) composite, IsCorrect
   - QuizAttempt: (UserId, QuizId) composite, StartedAt, CompletedAt, IsCompleted
   - UserResponse: (QuizAttemptId, QuestionId) composite, IsCorrect, AnsweredAt

2. **Course System:**
   - Course: Category, IsPublished, CreatedAt
   - CourseCategory: Name (unique), DisplayOrder, IsActive
   - CourseReview: (CourseId, UserId) unique, (CourseId, Rating), CreatedAt, IsApproved
   - CourseInstructor: (CourseId, UserId) unique, UserId
   - CourseCertificate: CertificateNumber (unique), EnrollmentId (unique), (UserId, CourseId), IssuedAt

### Unique Constraints
- CourseCategory.Name
- CourseReview: (CourseId, UserId)
- CourseInstructor: (CourseId, UserId)
- CourseCertificate.CertificateNumber
- CourseCertificate.EnrollmentId
- UserResponse: (QuizAttemptId, QuestionId)
- UserResponseAnswer: (UserResponseId, AnswerOptionId)

---

## Precision Configuration

### Decimal Fields
- **Scores/Percentages (5,2):** Max 999.99
  - Quiz.PassingScore
  - Question.Points
  - QuizAttempt.Score
  - UserResponse.PointsEarned
  - CourseProgress.QuizScore, ProgressPercentage
  - CourseModuleProgress.ProgressPercentage
  - CourseEnrollment.ProgressPercentage

- **Ratings (3,2):** Max 99.9
  - CourseReview.Rating

- **Financial (10,2):** Max 99,999,999.99
  - Course.Price
  - QuizAttempt.TotalPointsEarned, TotalPointsPossible
  - CourseEnrollment.PaymentAmount

---

## Key Design Patterns

### 1. Soft Delete Support
- Fields like `IsActive`, `IsValid` allow logical deletion
- `RevokedAt`, `RevokedByUserId` track revocation of certificates

### 2. Audit Trail
- `CreatedAt`, `UpdatedAt` timestamps on most entities
- Restrict delete behaviors preserve historical data
- Approval workflows (CourseReview: IsApproved, ApprovedAt, ApprovedByUserId)

### 3. Flexible Relationships
- Optional course-quiz relationship (quizzes can be standalone)
- Optional category relationship (courses can exist without categories initially)
- Multiple instructor support through junction table

### 4. Progress Tracking
- Dual-level progress tracking: content-level and module-level
- Percentage-based progress calculation
- Quiz scores tied to content progress

### 5. Ordering/Sequencing
- OrderIndex fields for questions, answer options, instructors
- Allows custom sequencing and reordering

---

## Navigation Properties Summary

### Course Model
```
Course
├── CreatedBy: ApplicationUser?
├── CourseCategory: CourseCategory?
├── Modules: ICollection<CourseModule>
├── Enrollments: ICollection<CourseEnrollment>
├── Quizzes: ICollection<Quiz>
├── Reviews: ICollection<CourseReview>
├── Instructors: ICollection<CourseInstructor>
└── Certificates: ICollection<CourseCertificate>
```

### ApplicationUser Model
```
ApplicationUser (extends IdentityUser)
├── CourseReviews: ICollection<CourseReview>
├── CourseInstructors: ICollection<CourseInstructor>
└── CourseCertificates: ICollection<CourseCertificate>
```

---

## Migration Status

### Completed Migrations
1. `InitialQuizModel` - Initial quiz system
2. `AddUserResponseTextAnswer` - Text answer support
3. `AddIdentity` - ASP.NET Identity integration
4. `AddCategoryToQuiz` - Quiz categorization
5. `AddUserManagementFields` - User management features
6. `AddCoursePlatformEntities` - Course system (partial)

### Next Migration Required
- **AddCourseReviewInstructorCertificate** - Complete the course platform with:
  - CourseReview entity and relationships
  - CourseInstructor entity and relationships
  - CourseCertificate entity and relationships
  - CourseCategory navigation property fix

---

## Database Context Status

✅ **All DbSets Registered:**
- Quiz System: 6/6 entities
- Course Platform: 10/10 entities
- Identity: ApplicationUser extended

✅ **All Relationships Configured:**
- Quiz System: Fully configured
- Course Platform: Fully configured
- Cross-system: Quiz-Course link configured

✅ **All Navigation Properties:**
- Course model: Complete
- ApplicationUser model: Complete
- All junction tables: Properly configured

---

## Next Steps

1. **Create Migration:**
   ```bash
   dotnet ef migrations add AddCourseReviewInstructorCertificate
   ```

2. **Review Migration:**
   - Verify table creation for course_reviews, course_instructors, course_certificates
   - Check foreign key constraints
   - Verify indexes

3. **Apply Migration:**
   ```bash
   dotnet ef database update
   ```

4. **Test Relationships:**
   - Create test courses with reviews
   - Assign instructors to courses
   - Issue test certificates
   - Verify cascade and restrict behaviors

---

## Notes

- All tables use snake_case column names for PostgreSQL convention
- Navigation properties use PascalCase following C# conventions
- Composite indexes optimize common query patterns
- Restrict delete behaviors ensure data integrity and audit trail
- Certificate verification system supports public certificate validation

