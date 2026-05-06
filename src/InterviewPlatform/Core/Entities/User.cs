using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Trainee;
    // Role upgrade request workflow
    public Role? RequestedRole { get; set; }
    public bool IsApproved { get; set; } = true;

    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
}
