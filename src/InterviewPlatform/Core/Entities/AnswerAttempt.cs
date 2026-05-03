namespace InterviewPlatform.Core.Entities;

public class AnswerAttempt
{
    public Guid Id { get; set; }
    public Guid InterviewAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string SubmittedText { get; set; } = string.Empty;
    public decimal AiScore { get; set; }
    public List<string> AiStrengths { get; set; } = new List<string>();
    public List<string> AiWeaknesses { get; set; } = new List<string>();
    public List<string> AiSuggestions { get; set; } = new List<string>();

    public InterviewAttempt InterviewAttempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
}
