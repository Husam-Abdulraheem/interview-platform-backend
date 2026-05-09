using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class QuestionAttemptConfiguration : IEntityTypeConfiguration<QuestionAttempt>
{
    public void Configure(EntityTypeBuilder<QuestionAttempt> builder)
    {
        builder.HasKey(qa => qa.Id);

        builder.Property(qa => qa.TraineeAnswer)
            .IsRequired();

        builder.Property(qa => qa.Score)
            .HasPrecision(5, 2);

        builder.Property(qa => qa.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(qa => qa.CourseAttempt)
            .WithMany(ca => ca.QuestionAttempts)
            .HasForeignKey(qa => qa.CourseAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qa => qa.Question)
            .WithMany()
            .HasForeignKey(qa => qa.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
