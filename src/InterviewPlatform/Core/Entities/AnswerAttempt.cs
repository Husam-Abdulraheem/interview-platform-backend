using System.Text.Json;

namespace InterviewPlatform.Core.Entities;

public class AnswerAttempt
{
    public Guid Id { get; set; }
    public Guid InterviewAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string SubmittedText { get; set; } = string.Empty;
    public decimal AiScore { get; set; }
    
    // Store arrays as JSON in database
    public string AiStrengthsJson { get; set; } = string.Empty;
    public string AiWeaknessesJson { get; set; } = string.Empty;
    public string AiSuggestionsJson { get; set; } = string.Empty;

    // Computed properties for easier access
    public List<string> AiStrengths 
    { 
        get => string.IsNullOrEmpty(AiStrengthsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(AiStrengthsJson) ?? new List<string>();
        set => AiStrengthsJson = JsonSerializer.Serialize(value);
    }
    
    public List<string> AiWeaknesses 
    { 
        get => string.IsNullOrEmpty(AiWeaknessesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(AiWeaknessesJson) ?? new List<string>();
        set => AiWeaknessesJson = JsonSerializer.Serialize(value);
    }
    
    public List<string> AiSuggestions 
    { 
        get => string.IsNullOrEmpty(AiSuggestionsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(AiSuggestionsJson) ?? new List<string>();
        set => AiSuggestionsJson = JsonSerializer.Serialize(value);
    }

    public InterviewAttempt InterviewAttempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
