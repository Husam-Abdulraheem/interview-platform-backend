using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Application.DTOs;

public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role? Role { get; set; }
    
    // Creator fields
    public string? Bio { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public int? ExperienceYears { get; set; }
}
