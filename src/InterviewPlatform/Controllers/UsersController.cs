using System.Security.Claims;
using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    [HttpGet("profile")]
    [Authorize] // All users can access their own profile
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User identity not found in token.");

        var profile = await _userService.GetUserProfileAsync(userId);
        return Ok(ApiResponse<UserProfileDto>.SuccessResult(profile));
    }

    // --- Admin Only Features ---

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<IEnumerable<UserProfileDto>>.SuccessResult(users));
    }

    [HttpGet("requests")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingRoleRequests()
    {
        var pending = await _userService.GetPendingRoleRequestsAsync();
        return Ok(ApiResponse<IEnumerable<UserProfileDto>>.SuccessResult(pending));
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRoleRequest(Guid id)
    {
        await _userService.ApproveUserRoleRequestAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "Role request approved successfully"));
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromQuery] Role newRole)
    {
        if (!Enum.IsDefined(typeof(Role), newRole))
            return BadRequest(ApiResponse<object>.Fail("Invalid Role specified."));

        await _userService.UpdateUserRoleAsync(id, newRole);
        return Ok(ApiResponse<object>.SuccessResult(null, "User role updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "User deleted successfully"));
    }
}
