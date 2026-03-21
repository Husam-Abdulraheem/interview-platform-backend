using InterviewPlatform.Core.Entities;

namespace InterviewPlatform.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Course> Courses { get; }
    IRepository<Interview> Interviews { get; }
    IRepository<InterviewAttempt> InterviewAttempts { get; }
    IRepository<Question> Questions { get; }
    IRepository<AnswerAttempt> AnswerAttempts { get; }

    Task<int> CompleteAsync();
}
