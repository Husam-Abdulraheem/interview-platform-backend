namespace InterviewPlatform.Core.Entities;

public class QuestionAttempt
{
    public Guid Id { get; set; }
    public Guid CourseAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string TraineeAnswer { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string GeneralFeedback { get; set; } = string.Empty;
    
    // We can store arrays as lists and EF Core with Npgsql will map them to text arrays
    public List<string> Strengths { get; set; } = new List<string>();
    public List<string> Weaknesses { get; set; } = new List<string>();
    public List<string> Suggestions { get; set; } = new List<string>();
    
    public DateTime CreatedAt { get; set; }

    public CourseAttempt? CourseAttempt { get; set; }
    public Question? Question { get; set; }
}
