namespace InterviewPlatform.Application.DTOs;

public class CourseAttemptDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal TotalScore { get; set; }
    
    public CourseDto? Course { get; set; }
    public List<QuestionAttemptDto> QuestionAttempts { get; set; } = new List<QuestionAttemptDto>();
}
