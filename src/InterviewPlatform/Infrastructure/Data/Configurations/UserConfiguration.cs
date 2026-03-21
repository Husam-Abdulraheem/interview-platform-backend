using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.FullName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Role).IsRequired();

        // A user can create many courses (Creator)
        builder.HasMany(e => e.CreatedCourses)
               .WithOne(c => c.Creator)
               .HasForeignKey(c => c.CreatorId)
               .OnDelete(DeleteBehavior.Restrict);

        // A user can have many interview attempts (Trainee)
        builder.HasMany(e => e.InterviewAttempts)
               .WithOne(ia => ia.Trainee)
               .HasForeignKey(ia => ia.TraineeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
