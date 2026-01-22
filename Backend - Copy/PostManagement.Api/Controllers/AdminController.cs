using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Users;
using PostManagement.Application.Services;

namespace PostManagement.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly UserManagementService _userManagementService;

    public AdminController(UserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserSummaryResponse>>> GetUsers()
    {
        var result = await _userManagementService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/suspend")]
    public async Task<IActionResult> SuspendUser(Guid userId)
    {
        await _userManagementService.SuspendUserAsync(userId, GetCurrentUserId());
        return NoContent();
    }

    
    //To bring back the suspended users
    [HttpPost("users/{userId:guid}/reinstate")]
    public async Task<IActionResult> ReinstateUser(Guid userId)
    {
        await _userManagementService.ReinstateUserAsync(userId, GetCurrentUserId());
        return NoContent();
    }
    [HttpDelete("users/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _userManagementService.DeleteUserAsync(userId, GetCurrentUserId());
        return NoContent();
    }

    [HttpPost("users/{userId:guid}/promote")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> PromoteToAdmin(Guid userId)
    {
        await _userManagementService.PromoteToAdminAsync(userId, GetCurrentUserId());
        return NoContent();
    }

    [HttpPost("users/{userId:guid}/demote")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DemoteAdmin(Guid userId)
    {
        await _userManagementService.DemoteAdminAsync(userId, GetCurrentUserId());
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }
}
