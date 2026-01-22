using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Services;
using System.Security.Claims;

namespace PostManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController:ControllerBase

    {
        private readonly PasswordService _passwordService;
    
        private readonly UserManagementService _userManagementService;
          public PasswordController(PasswordService passwordService, UserManagementService userManagementService)
        {
            _passwordService = passwordService;
       
            _userManagementService = userManagementService;
        }


        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _passwordService.ChangePasswordAsync(userId, request);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{userId:guid}/reset-password")]
        [Authorize(Roles = "User,SuperAdmin")]
        public async Task<IActionResult> ResetPassword(Guid userId)
        {
            try
            {
                var actingUserId = GetCurrentUserId();
                await _userManagementService.ResetPasswordAsync(userId, actingUserId, AuthService.DefaultResetPassword);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
            return Guid.Parse(sub!);
        }
    }
}
