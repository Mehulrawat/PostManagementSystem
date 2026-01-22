using System;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Contracts.Posts;

public class PostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public PostCategory Category { get; set; }
    public PostStatus Status { get; set; }
    public string CreatedBy { get; set; } = default!;
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
}
