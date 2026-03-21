using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }

    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<InterviewAttempt> InterviewAttempts { get; set; } = new List<InterviewAttempt>();
}
