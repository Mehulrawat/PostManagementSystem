using System;
using System.Collections.Generic;
using PostManagement.Domain.Enums;

namespace PostManagement.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public PostCategory Category { get; set; }
    public PostStatus Status { get; set; } = PostStatus.PendingApproval;

    public Guid CreatedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User CreatedByUser { get; set; } = default!;
    public User? AssignedToUser { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostStatusHistory> StatusHistory { get; set; } = new List<PostStatusHistory>();
}
