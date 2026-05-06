using InterviewPlatform.Core.Entities;

namespace InterviewPlatform.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Course> Courses { get; }
    IRepository<Question> Questions { get; }

    Task<int> CompleteAsync();
}
