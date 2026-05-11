using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var token = await _authService.LoginAsync(request);
        if (token == null) return Unauthorized(ApiResponse<object>.Fail("Invalid email or password.", 401));
        return Ok(ApiResponse<TokenDto>.SuccessResult(token, "Login successful"));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        // Prevent clients from registering as Admin or SuperAdmin directly
        if (request.Role == InterviewPlatform.Core.Enums.Role.Admin || request.Role == InterviewPlatform.Core.Enums.Role.SuperAdmin)
            return BadRequest(ApiResponse<object>.Fail("Cannot register as an administrator."));

        var success = await _authService.RegisterAsync(request);
        if (!success) return BadRequest(ApiResponse<object>.Fail("Email is already in use."));
        return Ok(ApiResponse<object>.SuccessResult(null, "Registration successful. Please wait for approval if you requested a Creator role."));
    }
}
