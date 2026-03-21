using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class InterviewAttemptConfiguration : IEntityTypeConfiguration<InterviewAttempt>
{
    public void Configure(EntityTypeBuilder<InterviewAttempt> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.StartedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(e => e.TotalScore).HasPrecision(5, 2); // E.g. 100.00

        builder.HasMany(e => e.AnswerAttempts)
               .WithOne(aa => aa.InterviewAttempt)
               .HasForeignKey(aa => aa.InterviewAttemptId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
