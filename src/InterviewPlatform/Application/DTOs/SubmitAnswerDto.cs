namespace InterviewPlatform.Application.DTOs;

public class SubmitAnswerDto
{
    public Guid InterviewAttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public string SubmittedText { get; set; } = string.Empty;
}
