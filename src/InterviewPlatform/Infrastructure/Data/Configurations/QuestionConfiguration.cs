using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content).IsRequired().HasColumnType("text");
        builder.Property(e => e.OrderIndex).IsRequired();
    }
}
