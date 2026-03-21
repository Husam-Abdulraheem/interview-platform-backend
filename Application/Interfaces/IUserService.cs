using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Core.Enums;

namespace InterviewPlatform.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
    Task UpdateUserRoleAsync(Guid userId, Role newRole);
    Task DeleteUserAsync(Guid userId);
}
