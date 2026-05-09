namespace InterviewPlatform.Application.DTOs;

public class SubmitAnswerDto
{
    public Guid QuestionId { get; set; }
    public string Answer { get; set; } = string.Empty;
}
