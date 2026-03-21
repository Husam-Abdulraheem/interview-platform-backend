using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; }
    
    // Additional Optional Stats
    public int CreatedCoursesCount { get; set; }
    public int InterviewAttemptsCount { get; set; }
}
