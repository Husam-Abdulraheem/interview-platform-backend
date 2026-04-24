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
        if (token == null) return Unauthorized("Invalid credentials.");
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        // Prevent clients from registering as Admin directly
        if (request.Role == InterviewPlatform.Core.Enums.Role.Admin)
            return BadRequest("Cannot register as admin. Admin accounts must be created by an existing admin.");

        var success = await _authService.RegisterAsync(request);
        if (!success) return BadRequest("Email is already in use.");
        return Ok("Registration successful.");
    }
}
