using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class AnswerAttemptConfiguration : IEntityTypeConfiguration<AnswerAttempt>
{
    public void Configure(EntityTypeBuilder<AnswerAttempt> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.SubmittedText).HasColumnType("text");
        builder.Property(e => e.AiScore).HasPrecision(5, 2);
        builder.Property(e => e.AiStrengths).HasColumnType("text");
        builder.Property(e => e.AiWeaknesses).HasColumnType("text");
    }
}
