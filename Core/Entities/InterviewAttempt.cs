namespace InterviewPlatform.Core.Entities;

public class InterviewAttempt
{
    public Guid Id { get; set; }
    public Guid TraineeId { get; set; }
    public Guid InterviewId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal TotalScore { get; set; }

    public User Trainee { get; set; } = null!;
    public Interview Interview { get; set; } = null!;
    public ICollection<AnswerAttempt> AnswerAttempts { get; set; } = new List<AnswerAttempt>();
}
