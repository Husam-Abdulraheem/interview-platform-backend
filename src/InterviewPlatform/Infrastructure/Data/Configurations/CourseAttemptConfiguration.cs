using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class CourseAttemptConfiguration : IEntityTypeConfiguration<CourseAttempt>
{
    public void Configure(EntityTypeBuilder<CourseAttempt> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.StartedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(ca => ca.TotalScore)
            .HasPrecision(5, 2);

        builder.HasOne(ca => ca.Course)
            .WithMany()
            .HasForeignKey(ca => ca.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.User)
            .WithMany()
            .HasForeignKey(ca => ca.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
