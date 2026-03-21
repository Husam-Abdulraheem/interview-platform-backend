using Mapster;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Application.DTOs;
using System.Reflection;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewPlatform.Application.Mappings;

public static class MappingConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Optionally, register custom mappings or scan assembly
        config.Scan(Assembly.GetExecutingAssembly());

        config.NewConfig<Course, CourseDto>();
        config.NewConfig<CreateCourseDto, Course>();
        config.NewConfig<UpdateCourseDto, Course>();
        
        config.NewConfig<RegisterDto, User>()
              .Map(dest => dest.PasswordHash, src => src.Password);
              
        config.NewConfig<User, UserProfileDto>();
        
        config.NewConfig<Interview, InterviewDto>();
        config.NewConfig<CreateInterviewDto, Interview>();
        config.NewConfig<UpdateInterviewDto, Interview>();

        config.NewConfig<Question, QuestionDto>();
        config.NewConfig<CreateQuestionDto, Question>();

        config.NewConfig<InterviewAttempt, InterviewAttemptDto>();
        config.NewConfig<AnswerAttempt, AnswerAttemptDto>();

        // Register mapper
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
