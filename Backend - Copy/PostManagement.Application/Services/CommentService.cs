using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostManagement.Application.Contracts.Comments;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Services;

public class CommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
       _postRepository = postRepository;
    }

    public async Task AddCommentAsync(Guid postId, Guid userId, string content, Guid? parentCommentId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("User not found.");
        if (parentCommentId != null)
        {
            var parentExists = await _commentRepository
                .ExistsAsync(parentCommentId.Value);

            if (!parentExists)
                throw new Exception("Parent comment does not exist.");
        }
        var post = await _postRepository.GetByIdAsync(postId);
        if (post.Status != PostStatus.Approved)
        {
            throw new Exception("Comments are disabled. The post isn't approved yet.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            ParentCommentId = parentCommentId,
            Content = content,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<CommentResponse>> GetCommentsForPostAsync(Guid postId)
    {
        var comments = await _commentRepository.GetByPostIdAsync(postId);

        var responseLookup = comments.ToDictionary(
            c => c.Id,
            c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedByUser.Username,
                ParentCommentId = c.ParentCommentId
            });

        List<CommentResponse> roots = new();
        foreach (var c in comments)
        {
            var resp = responseLookup[c.Id];
            if (c.ParentCommentId.HasValue && responseLookup.ContainsKey(c.ParentCommentId.Value))
            {
                responseLookup[c.ParentCommentId.Value].Replies.Add(resp);
            }
            else
            {
                roots.Add(resp);
            }
        }

        return roots;
    }
}
