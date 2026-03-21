using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.HasKey(e => e.Id);

        // Unique constraint on CourseId is handled by the 1-to-1 relationship defined in CourseConfiguration

        builder.Property(e => e.Title).IsRequired().HasMaxLength(255);
        builder.Property(e => e.PassingScore).IsRequired();

        builder.HasMany(e => e.Attempts)
               .WithOne(ia => ia.Interview)
               .HasForeignKey(ia => ia.InterviewId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Questions)
               .WithOne(q => q.Interview)
               .HasForeignKey(q => q.InterviewId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
