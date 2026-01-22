using System;
using System.Collections.Generic;

namespace PostManagement.Application.Contracts.Comments;

public class CommentResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public Guid? ParentCommentId { get; set; }
    public List<CommentResponse> Replies { get; set; } = new();
}
