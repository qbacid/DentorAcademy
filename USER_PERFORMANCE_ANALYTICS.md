# User Performance Analytics System

## Overview
Implemented a comprehensive performance tracking and analytics system that calculates user scores by category, identifies strengths and improvement areas, and provides insights for future student dashboard features.

---

## 🎯 Key Features

### 1. **Category-Based Performance Tracking**
- Calculates average scores per category
- Tracks improvement over time
- Identifies strength areas (80%+ scores)
- Highlights areas needing focus (<70% scores)

### 2. **Performance Insights**
- Overall statistics (total quizzes, questions, average score)
- Best score per category
- Latest score and attempt date
- Performance levels: Excellent, Very Good, Good, Fair, Needs Improvement

### 3. **Service-Based Architecture**
- Reusable `UserPerformanceService` for future dashboard features
- All analytics logic centralized in the service layer
- Ready for API endpoints or additional UI components

---

## 📦 New Components Created

### **DTOs Created** (`DTOs/UserPerformanceDto.cs`)

#### 1. **CategoryPerformanceDto**
Detailed performance statistics for a single category:
```csharp
- Category: string
- QuizzesTaken: int
- QuestionsAnswered: int
- CorrectAnswers: int
- AverageScore: decimal
- BestScore: decimal?
- LatestScore: decimal?
- LastAttemptDate: DateTime?
- PerformanceLevel: string (Excellent, Good, etc.)
- IsStrength: bool (score >= 80%)
- NeedsImprovement: bool (score < 70%)
```

#### 2. **UserPerformanceSummaryDto**
Overall user performance summary:
```csharp
- UserId: string
- UserEmail: string
- TotalQuizzesTaken: int
- TotalQuestionsAnswered: int
- OverallAverageScore: decimal
- CategoryPerformances: List<CategoryPerformanceDto>
- StrengthAreas: List<string> (categories with 80%+)
- ImprovementAreas: List<string> (categories with <70%)
- LastActivityDate: DateTime?
```

#### 3. **QuizAttemptSummaryDto**
Individual quiz attempt information with category context.

---

## 🔧 Service Methods

### **UserPerformanceService** (`Services/UserPerformanceService.cs`)

#### Core Methods:

**1. GetUserPerformanceSummaryAsync(userId)**
- Returns complete performance overview
- Includes all categories and overall statistics
- Use for dashboard main view

**2. CalculateCategoryPerformancesAsync(userId)**
- Calculates detailed performance for each category
- Ordered by average score (best to worst)
- Use for category breakdown displays

**3. GetCategoryPerformanceAsync(userId, category)**
- Get performance for a specific category
- Use for category-specific insights

**4. GetUserQuizAttemptsAsync(userId)**
- Returns all quiz attempts with summaries
- Includes category information
- Use for attempt history views

**5. GetUserQuizAttemptsByCategoryAsync(userId, category)**
- Filter attempts by specific category
- Use for category drill-down views

**6. GetTopStrengthAreasAsync(userId, topCount = 3)**
- Returns top performing categories
- Requires at least 2 attempts for reliability
- Use for "Your Strengths" displays

**7. GetImprovementAreasAsync(userId, bottomCount = 3)**
- Returns categories needing improvement
- Filters scores < 80%
- Use for "Focus Areas" displays

**8. HasImprovedInCategoryAsync(userId, category)**
- Checks if user improved over time
- Compares first half vs second half of attempts
- Use for progress indicators

---

## 🎨 UI Components

### **My Performance Page** (`/my-performance`)

**Features:**
- ✅ Overall statistics card (4 key metrics)
- ✅ Strengths section (categories with 80%+ scores)
- ✅ Focus areas section (categories with <70% scores)
- ✅ Category performance cards with:
  - Average score with color coding
  - Performance level badge
  - Progress bar visualization
  - Detailed stats (quizzes taken, correct answers, best score, last attempt)
  - Color-coded borders (green for strengths, yellow for improvement areas)

**Access:**
- Available in navigation menu for authenticated users
- Displays sign-in prompt for anonymous users
- Shows empty state if no quizzes taken yet

---

## 🎨 Visual Design

### Score Color Coding:
- **90%+**: Green (Excellent) 🟢
- **80-89%**: Blue/Info (Very Good) 🔵
- **70-79%**: Primary (Good) 🟦
- **60-69%**: Yellow (Fair) 🟡
- **<60%**: Red (Needs Improvement) 🔴

### Performance Levels:
- **Excellent**: ≥90%
- **Very Good**: 80-89%
- **Good**: 70-79%
- **Fair**: 60-69%
- **Needs Improvement**: <60%

---

## 📊 Use Cases

### For Students:
1. **Track Progress**: See overall improvement across all categories
2. **Identify Strengths**: Know which topics you've mastered
3. **Focus Learning**: Target specific categories needing improvement
4. **Set Goals**: Work towards improving weak areas

