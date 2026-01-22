using System;
using PostManagement.Domain.Enums;

namespace PostManagement.Domain.Entities;

public class PostStatusHistory
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public PostStatus OldStatus { get; set; }
    public PostStatus NewStatus { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }

    public Post Post { get; set; } = default!;
    public User ChangedByUser { get; set; } = default!;
}
