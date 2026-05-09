namespace InterviewPlatform.Core.Entities;

public class CourseAttempt
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal TotalScore { get; set; }

    public Course? Course { get; set; }
    public User? User { get; set; }
    public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
