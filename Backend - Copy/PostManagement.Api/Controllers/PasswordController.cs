using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Auth;
using PostManagement.Application.Services;
using PostManagement.Application.Interfaces.Notification;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using PostManagement.Application.Contracts.Password;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Application.Interfaces;
using PostManagement.Domain.Entities;
using PostManagement.Application.Interfaces.Security;

namespace PostManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController:ControllerBase

    {
        private readonly PasswordService _passwordService;
    
        private readonly UserManagementService _userManagementService;

        private readonly IUserRepository _userRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationService _notificationService;

        private readonly IPasswordHasher _passwordHasher;
          public PasswordController(PasswordService passwordService, UserManagementService userManagementService, 
              IUserRepository userRepository, IUnitOfWork unitOfWork,
              INotificationService notificationService,IPasswordHasher passwordHasher)
        {
            _passwordService = passwordService;
       
            _userManagementService = userManagementService;

            _userRepository = userRepository;

            _unitOfWork = unitOfWork;

            _notificationService = notificationService;

            _passwordHasher = passwordHasher;
 
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

        //[HttpPost("{userId:guid}/reset-password")]
        //[Authorize(Roles = "User,SuperAdmin")]
        //public async Task<IActionResult> ResetPassword(Guid userId)
        //{
        //    try
        //    {
        //        var actingUserId = GetCurrentUserId();
        //        await _userManagementService.ResetPasswordAsync(userId, actingUserId, AuthService.DefaultResetPassword);
        //        return NoContent();
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Forbid(ex.Message);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
           await _passwordService.ForgotPasswordAsync(request);



                return Ok(new { message = "If the email exists, a reset link has been sent." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {


            try
            {
                //var encodedToken = WebUtility.UrlEncode(request.Token);

                //var user = await _userRepository.GetByPasswordResetTokenAsync(encodedToken);
                var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token);
                if (user == null || user.PasswordResetTokenExpiresAt < DateTime.Now)
                    return BadRequest(new { message = "Invalid or expired reset link" });

                user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiresAt = null;

                await _unitOfWork.SaveChangesAsync();
                return Ok(new { message = "Password reset successfully." });
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
