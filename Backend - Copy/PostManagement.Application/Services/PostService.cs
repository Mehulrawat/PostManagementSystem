using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostManagement.Application.Contracts.Posts;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Services;

public class PostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IPostRepository postRepository, IUserRepository userRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostResponse> CreateDraftAsync(PostCreateRequest request, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("User not found.");

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            Status = PostStatus.PendingApproval,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(post, user.Username, null);
    }

    public async Task SubmitForApprovalAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.CreatedByUserId != userId)
            throw new InvalidOperationException("You can only submit your own posts.");

        if (post.Status != PostStatus.Draft)
            throw new InvalidOperationException("Only posts in Draft state can be submitted for approval.");

        post.Status = PostStatus.PendingApproval;
        post.UpdatedAt = DateTime.UtcNow;

    
        var StatusHistory = new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = PostStatus.Draft,
            NewStatus = PostStatus.PendingApproval,
            ChangedByUserId = userId,
            ChangedAt = DateTime.UtcNow
        };

        _postRepository.AddStatusHistory(StatusHistory);

        await _unitOfWork.SaveChangesAsync();
    }

  


    public async Task<PostResponse?> GetByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null) return null;

        return MapToResponse(
            post,
            post.CreatedByUser.Username,
            post.AssignedToUser?.Username
        );
    }

    public async Task<List<PostResponse>> GetApprovedAsync()
    {
        var posts = await _postRepository.GetAllByStatusesAsync(new[] { PostStatus.Approved, PostStatus.Resolved });

        return posts.Select(p =>
                MapToResponse(p, p.CreatedByUser.Username, p.AssignedToUser?.Username))
            .ToList();
    }

    public async Task<List<PostResponse>> GetAllAsync(PostStatus? status)
    {
        var posts = await _postRepository.GetAllAsync(status);
        return posts.Select(p =>
                MapToResponse(p, p.CreatedByUser.Username, p.AssignedToUser?.Username))
            .ToList();
    }

    public async Task<List<PostResponse>> GetMyPostsAsync(Guid userId)
    {
        var posts = await _postRepository.GetByUserAsync(userId);
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("User not found.");

        return posts.Select(p => MapToResponse(p, user.Username, p.AssignedToUser?.Username)).ToList();
    }

    public async Task DeletePostAsync(Guid postId, bool isAdmin, Guid? userId = null)
    {
        var post = await _postRepository.GetByIdAsync(postId);

        if (post == null)
            throw new KeyNotFoundException("Post not found");


        if (!isAdmin && post.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own posts");


        if (post.Status == PostStatus.Rejected)
        {
            await _postRepository.DeleteAsync(post);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        await _postRepository.DeleteAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }



    #region Admin workflow

    public async Task ApprovePostAsync(Guid postId, Guid adminUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.Status != PostStatus.PendingApproval)
            throw new InvalidOperationException("Only posts in PendingApproval state can be approved.");

        await EnsureApprovalRightsAsync(post, adminUserId);

        post.Status = PostStatus.Approved;
        post.UpdatedAt = DateTime.UtcNow;
       var StatusHistory=new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = PostStatus.PendingApproval,
            NewStatus = PostStatus.Approved,
            ChangedByUserId = adminUserId,
            ChangedAt = DateTime.UtcNow
        };
        _postRepository.AddStatusHistory(StatusHistory);
        await _unitOfWork.SaveChangesAsync();

    }

    public async Task RejectPostAsync(Guid postId, Guid adminUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.Status != PostStatus.PendingApproval)
            throw new InvalidOperationException("Only posts in PendingApproval state can be rejected.");

        await EnsureApprovalRightsAsync(post, adminUserId);

        post.Status = PostStatus.Rejected;
        post.UpdatedAt = DateTime.UtcNow;
        var StatusHistory=new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = PostStatus.PendingApproval,
            NewStatus = PostStatus.Rejected,
            ChangedByUserId = adminUserId,
            ChangedAt = DateTime.UtcNow
        };
        _postRepository.AddStatusHistory(StatusHistory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ClosePostAsync(Guid postId, Guid UserId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.Status != PostStatus.Resolved && post.Status != PostStatus.Approved)
            throw new InvalidOperationException("Only approved or resolved posts can be closed.");

        var previousStatus = post.Status;
        post.Status = PostStatus.Closed;
        post.UpdatedAt = DateTime.UtcNow;
        var StatusHistory=new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = previousStatus,
            NewStatus = PostStatus.Closed,
            ChangedByUserId = UserId,
            ChangedAt = DateTime.UtcNow
        };
        _postRepository.AddStatusHistory(StatusHistory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ResolvePostAsync(Guid postId, Guid adminUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.Status != PostStatus.Approved)
            throw new InvalidOperationException("Only approved posts can be resolved.");

        post.Status = PostStatus.Resolved;
        post.UpdatedAt = DateTime.UtcNow;
        var StatusHistory=new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = PostStatus.Approved,
            NewStatus = PostStatus.Resolved,
            ChangedByUserId = adminUserId,
            ChangedAt = DateTime.UtcNow
        };
        _postRepository.AddStatusHistory(StatusHistory);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignPostAsync(Guid postId, Guid assigneeUserId, Guid actingUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId)
                   ?? throw new InvalidOperationException("Post not found.");

        if (post.Status == PostStatus.Closed)
            throw new InvalidOperationException("Cannot assign a closed post.");

        var assignee = await _userRepository.GetByIdAsync(assigneeUserId)
                       ?? throw new InvalidOperationException("Assignee user not found.");

        var assigneeRoles = await _userRoleRepository.GetRoleNamesForUserAsync(assignee.Id);
        var canOwn = assigneeRoles.Any(r =>
            string.Equals(r, UserRoleName.Admin.ToString(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(r, UserRoleName.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase));

        if (!canOwn)
            throw new InvalidOperationException("Posts can only be assigned to admin or superadmin users.");

        post.AssignedToUserId = assignee.Id;
        post.UpdatedAt = DateTime.UtcNow;

      var StatusHistory=new PostStatusHistory
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            OldStatus = post.Status,
            NewStatus = post.Status,
            ChangedByUserId = actingUserId,
            ChangedAt = DateTime.UtcNow,
            Remark = $"Assigned to {assignee.Username}"
        };
        _postRepository.AddStatusHistory(StatusHistory);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    private async Task EnsureApprovalRightsAsync(Post post, Guid actingUserId)
    {
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(actingUserId);
        var actingIsSuperAdmin = roles.Any(r =>
            string.Equals(r, UserRoleName.SuperAdmin.ToString(), StringComparison.OrdinalIgnoreCase));

        var actingIsAdmin = roles.Any(r =>
            string.Equals(r, UserRoleName.Admin.ToString(), StringComparison.OrdinalIgnoreCase));

        if (!actingIsAdmin && !actingIsSuperAdmin)
            throw new UnauthorizedAccessException("You do not have permission to approve posts.");

        // Posts created by admins require superadmin approval
        var createdByRoles = await _userRoleRepository.GetRoleNamesForUserAsync(post.CreatedByUserId);
        var createdByIsAdmin = createdByRoles.Any(r =>
            string.Equals(r, UserRoleName.Admin.ToString(), StringComparison.OrdinalIgnoreCase));

        if (createdByIsAdmin && !actingIsSuperAdmin)
            throw new UnauthorizedAccessException("Only the superadmin can approve admin posts.");
    }

    private static PostResponse MapToResponse(Post p, string createdBy, string? assignedTo)
    {
        return new PostResponse
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            Category = p.Category,
            Status = p.Status,
            CreatedBy = createdBy,
            AssignedTo = assignedTo,
            CreatedAt = p.CreatedAt
        };
    }
}