### For Future Dashboard Features:
1. **Learning Path Recommendations**: Suggest quizzes based on weak categories
2. **Achievement System**: Award badges for category mastery
3. **Progress Charts**: Visualize improvement over time
4. **Peer Comparison**: Compare category performance with community averages
5. **Study Plans**: Generate personalized study recommendations

---

## 🔌 Integration Points

### Current Integration:
- ✅ Registered in dependency injection (`Program.cs`)
- ✅ Available in navigation menu
- ✅ Uses existing `QuizAttempt` and `UserResponse` data
- ✅ Works with the Category field added to Quiz model

### Future Integration Possibilities:

**1. Home Page Enhancement:**
```csharp
// Show personalized recommendations based on weak categories
var improvementAreas = await performanceService.GetImprovementAreasAsync(userId);
var recommendedQuizzes = quizzes.Where(q => improvementAreas.Any(a => a == q.Category));
```

**2. Quiz Results Page:**
```csharp
// Show category performance after completing a quiz
var categoryPerf = await performanceService.GetCategoryPerformanceAsync(userId, quizCategory);
// Display: "Your average in this category is X%"
```

**3. API Endpoints (Future):**
```csharp
// REST API for mobile apps or external systems
[HttpGet("api/performance/{userId}")]
public async Task<UserPerformanceSummaryDto> GetPerformance(string userId)
{
    return await performanceService.GetUserPerformanceSummaryAsync(userId);
}
```

---

## 📈 Data Requirements

### For Accurate Analytics:
- **Minimum Data**: At least 1 completed quiz attempt
- **Reliable Statistics**: 2+ attempts per category recommended
- **Category Assignment**: Quizzes should have the Category field populated

### Current Data Model Support:
- ✅ Uses existing `QuizAttempt.Score` for calculations
- ✅ Uses `Quiz.Category` for grouping
- ✅ Uses `UserResponse.IsCorrect` for correct answer counts
- ✅ No database schema changes required

---

## 🚀 Example Usage

### In a Razor Component:
```csharp
@inject UserPerformanceService PerformanceService

@code {
    private UserPerformanceSummaryDto? summary;
    
    protected override async Task OnInitializedAsync()
    {
        var userId = GetCurrentUserId(); // Your auth logic
        summary = await PerformanceService.GetUserPerformanceSummaryAsync(userId);
        
        // Display strengths
        foreach (var strength in summary.StrengthAreas)
        {
            Console.WriteLine($"Strength: {strength}");
        }
        
        // Display improvement areas
        foreach (var area in summary.ImprovementAreas)
        {
            Console.WriteLine($"Needs work: {area}");
        }
    }
}
```

### In a Service/Controller:
```csharp
public class RecommendationService
{
    private readonly UserPerformanceService _performanceService;
    
    public async Task<List<QuizCardDto>> GetRecommendedQuizzesAsync(string userId)
    {
        var improvements = await _performanceService.GetImprovementAreasAsync(userId);
        var categories = improvements.Select(i => i.Category).ToList();
        
        // Return quizzes from weak categories
        return await GetQuizzesByCategories(categories);
    }
}
```

---

## ✅ Testing Checklist

To test the performance analytics:

1. **Create Test Data:**
   - Import quizzes with different categories
   - Take multiple quizzes in each category
   - Vary your scores (some high, some low)

2. **Verify Calculations:**
   - Check if average scores are accurate
   - Confirm strength areas show for 80%+ scores
   - Verify improvement areas show for <70% scores

3. **Test Edge Cases:**
   - No quizzes taken (empty state)
   - Only one category
   - Multiple attempts in same category
   - Perfect scores (100%)
   - Very low scores

---

## 🎯 Future Enhancements

### Phase 2 - Advanced Analytics:
- [ ] Time-series charts (score trends over time)
- [ ] Comparison with community averages per category
- [ ] Detailed question-level analytics
- [ ] Export performance reports (PDF/CSV)

### Phase 3 - Gamification:
- [ ] Achievement badges per category
- [ ] Leaderboards by category
- [ ] Streak tracking (consecutive days)
- [ ] Category mastery levels

### Phase 4 ~~~~- AI Recommendations:
- [ ] Smart quiz recommendations
- [ ] Personalized study plans
- [ ] Difficulty adjustment based on performance
- [ ] Spaced repetition scheduling

---

## 📝 Notes

- **Performance**: All queries are optimized with proper includes and filtering
- **Privacy**: User data is isolated by userId - no cross-user data leaks
- **Scalability**: Service methods use async/await for better performance
- **Flexibility**: Easy to extend with additional metrics or calculations
- **Testing**: Can be unit tested independently of UI components

---

**Build Status**: ✅ Successful - All components compile without errors
**Ready for**: Student dashboard, mobile apps, API integration, advanced analytics

