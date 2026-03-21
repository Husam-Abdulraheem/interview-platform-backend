using FluentValidation;
using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Validators;

public class UpdateInterviewDtoValidator : AbstractValidator<UpdateInterviewDto>
{
    public UpdateInterviewDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
    }
}
