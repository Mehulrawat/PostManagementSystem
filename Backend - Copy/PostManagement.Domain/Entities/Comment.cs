using System;
using System.Collections.Generic;

namespace PostManagement.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = default!;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; } = default!;
    public User CreatedByUser { get; set; } = default!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
