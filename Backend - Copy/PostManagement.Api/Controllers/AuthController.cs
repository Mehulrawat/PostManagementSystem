using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Services;

using System.Security.Claims;

namespace PostManagement.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManagementService _userManagementService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;


    public AuthController(AuthService authService, IUserRepository userRepository, UserManagementService userManagementService, IUserRoleRepository userRoleRepository)
    {
        _authService = authService;
        _userManagementService = userManagementService;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("basic-login")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<LoginResponse>> BasicLogin()
    {
        var userId = GetCurrentUserId();
        var response = await _authService.IssueTokenForUserAsync(userId);
        return Ok(response);
    }







    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }
}
