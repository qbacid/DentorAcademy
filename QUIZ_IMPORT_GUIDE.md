# Quiz Import Feature - Complete Guide

## Overview

The Quiz Import feature allows administrators to quickly create quizzes by importing data from **JSON** files. This is perfect for bulk quiz creation and migration from other systems.

## Supported Format

### JSON Format

JSON provides a structured and flexible format for importing quizzes with complete control over all quiz properties.

**Structure:**
```json
{
  "title": "Quiz Title",
  "description": "Quiz description (optional)",
  "passingScore": 70.0,
  "timeLimitMinutes": 15,
  "isActive": true,
  "questions": [
    {
      "questionText": "Your question here?",
      "questionType": "MultipleChoice",
      "explanation": "Explanation shown after answer",
      "points": 1.0,
      "displayOrder": 1,
      "answerOptions": [
        { "optionText": "Option A", "isCorrect": false, "displayOrder": 1 },
        { "optionText": "Option B", "isCorrect": true,  "displayOrder": 2 }
      ]
    }
  ]
}
```

**Example:** `/wwwroot/sample-quiz.json`

## Question Types

The system supports four question types:

1. MultipleChoice: Single correct answer selection
   - Use radio buttons for user selection
   - Only one option should be marked as correct

2. MultipleCheckbox: Multiple correct answers
   - Use checkboxes for user selection
   - Multiple options can be marked as correct
   - User must select ALL correct answers to get points

3. TrueFalse: Boolean questions
   - Should have exactly 2 options: "True" and "False"
   - One option marked as correct

4. ShortAnswer: User provides a short text response (free text)
   - No answer options are required; the user types the answer
   - These questions are not auto-graded and are excluded from the automatic score calculation

## Field Descriptions

### Quiz Level Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | Yes | The quiz title |
| description | string | No | Quiz description shown to users |
| passingScore | decimal | Yes | Minimum score percentage to pass (0-100) |
| timeLimitMinutes | integer | No | Time limit in minutes (null for unlimited) |
| isActive | boolean | No | Whether quiz is active (default: true) |

### Question Level Fields

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| questionText | string | Yes | The question text |
| questionType | string | Yes | "MultipleChoice", "MultipleCheckbox", "TrueFalse", or "ShortAnswer" |
| explanation | string | No | Explanation shown after answering |
| points | decimal | Yes | Points awarded for correct answer (ShortAnswer is not auto-graded) |
| displayOrder | integer | Yes | Display order of question |

### Answer Option Fields (Not used for ShortAnswer)

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| optionText | string | Yes | The answer option text |
| isCorrect | boolean | Yes | Whether this option is correct |
| displayOrder | integer | Yes | Display order of option |

## Import Process

1. Navigate to `/quiz-import` in the application
2. Select your JSON file using the file picker
3. Preview the JSON content (optional)
4. Click "Import Quiz" button
5. Review the import results (success/errors/warnings)
6. Navigate to the quiz to test it

## Validation Rules

The import service validates:

- ✅ Quiz title is required
- ✅ Passing score must be between 0 and 100
- ✅ At least one question is required
- ✅ Each question must have text
- ✅ For MultipleChoice, MultipleCheckbox, and TrueFalse: at least one answer option and at least one correct answer
- ✅ For TrueFalse specifically: exactly 2 options are recommended (True/False)
- ✅ For ShortAnswer: no options are required
- ✅ Valid question types only

## ShortAnswer Example

```json
{
  "questionText": "In one sentence, explain why flossing is important.",
  "questionType": "ShortAnswer",
  "points": 0,
  "displayOrder": 6,
  "explanation": "Flossing removes plaque and food particles from between teeth, preventing gum disease.",
  "answerOptions": []
}
```

## Error Handling

If validation fails, you'll receive:
- Errors: Critical issues that prevent import
- Warnings: Non-critical issues (e.g., invalid question type - question skipped)

## Best Practices

1. Start with the sample: Download and modify `/sample-quiz.json`
2. Validate JSON: Use a JSON validator before importing
3. Test incrementally: Start with a small quiz to test the format
4. Use displayOrder: Control the sequence of questions and answers
5. Add explanations: Help users learn from their mistakes
6. Set appropriate points: Can use decimals (e.g., 0.5, 1.0, 2.5). For ShortAnswer, consider 0 points if it's ungraded.

## Sample JSON Download

A complete sample quiz is available at `/sample-quiz.json` which includes:
- All four question types
- Proper structure and formatting
- Example explanations
- Proper ordering

Download this file and modify it to create your own quizzes.

## Troubleshooting

### Common Issues

Issue: "Failed to parse JSON content"
- Solution: Validate your JSON syntax using an online JSON validator

Issue: "Quiz must have at least one question"
- Solution: Ensure the `questions` array is not empty

Issue: "Question has no correct answer marked"
- Solution: Set `isCorrect: true` for at least one answer option (not applicable to ShortAnswer)

Issue: "Invalid question type"
- Solution: Use only "MultipleChoice", "MultipleCheckbox", "TrueFalse", or "ShortAnswer"

## API Usage (For Developers)

The import service can be used programmatically:

```csharp
// Inject the service
private readonly QuizImportService _importService;

// Import from JSON string
var result = await _importService.ImportFromJsonAsync(jsonContent);

if (result.Success)
{
    // Quiz imported successfully
    var quizId = result.QuizId;
    var questionCount = result.QuestionsImported;
}
else
{
    // Handle errors
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error);
    }
}
```

## Future Enhancements

Potential future features:
- Export quiz to JSON format
- Quiz templates and categories
- Bulk quiz operations
- Quiz versioning and history
