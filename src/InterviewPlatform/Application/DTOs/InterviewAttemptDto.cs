namespace InterviewPlatform.Application.DTOs;

public class InterviewAttemptDto
{
    public Guid Id { get; set; }
    public Guid TraineeId { get; set; }
    public Guid InterviewId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal TotalScore { get; set; }

    public ICollection<AnswerAttemptDto> AnswerAttempts { get; set; } = new List<AnswerAttemptDto>();
}
