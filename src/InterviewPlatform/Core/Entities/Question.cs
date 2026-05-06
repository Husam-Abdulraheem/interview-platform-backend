namespace InterviewPlatform.Core.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid? CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public Course? Course { get; set; }
}
