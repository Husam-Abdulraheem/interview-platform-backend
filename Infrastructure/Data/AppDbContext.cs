using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InterviewPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Interview> Interviews { get; set; } = null!;
    public DbSet<InterviewAttempt> InterviewAttempts { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<AnswerAttempt> AnswerAttempts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
