using System;

namespace PostManagement.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }

    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
