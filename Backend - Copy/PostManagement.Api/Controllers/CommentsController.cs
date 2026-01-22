using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Contracts.Comments;
using PostManagement.Application.Services;

namespace PostManagement.Api.Controllers;

[ApiController]
[Route("api/Posts/{postId:guid}/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;

    public CommentsController(CommentService commentService)
    {
        _commentService = commentService;
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                  ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(Guid postId, CommentCreateRequest request)
    {
        var userId = GetCurrentUserId();
        await _commentService.AddCommentAsync(postId, userId, request.Content, request.ParentCommentId);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentResponse>>> GetComments(Guid postId)
    {
        var comments = await _commentService.GetCommentsForPostAsync(postId);
        return Ok(comments);
    }
}
