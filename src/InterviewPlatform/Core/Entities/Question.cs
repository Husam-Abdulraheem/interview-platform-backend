namespace InterviewPlatform.Core.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public Interview Interview { get; set; } = null!;
    public ICollection<AnswerAttempt> AnswerAttempts { get; set; } = new List<AnswerAttempt>();
}
