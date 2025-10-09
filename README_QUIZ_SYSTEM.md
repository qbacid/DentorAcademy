# Quiz System - Complete Implementation Guide

## Overview

A complete data model for a flashcard-style quiz system with scoring capabilities, built with Entity Framework Core and optimized for PostgreSQL database.

## Features

✅ **Three Question Types:**
- Multiple Choice (single correct answer)
- Multiple Checkbox (multiple correct answers)
- True/False

✅ **Flashcard Experience:**
- Users select answers
- System shows correct answer and explanation
- Move to next question

✅ **Scoring System:**
- Track points per question
- Calculate percentage scores
- Pass/fail determination
- Complete attempt history

✅ **PostgreSQL Optimized:**
- Snake case naming convention
- Strategic indexes for performance
- Proper cascade/restrict deletion rules
- Connection resilience with retry logic

## Data Model Structure

### Core Entities

1. **Quiz** - Container for questions with passing score and time limit
2. **Question** - Individual questions with type, points, and explanation
3. **AnswerOption** - Possible answers with correct/incorrect flag
4. **QuizAttempt** - User's attempt at a quiz with final score
5. **UserResponse** - User's answer to specific question
6. **UserResponseAnswer** - Junction table for selected answers

### Relationships

```
Quiz 1:N Question 1:N AnswerOption
Quiz 1:N QuizAttempt 1:N UserResponse
Question 1:N UserResponse
UserResponse N:M AnswerOption (via UserResponseAnswer)
```

## Files Created

### Models
- `Models/QuestionType.cs` - Enum for question types
- `Models/Quiz.cs` - Quiz entity
- `Models/Question.cs` - Question entity
- `Models/AnswerOption.cs` - Answer option entity
- `Models/QuizAttempt.cs` - Quiz attempt entity
- `Models/UserResponse.cs` - User response entity
- `Models/UserResponseAnswer.cs` - Junction table entity

### Data Layer
- `Data/QuizDbContext.cs` - EF Core DbContext with configurations
- `Data/SeedData.cs` - Sample data for testing

### Services
- `Services/QuizScoringService.cs` - Business logic for scoring

### Configuration
- `appsettings.json` - Updated with connection string
- `Program.cs` - Registered DbContext and services

## Setup Instructions

### 1. Update Connection String

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "QuizDatabase": "Host=your_host;Database=dentor_academy_db;Username=your_user;Password=your_password"
  }
}
```

### 2. Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### 3. Create Initial Migration

```bash
cd Dentor.Academy.Web
dotnet ef migrations add InitialQuizModel
```

### 4. Apply Migration to Database

```bash
dotnet ef database update
```

### 5. (Optional) Seed Sample Data

In your `Program.cs`, before `app.Run()`:

```csharp
// Seed sample data (optional)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    await SeedData.InitializeAsync(context);
}
```

## Usage Examples

### Creating a Quiz

```csharp
var quiz = new Quiz
{
    Title = "Dental Anatomy Basics",
    Description = "Test your knowledge",
    PassingScore = 70.0m,
    TimeLimitMinutes = 15,
    IsActive = true
};

context.Quizzes.Add(quiz);
await context.SaveChangesAsync();
```

### Creating Questions

```csharp
// Multiple Choice Question
var question = new Question
{
    QuizId = quiz.Id,
    QuestionText = "How many teeth?",
    QuestionType = QuestionType.MultipleChoice,
    Explanation = "Adults have 32 teeth",
    Points = 1.0m,
    OrderIndex = 1
};

context.Questions.Add(question);
await context.SaveChangesAsync();

// Add answer options
var options = new List<AnswerOption>
{
    new() { QuestionId = question.Id, OptionText = "28", IsCorrect = false, OrderIndex = 1 },
    new() { QuestionId = question.Id, OptionText = "32", IsCorrect = true, OrderIndex = 2 }
};

