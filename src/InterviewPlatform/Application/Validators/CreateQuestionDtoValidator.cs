using FluentValidation;
using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Validators;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.InterviewId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}
