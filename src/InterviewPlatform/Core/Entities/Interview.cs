namespace InterviewPlatform.Core.Entities;

public class Interview
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<InterviewAttempt> Attempts { get; set; } = new List<InterviewAttempt>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
