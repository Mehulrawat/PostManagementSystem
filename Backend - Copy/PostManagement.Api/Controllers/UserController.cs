using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Services;
using System.Security.Claims;

namespace PostManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
     
     

      
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            try
            {
               Guid userId = GetCurrentUserId();
                 var user=await _userService.MeAsync(userId);
                return Ok(user);
            }
            catch( UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
