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

        builder.HasMany(e => e.AnswerAttempts)
               .WithOne(aa => aa.Question)
               .HasForeignKey(aa => aa.QuestionId)
               .OnDelete(DeleteBehavior.Restrict); // Don't delete answers if question is deleted, or maybe Cascade. Restrict is safer for historical answers.
    }
}
