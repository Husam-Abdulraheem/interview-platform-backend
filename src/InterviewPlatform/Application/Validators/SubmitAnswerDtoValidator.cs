using FluentValidation;
using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Validators;

public class SubmitAnswerDtoValidator : AbstractValidator<SubmitAnswerDto>
{
    public SubmitAnswerDtoValidator()
    {
        RuleFor(x => x.InterviewAttemptId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.SubmittedText).NotEmpty();
    }
}
