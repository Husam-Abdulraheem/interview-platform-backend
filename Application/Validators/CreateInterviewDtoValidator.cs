using FluentValidation;
using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Validators;

public class CreateInterviewDtoValidator : AbstractValidator<CreateInterviewDto>
{
    public CreateInterviewDtoValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
    }
}
