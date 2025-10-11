# Reusable Components Implementation

## Overview
Successfully created reusable components following the visual style from the Quizzes page, with authentication/authorization support built-in.

## Components Created

### 1. **QuizCardGrid.razor** (`Components/Shared/`)
**Purpose:** Displays a responsive grid of quiz cards with consistent styling across the application.

**Features:**
- ✅ Uses the preferred visual style from QuizList page
- ✅ Purple gradient header (`#667eea` to `#764ba2`)
- ✅ Hover effects with smooth transitions
- ✅ Keyboard navigation support (Enter/Space key)
- ✅ Auth-ready with `CascadingParameter` for `AuthenticationState`
- ✅ Event callback for quiz selection

**Parameters:**
- `List<QuizCardDto> Quizzes` - List of quizzes to display
- `EventCallback<int> OnQuizSelected` - Callback when a quiz is selected

**Usage:**
```razor
<QuizCardGrid Quizzes="@quizzes" OnQuizSelected="@NavigateToQuiz" />
```

---

### 2. **LoadingSpinner.razor** (`Components/Shared/`)
**Purpose:** Reusable loading indicator with customizable message.

**Features:**
- ✅ Consistent styling (3rem size, primary color)
- ✅ Centered layout with optional message
- ✅ Simple and clean

**Parameters:**
- `string Message` - Optional loading message (default: "Loading...")

**Usage:**
```razor
<LoadingSpinner Message="Loading quizzes..." />
```

---

### 3. **EmptyState.razor** (`Components/Shared/`)
**Purpose:** Generic empty state component with optional action button.

**Features:**
- ✅ Customizable icon, title, and message
- ✅ **Optional authentication support** - works with or without auth
- ✅ **Smart role-based display:**
  - If `RequiredRole` is **empty/null**: Action visible to **all users** (public)
  - If `RequiredRole` is **specified**: Action only visible to users in that role
- ✅ Supports both URL navigation and event callbacks
- ✅ Auth-ready with `CascadingParameter` for `AuthenticationState`
- ✅ Option to show action for unauthorized users

**Parameters:**
- `string Title` - Main ~~~~heading
- `string Message` - Description text
- `string IconClass` - Bootstrap icon class
- `bool ShowAction` - Whether to show action button
- `string ActionText` - Button text
- `string? ActionIconClass` - Optional button icon
- `string? ActionUrl` - URL for navigation
- `EventCallback OnAction` - Alternative to URL navigation
- `string? RequiredRole` - **OPTIONAL** - Role required to see action (e.g., "Admin"). **If null/empty, action is public**
- `bool ShowActionForUnauthorized` - Show action even if not authorized

**Usage Examples:**

**Public action (no auth required):**
```razor
<EmptyState 
    Title="No Items Found"
    Message="Try adjusting your search."
    IconClass="bi bi-search"
    ShowAction="true"
    ActionText="Clear Filters"
    OnAction="@ClearFilters" />
<!-- RequiredRole is omitted/null = visible to everyone -->
```

**Admin-only action:**
```razor
<EmptyState 
    Title="No Quizzes Available Yet"
    Message="Import your first quiz to get started!"
    IconClass="bi bi-inbox"
    ShowAction="true"
    ActionText="Import Quiz"
    ActionIconClass="bi bi-upload"
    ActionUrl="/quiz-import"
    RequiredRole="Admin" />
<!-- RequiredRole="Admin" = only Admins see the button -->
```

---

## New DTO Created

### **QuizCardDto.cs** (`DTOs/`)
**Purpose:** Standardized DTO for displaying quiz information in cards/lists.

**Properties:**
- `int Id` - Quiz identifier
- `string Title` - Quiz title
- `string? Description` - Optional description
- `decimal PassingScore` - Passing percentage
- `int? TimeLimitMinutes` - Optional time limit
- `int QuestionCount` - Number of questions

---

## Pages Updated

### **Home.razor**
- ✅ Replaced loading spinner with `<LoadingSpinner />`
- ✅ Replaced empty state with `<EmptyState />` (Admin-only action)
- ✅ Replaced quiz grid with `<QuizCardGrid />`
- ✅ Reduced code duplication by ~100 lines

### **QuizList.razor**
- ✅ Replaced loading spinner with `<LoadingSpinner />`
- ✅ Replaced empty state with `<EmptyState />` (Admin-only action)
- ✅ Replaced quiz grid with `<QuizCardGrid />`
- ✅ Removed duplicate CSS file (styles moved to shared component)
- ✅ Reduced code duplication by ~60 lines

---

## Authentication & Authorization Support

All components are **auth-ready** with:

1. **Cascading Authentication State:**
   ```csharp
   [CascadingParameter]
   private Task<AuthenticationState>? AuthenticationState { get; set; }
   ```

2. **Built-in AuthorizeView:**
   - `EmptyState` uses `AuthorizeView` with `Roles` parameter
   - Automatically shows/hides action buttons based on user role

3. **Flexible Authorization:**
   - Components accept authentication cascading parameters
   - Can be used with or without authentication
   - Role-based access control built-in

---

## Benefits Achieved

✅ **Consistency:** Same visual style across Home and QuizList pages  
✅ **Maintainability:** Update once, changes reflect everywhere  
✅ **Reusability:** Components can be used in future pages  
✅ **Auth-Ready:** Built-in support for authentication/authorization  
✅ **Clean Code:** Reduced duplication by ~160 lines total  
✅ **Accessibility:** Keyboard navigation and ARIA support  
✅ **Performance:** Minimal overhead, efficient rendering  

---

## File Structure

```
Components/
  ├── Shared/
  │   ├── QuizCardGrid.razor
  │   ├── QuizCardGrid.razor.css
  │   ├── LoadingSpinner.razor
  │   └── EmptyState.razor
  ├── Pages/
  │   ├── Home.razor (✨ refactored)
  │   ├── QuizList.razor (✨ refactored)
  │   └── ...
  └── _Imports.razor (✨ updated with Shared namespace)

DTOs/
  ├── QuizCardDto.cs (✨ new)
  └── ...
```

---

## Next Steps

Future component candidates:
- Stats card component (for the 4-stat display on Home page)
- Quiz result card component (for TakeQuiz results)
- Form input components with consistent styling
- Alert/notification component

---

**Build Status:** ✅ Successful - No errors or warnings
