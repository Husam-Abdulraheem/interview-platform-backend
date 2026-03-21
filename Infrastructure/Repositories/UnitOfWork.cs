using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using InterviewPlatform.Infrastructure.Data;

namespace InterviewPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<User> Users { get; private set; }
    public IRepository<Course> Courses { get; private set; }
    public IRepository<Interview> Interviews { get; private set; }
    public IRepository<InterviewAttempt> InterviewAttempts { get; private set; }
    public IRepository<Question> Questions { get; private set; }
    public IRepository<AnswerAttempt> AnswerAttempts { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new Repository<User>(_context);
        Courses = new Repository<Course>(_context);
        Interviews = new Repository<Interview>(_context);
        InterviewAttempts = new Repository<InterviewAttempt>(_context);
        Questions = new Repository<Question>(_context);
        AnswerAttempts = new Repository<AnswerAttempt>(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
