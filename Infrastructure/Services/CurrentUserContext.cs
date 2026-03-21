using System.Security.Claims;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace InterviewPlatform.Infrastructure.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) 
        ? userId 
        : throw new UnauthorizedAccessException("User ID is missing from the token.");

    public Role Role 
    {
        get 
        {
            var roleString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            if (Enum.TryParse<Role>(roleString, out var role))
                return role;
            throw new UnauthorizedAccessException("Role is missing or invalid in the token.");
        }
    }

    public bool IsAdmin => Role == Role.Admin;
    public bool IsCreator => Role == Role.Creator;
}
