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
    public DbSet<Question> Questions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure Course-Question relationship
        modelBuilder.Entity<Question>()
            .HasOne(q => q.Course)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
