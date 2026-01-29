using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Notification;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Services;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Net;
namespace PostManagement.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManagementService _userManagementService;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;


    public AuthController(AuthService authService, IUserRepository userRepository, 
        UserManagementService userManagementService, IUserRoleRepository userRoleRepository, 
        INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _userManagementService = userManagementService;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork; 
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

    //[HttpPost("test-email")]
    //public async Task<IActionResult> TestEmail(string email)
    //{
    //    // generate dummy token
    //    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    //    await _notificationService.SendEmailVerificationAsync(email, token);
    //    return Ok("Test email sent! Token: " + token);
    //}


    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        try
        {
            var encodedToken = WebUtility.UrlEncode(token);
            var user = await _userRepository
                .GetByEmailVerificationTokenAsync(encodedToken);

            if (user == null ||
                user.EmailVerificationTokenExpiresAt < DateTime.Now)
            {
                return BadRequest("Invalid or expired token");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiresAt = null;

            //await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok("Email verified successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpPost("resend-verification")]
    //public async Task<IActionResult> ResendVerificationEmail(ResendVerificationRequest request)
    //{
    //    var user = await _userRepository.GetByEmailAsync(request.Email);
    //    if (user == null)
    //        return BadRequest(new { message = "Email not found" });

    //    if (user.IsEmailVerified)
    //        return BadRequest(new { message = "Email is already verified" });

    //    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    //    user.EmailVerificationToken = token;
    //    user.EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);
    //    await _userRepository.UpdateAsync(user);

    //    var encodedToken = WebUtility.UrlEncode(token);
    //    await _emailNotificationService.SendEmailVerificationAsync(user.Email, encodedToken);

    //    return Ok(new { message = "Verification email resent successfully" });
  //  }



    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }
}
