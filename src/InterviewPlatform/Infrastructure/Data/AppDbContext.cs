using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Text.Json;

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

        // Configure List<string> to JSON conversion for AnswerAttempt
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
        );

        modelBuilder.Entity<AnswerAttempt>()
            .Property(a => a.AiStrengths)
            .HasConversion(stringListConverter);

        modelBuilder.Entity<AnswerAttempt>()
            .Property(a => a.AiWeaknesses)
            .HasConversion(stringListConverter);

        modelBuilder.Entity<AnswerAttempt>()
            .Property(a => a.AiSuggestions)
            .HasConversion(stringListConverter);

        // Configure Course-Question relationship
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Course)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
