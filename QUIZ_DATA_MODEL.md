# Quiz System Data Model Documentation

## Overview
This data model supports a flashcard-style quiz system with three types of questions (Multiple Choice, Multiple Checkbox, True/False), complete with scoring capabilities and response tracking.

## Database Entities

### 1. **Quiz**
Represents the main quiz container.

**Table:** `quizzes`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| title | varchar(200) | Quiz title |
| description | varchar(1000) | Optional description |
| passing_score | decimal(5,2) | Minimum score to pass (percentage) |
| time_limit_minutes | int | Optional time limit |
| is_active | boolean | Quiz availability status |
| created_at | timestamp | Creation timestamp |
| updated_at | timestamp | Last update timestamp |

**Indexes:**
- `title`
- `is_active`
- `created_at`

---

### 2. **Question**
Represents individual questions in flashcard format.

**Table:** `questions`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| quiz_id | int (FK) | Reference to Quiz |
| question_text | varchar(1000) | The question content |
| question_type | int | 1=MultipleChoice, 2=MultipleCheckbox, 3=TrueFalse |
| explanation | varchar(2000) | Shown after user answers |
| points | decimal(5,2) | Points for correct answer |
| order_index | int | Display order in quiz |
| created_at | timestamp | Creation timestamp |
| updated_at | timestamp | Last update timestamp |

**Indexes:**
- `(quiz_id, order_index)` - Composite index for efficient ordering
- Foreign key to `quizzes` with CASCADE delete

**Question Types:**
- **1 - MultipleChoice**: Single correct answer
- **2 - MultipleCheckbox**: Multiple correct answers
- **3 - TrueFalse**: True or False

---

### 3. **AnswerOption**
Represents possible answers for each question.

**Table:** `answer_options`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| question_id | int (FK) | Reference to Question |
| option_text | varchar(500) | Answer text |
| is_correct | boolean | Whether this is a correct answer |
| order_index | int | Display order |
| created_at | timestamp | Creation timestamp |

**Indexes:**
- `(question_id, order_index)` - Composite index
- `is_correct`
- Foreign key to `questions` with CASCADE delete

**Usage by Question Type:**
- **MultipleChoice**: One option has `is_correct = true`
- **MultipleCheckbox**: Multiple options can have `is_correct = true`
- **TrueFalse**: Two options (True/False), one has `is_correct = true`

---

### 4. **QuizAttempt**
Tracks user attempts to complete a quiz.

**Table:** `quiz_attempts`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| quiz_id | int (FK) | Reference to Quiz |
| user_id | varchar(450) | User identifier |
| started_at | timestamp | When attempt started |
| completed_at | timestamp | When attempt completed |
| score | decimal(5,2) | Final percentage score (0-100) |
| total_points_earned | decimal(10,2) | Points earned |
| total_points_possible | decimal(10,2) | Maximum possible points |
| passed | boolean | Whether user passed |
| is_completed | boolean | Completion status |

**Indexes:**
- `(user_id, quiz_id)` - Find user's attempts for a quiz
- `started_at`
- `completed_at`
- `is_completed`
- Foreign key to `quizzes` with RESTRICT delete (preserves history)

---

### 5. **UserResponse**
Stores user's response to each question.

**Table:** `user_responses`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| quiz_attempt_id | int (FK) | Reference to QuizAttempt |
| question_id | int (FK) | Reference to Question |
| is_correct | boolean | Whether answer was correct |
| points_earned | decimal(5,2) | Points awarded |
| answered_at | timestamp | When answered |

**Indexes:**
- `(quiz_attempt_id, question_id)` - UNIQUE composite index (one response per question)
- `is_correct`
- `answered_at`
- Foreign key to `quiz_attempts` with CASCADE delete
- Foreign key to `questions` with RESTRICT delete (preserves history)

---

### 6. **UserResponseAnswer**
Junction table linking responses to selected answer options.

**Table:** `user_response_answers`

| Column | Type | Description |
|--------|------|-------------|
| id | int (PK) | Primary key |
| user_response_id | int (FK) | Reference to UserResponse |
| answer_option_id | int (FK) | Reference to AnswerOption |
| selected_at | timestamp | When selected |

