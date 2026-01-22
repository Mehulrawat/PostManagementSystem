using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Posts;
using PostManagement.Application.Services;
using PostManagement.Domain.Enums;

namespace PostManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }

    // ---------- User / public endpoints ----------

    // Create draft post (requires login)
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<PostResponse>> Create(PostCreateRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _postService.CreateDraftAsync(request, userId);
        return Ok(result);
    }

    // Submit draft for approval
    [HttpPost("{id:guid}/submit")]
    [Authorize]
    public async Task<IActionResult> SubmitForApproval(Guid id)
    {
        var userId = GetCurrentUserId();
        await _postService.SubmitForApprovalAsync(id, userId);
        //return NoContent(new {""};
        return Ok(new { message = "Post pending for approval" });
    }

    // Public: list approved posts
    [HttpGet]
    public async Task<ActionResult<List<PostResponse>>> GetApproved()
    {
        var result = await _postService.GetApprovedAsync();
        return Ok(result);
    }

    // Public: get a single post by id
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> GetById(Guid id)
    {
        var result = await _postService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // Logged-in user: own posts
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<PostResponse>>> MyPosts()
    {
        var userId = GetCurrentUserId();
        var result = await _postService.GetMyPostsAsync(userId);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

        await _postService.DeletePostAsync(id, isAdmin, userId);

        return NoContent();
    }

    // ---------- Admin endpoints ----------

    // Admin: list all posts (any status)
    [HttpGet("admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<List<PostResponse>>> GetAllForAdmin([FromQuery] PostStatus? status)
    {
        var result = await _postService.GetAllAsync(status);
        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var adminId = GetCurrentUserId();
        await _postService.ApprovePostAsync(id, adminId);
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var adminId = GetCurrentUserId();
        await _postService.RejectPostAsync(id, adminId);
        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    [Authorize]
    public async Task<IActionResult> Close(Guid id)
    {
        var userId = GetCurrentUserId();
        await _postService.ClosePostAsync(id, userId);
        return NoContent();
    }


    [HttpPost("{id:guid}/resolve")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var adminId = GetCurrentUserId();
        await _postService.ResolvePostAsync(id, adminId);
        return NoContent();
    }

    [HttpPost("{id:guid}/assign/{assigneeId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Assign(Guid id, Guid assigneeId)
    {
        var adminId = GetCurrentUserId();
        await _postService.AssignPostAsync(id, assigneeId, adminId);
        return NoContent();
    }
}
