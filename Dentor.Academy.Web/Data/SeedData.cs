using Dentor.Academy.Web.Models;

namespace Dentor.Academy.Web.Data;

/// <summary>
/// Example seed data for the Quiz system
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(QuizDbContext context)
    {
        // Check if data already exists
        if (context.Quizzes.Any())
        {
            return; // Database has been seeded
        }

        // Create a sample quiz
        var quiz = new Quiz
        {
            Title = "Dental Anatomy Basics",
            Description = "Test your knowledge of basic dental anatomy",
            PassingScore = 70.0m,
            TimeLimitMinutes = 15,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();

        // Question 1: Multiple Choice
        var question1 = new Question
        {
            QuizId = quiz.Id,
            QuestionText = "How many permanent teeth does an adult human typically have?",
            QuestionType = QuestionType.MultipleChoice,
            Explanation = "Adults typically have 32 permanent teeth, including wisdom teeth.",
            Points = 1.0m,
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Questions.Add(question1);
        await context.SaveChangesAsync();

        // Answer options for Question 1
        var q1Options = new List<AnswerOption>
        {
            new() { QuestionId = question1.Id, OptionText = "28", IsCorrect = false, OrderIndex = 1 },
            new() { QuestionId = question1.Id, OptionText = "30", IsCorrect = false, OrderIndex = 2 },
            new() { QuestionId = question1.Id, OptionText = "32", IsCorrect = true, OrderIndex = 3 },
            new() { QuestionId = question1.Id, OptionText = "34", IsCorrect = false, OrderIndex = 4 }
        };

        context.AnswerOptions.AddRange(q1Options);
        await context.SaveChangesAsync();

        // Question 2: Multiple Checkbox
        var question2 = new Question
        {
            QuizId = quiz.Id,
            QuestionText = "Which of the following are types of teeth? (Select all that apply)",
            QuestionType = QuestionType.MultipleCheckbox,
            Explanation = "Incisors, Canines, Premolars, and Molars are the four types of teeth.",
            Points = 2.0m,
            OrderIndex = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Questions.Add(question2);
        await context.SaveChangesAsync();

        // Answer options for Question 2
        var q2Options = new List<AnswerOption>
        {
            new() { QuestionId = question2.Id, OptionText = "Incisors", IsCorrect = true, OrderIndex = 1 },
            new() { QuestionId = question2.Id, OptionText = "Canines", IsCorrect = true, OrderIndex = 2 },
            new() { QuestionId = question2.Id, OptionText = "Bicuspids", IsCorrect = false, OrderIndex = 3 },
            new() { QuestionId = question2.Id, OptionText = "Premolars", IsCorrect = true, OrderIndex = 4 },
            new() { QuestionId = question2.Id, OptionText = "Molars", IsCorrect = true, OrderIndex = 5 }
        };

        context.AnswerOptions.AddRange(q2Options);
        await context.SaveChangesAsync();

        // Question 3: True/False
        var question3 = new Question
        {
            QuizId = quiz.Id,
            QuestionText = "The enamel is the hardest substance in the human body.",
            QuestionType = QuestionType.TrueFalse,
            Explanation = "True! Tooth enamel is the hardest and most mineralized substance in the human body.",
            Points = 1.0m,
            OrderIndex = 3,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Questions.Add(question3);
        await context.SaveChangesAsync();

        // Answer options for Question 3
        var q3Options = new List<AnswerOption>
        {
            new() { QuestionId = question3.Id, OptionText = "True", IsCorrect = true, OrderIndex = 1 },
            new() { QuestionId = question3.Id, OptionText = "False", IsCorrect = false, OrderIndex = 2 }
        };

        context.AnswerOptions.AddRange(q3Options);
        await context.SaveChangesAsync();

        // Question 4: Multiple Choice
        var question4 = new Question
        {
            QuizId = quiz.Id,
            QuestionText = "What is the primary function of molars?",
            QuestionType = QuestionType.MultipleChoice,
            Explanation = "Molars are primarily used for grinding and crushing food.",
            Points = 1.0m,
            OrderIndex = 4,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Questions.Add(question4);
        await context.SaveChangesAsync();

        // Answer options for Question 4
        var q4Options = new List<AnswerOption>
        {
            new() { QuestionId = question4.Id, OptionText = "Cutting food", IsCorrect = false, OrderIndex = 1 },
            new() { QuestionId = question4.Id, OptionText = "Tearing food", IsCorrect = false, OrderIndex = 2 },
            new() { QuestionId = question4.Id, OptionText = "Grinding and crushing food", IsCorrect = true, OrderIndex = 3 },
            new() { QuestionId = question4.Id, OptionText = "Holding food", IsCorrect = false, OrderIndex = 4 }
        };

        context.AnswerOptions.AddRange(q4Options);
        await context.SaveChangesAsync();

        // Question 5: True/False
        var question5 = new Question
        {
            QuizId = quiz.Id,
            QuestionText = "Baby teeth are also called deciduous teeth.",
            QuestionType = QuestionType.TrueFalse,
            Explanation = "True! Baby teeth are scientifically referred to as deciduous teeth or primary teeth.",
            Points = 1.0m,
            OrderIndex = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Questions.Add(question5);
        await context.SaveChangesAsync();

        // Answer options for Question 5
        var q5Options = new List<AnswerOption>
        {
            new() { QuestionId = question5.Id, OptionText = "True", IsCorrect = true, OrderIndex = 1 },
            new() { QuestionId = question5.Id, OptionText = "False", IsCorrect = false, OrderIndex = 2 }
        };

        context.AnswerOptions.AddRange(q5Options);
        await context.SaveChangesAsync();
    }
}
