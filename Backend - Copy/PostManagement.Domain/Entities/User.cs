using PostManagement.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PostManagement.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
  
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastActivityAt { get; set; }
    public bool IsAutoDeactivated { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Post> AssignedPosts { get; set; } = new List<Post>();

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