**Indexes:**
- `(user_response_id, answer_option_id)` - UNIQUE composite (prevents duplicate selections)
- Foreign key to `user_responses` with CASCADE delete
- Foreign key to `answer_options` with RESTRICT delete (preserves history)

**Usage:**
- **MultipleChoice/TrueFalse**: One record per response
- **MultipleCheckbox**: Multiple records per response

---

## Entity Relationships

```
Quiz (1) ──→ (Many) Question
Quiz (1) ──→ (Many) QuizAttempt

Question (1) ──→ (Many) AnswerOption
Question (1) ──→ (Many) UserResponse

QuizAttempt (1) ──→ (Many) UserResponse

UserResponse (1) ──→ (Many) UserResponseAnswer
AnswerOption (1) ──→ (Many) UserResponseAnswer
```

## PostgreSQL Optimizations

### 1. **Snake Case Naming Convention**
All table and column names use PostgreSQL's conventional snake_case naming.

### 2. **Indexes**
Strategic indexes are placed on:
- Foreign keys for faster joins
- Frequently queried fields (user_id, quiz_id, is_active)
- Timestamp fields for time-based queries
- Composite indexes for common query patterns

### 3. **Cascade vs Restrict**
- **CASCADE**: Used for quiz structure (deleting quiz removes questions/options)
- **RESTRICT**: Used for historical data (prevents deletion of referenced attempts/responses)

### 4. **Decimal Precision**
- Scores use `decimal(5,2)` for percentage precision (0.00 to 999.99)
- Points use `decimal(10,2)` for accumulation

### 5. **Connection Resilience**
- Automatic retry on failure (3 attempts, 5-second delay)
- 30-second command timeout
- Connection pooling via Npgsql

## Workflow Example

### Creating a Quiz
1. Create `Quiz` record
2. Create `Question` records with appropriate `question_type`
3. Create `AnswerOption` records (mark correct answers with `is_correct = true`)

### Taking a Quiz
1. Create `QuizAttempt` record when user starts
2. For each question:
   - User selects answer(s) in flashcard UI
   - Create `UserResponse` record
   - Create `UserResponseAnswer` record(s) for selected options
   - Calculate `is_correct` by comparing selected vs correct options
   - Award points if correct
3. After all questions answered:
   - Calculate total score
   - Update `QuizAttempt` with final score and completion status

### Scoring Logic
The `QuizScoringService` handles:
- **Evaluation**: Compares selected answers with correct answers
- **Partial Credit**: Currently all-or-nothing (can be modified)
- **Final Score**: `(total_points_earned / total_points_possible) * 100`
- **Pass/Fail**: Compare score against `passing_score`

## Usage Examples

### Service Registration (Program.cs)
```csharp
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<QuizScoringService>();
```

### Recording a Response
```csharp
await quizScoringService.RecordUserResponse(
    quizAttemptId: 1,
    questionId: 5,
    selectedAnswerOptionIds: new List<int> { 12, 15 } // Multiple for checkboxes
);
```

### Calculating Final Score
```csharp
var attempt = await quizScoringService.CalculateFinalScore(quizAttemptId: 1);
// Returns QuizAttempt with score, passed status, etc.
```

### Getting Results
```csharp
var results = await quizScoringService.GetAttemptResults(quizAttemptId: 1);
// Returns detailed breakdown with correct/incorrect answers and explanations
```

## Migration Commands

### Create Initial Migration
```bash
dotnet ef migrations add InitialQuizModel
```

### Apply Migration to Database
```bash
dotnet ef database update
```

### Generate SQL Script
```bash
dotnet ef migrations script -o migration.sql
```

## Configuration

Update `appsettings.json` with your PostgreSQL connection:

```json
{
  "ConnectionStrings": {
    "QuizDatabase": "Host=localhost;Database=dentor_academy_db;Username=postgres;Password=your_password"
  }
}
```

## Future Enhancements

Consider adding:
- Question categories/tags for organization
- Question difficulty levels
- Time tracking per question
- Partial credit scoring for multiple checkbox questions
- Quiz templates and cloning
- Question randomization
- Answer shuffling
- Multi-language support
- Media attachments (images, audio, video)
- Question pools for random quiz generation

