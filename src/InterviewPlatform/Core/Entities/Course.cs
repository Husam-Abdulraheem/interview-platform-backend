namespace InterviewPlatform.Core.Entities;

public class Course
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsGeneral { get; set; }
    public string? Specialty { get; set; }
    public string YouTubeVideoUrl { get; set; } = string.Empty;
    public string ContentMaterial { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User Creator { get; set; } = null!;
    public Interview? Interview { get; set; }
}