context.AnswerOptions.AddRange(options);
await context.SaveChangesAsync();
```

### Starting a Quiz Attempt

```csharp
var attempt = new QuizAttempt
{
    QuizId = quiz.Id,
    UserId = "user123",
    StartedAt = DateTime.UtcNow,
    IsCompleted = false
};

context.QuizAttempts.Add(attempt);
await context.SaveChangesAsync();
```

### Recording User Response

```csharp
// Inject the service
private readonly QuizScoringService _scoringService;

// Record answer (e.g., user selected options 2 and 3)
var response = await _scoringService.RecordUserResponse(
    quizAttemptId: attempt.Id,
    questionId: question.Id,
    selectedAnswerOptionIds: new List<int> { 2, 3 }
);
```

### Calculating Final Score

```csharp
var finalAttempt = await _scoringService.CalculateFinalScore(attempt.Id);

Console.WriteLine($"Score: {finalAttempt.Score}%");
Console.WriteLine($"Passed: {finalAttempt.Passed}");
Console.WriteLine($"Points: {finalAttempt.TotalPointsEarned}/{finalAttempt.TotalPointsPossible}");
```

### Getting Detailed Results

```csharp
var results = await _scoringService.GetAttemptResults(attempt.Id);

foreach (var question in results.QuestionResults)
{
    Console.WriteLine($"Q: {question.QuestionText}");
    Console.WriteLine($"Correct: {question.IsCorrect}");
    Console.WriteLine($"Explanation: {question.Explanation}");
}
```

## Database Schema

### Table Structure

**quizzes**
- id (PK)
- title
- description
- passing_score
- time_limit_minutes
- is_active
- created_at
- updated_at

**questions**
- id (PK)
- quiz_id (FK)
- question_text
- question_type (1=MultipleChoice, 2=MultipleCheckbox, 3=TrueFalse)
- explanation
- points
- order_index
- created_at
- updated_at

**answer_options**
- id (PK)
- question_id (FK)
- option_text
- is_correct
- order_index
- created_at

**quiz_attempts**
- id (PK)
- quiz_id (FK)
- user_id
- started_at
- completed_at
- score
- total_points_earned
- total_points_possible
- passed
- is_completed

**user_responses**
- id (PK)
- quiz_attempt_id (FK)
- question_id (FK)
- is_correct
- points_earned
- answered_at

**user_response_answers**
- id (PK)
- user_response_id (FK)
- answer_option_id (FK)
- selected_at

## Performance Considerations

### Indexes
The model includes strategic indexes on:
- Foreign keys for efficient joins
- `(user_id, quiz_id)` for finding user's quiz attempts
- `(quiz_id, order_index)` for ordered question retrieval
- `(question_id, order_index)` for ordered answer options
- `is_active`, `is_completed` for filtering
- Timestamp fields for time-based queries

### Cascade Rules
- **CASCADE**: Quiz → Question → AnswerOption (structure)
- **RESTRICT**: Quiz → QuizAttempt, Question → UserResponse (preserve history)

## NuGet Packages Installed

- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)
- `Microsoft.EntityFrameworkCore.Design` (9.0.9)
- `EFCore.NamingConventions` (9.0.0)

## Additional Documentation

- `QUIZ_DATA_MODEL.md` - Detailed entity documentation
- `DATABASE_DIAGRAM.md` - Visual database schema

## Next Steps

1. **Create Razor Components** for flashcard UI
2. **Add Identity** for user authentication
3. **Implement API Controllers** if building as API
4. **Add Validation** for business rules
5. **Create Reports** for quiz analytics
6. **Add Media Support** for images/videos in questions
7. **Implement Question Bank** for random quiz generation

## Support

For questions or issues with the data model, refer to:
- `QUIZ_DATA_MODEL.md` for detailed entity documentation
- `DATABASE_DIAGRAM.md` for relationship visualization
- EF Core migrations if database schema needs updates

---

**Built with:** .NET 9, Entity Framework Core 9, PostgreSQL
**Optimized for:** Flashcard-style quiz delivery with complete scoring capabilities

