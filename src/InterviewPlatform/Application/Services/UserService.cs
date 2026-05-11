using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Enums;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public UserService(IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
    {
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
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
        var users = await _unitOfWork.Users.FindAsync(u => 
            (u.RequestedRole != null && !u.IsApproved) || 
            (u.Role == Core.Enums.Role.Creator && !u.IsApproved));
        return users.Adapt<IEnumerable<UserProfileDto>>();
    }

    public async Task UpdateUserRoleAsync(Guid userId, Role newRole)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

        // Protection: Only SuperAdmin can modify SuperAdmin or Admin roles.
        if ((user.Role == Core.Enums.Role.Admin || user.Role == Core.Enums.Role.SuperAdmin) && !_currentUserContext.IsSuperAdmin)
            throw new ForbiddenException("Only a SuperAdmin can modify the role of an Admin or SuperAdmin.");

        // Protection: Prevent demoting the last Admin/SuperAdmin
        if ((user.Role == Core.Enums.Role.Admin || user.Role == Core.Enums.Role.SuperAdmin) && newRole != Core.Enums.Role.Admin && newRole != Core.Enums.Role.SuperAdmin)
        {
            var adminCount = (await _unitOfWork.Users.FindAsync(u => u.Role == Core.Enums.Role.Admin || u.Role == Core.Enums.Role.SuperAdmin)).Count();
            if (adminCount <= 1)
                throw new InvalidOperationException("Cannot demote the last administrator in the system.");
        }

        user.Role = newRole;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");

        // Protection: Only SuperAdmin can delete an Admin or SuperAdmin
        if ((user.Role == Core.Enums.Role.Admin || user.Role == Core.Enums.Role.SuperAdmin) && !_currentUserContext.IsSuperAdmin)
            throw new ForbiddenException("Only a SuperAdmin can delete an Admin or SuperAdmin.");

        // Protection: Prevent deleting the last Admin/SuperAdmin
        if (user.Role == Core.Enums.Role.Admin || user.Role == Core.Enums.Role.SuperAdmin)
        {
            var adminCount = (await _unitOfWork.Users.FindAsync(u => u.Role == Core.Enums.Role.Admin || u.Role == Core.Enums.Role.SuperAdmin)).Count();
            if (adminCount <= 1)
                throw new InvalidOperationException("Cannot delete the last administrator in the system.");
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ApproveUserRoleRequestAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found.");
        
        if (user.RequestedRole == null && user.Role != Core.Enums.Role.Creator) 
            throw new InvalidOperationException("User has not requested a role change and is not a pending creator.");

        if (user.RequestedRole != null)
        {
            user.Role = user.RequestedRole.Value;
            user.RequestedRole = null;
        }
        
        user.IsApproved = true;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
    }
}
