namespace InterviewPlatform.Application.DTOs;

public class CreateQuestionDto
{
    public Guid InterviewId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}
