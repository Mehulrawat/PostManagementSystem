using System;

namespace PostManagement.Application.Contracts.Users;

public class UserSummaryResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool IsAutoDeactivated { get; set; }
    public DateTime CreatedAt { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
