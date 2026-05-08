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
        config.NewConfig<CreateCourseDto, Course>()
              .Ignore(dest => dest.Questions);

        config.NewConfig<UpdateCourseDto, Course>()
              .Ignore(dest => dest.Questions);
        
        config.NewConfig<RegisterDto, User>()
              .Map(dest => dest.PasswordHash, src => src.Password);
              
        config.NewConfig<User, UserProfileDto>();
        
        config.NewConfig<Question, QuestionDto>();
        config.NewConfig<CreateQuestionDto, Question>();

        // Register mapper
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
