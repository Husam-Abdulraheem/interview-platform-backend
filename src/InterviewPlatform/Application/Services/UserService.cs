using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Enums;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with ID {userId} not found.");

        var dto = user.Adapt<UserProfileDto>();

        if (user.Role == Core.Enums.Role.Creator || user.Role == Core.Enums.Role.Admin)
        {
            var courses = await _unitOfWork.Courses.FindAsync(c => c.CreatorId == userId);
            dto.CreatedCoursesCount = courses.Count();
        }

        if (user.Role == Core.Enums.Role.Trainee)
        {
            // Simplified: No longer tracking interview attempts here
            dto.InterviewAttemptsCount = 0;
        }

        return dto;
    }

    public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return users.Adapt<IEnumerable<UserProfileDto>>();
    }

    public async Task<IEnumerable<UserProfileDto>> GetPendingRoleRequestsAsync()
    {
        var users = await _unitOfWork.Users.FindAsync(u => u.RequestedRole != null && !u.IsApproved);
        return users.Adapt<IEnumerable<UserProfileDto>>();
    }

    public async Task UpdateUserRoleAsync(Guid userId, Role newRole)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

        user.Role = newRole;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ApproveUserRoleRequestAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");
        if (user.RequestedRole == null) throw new InvalidOperationException("User has not requested a role change.");

        user.Role = user.RequestedRole.Value;
        user.RequestedRole = null;
        user.IsApproved = true;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
    }
}
