using InterviewPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewPlatform.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Specialty).HasMaxLength(255);
        builder.Property(e => e.YouTubeVideoUrl).HasMaxLength(1000);
        builder.Property(e => e.ContentMaterial).HasMaxLength(4000); // Could be text or URL
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // 1-to-1 or 1-to-0..1 relationship with Interview
        builder.HasOne(e => e.Interview)
               .WithOne(i => i.Course)
               .HasForeignKey<Interview>(i => i.CourseId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
