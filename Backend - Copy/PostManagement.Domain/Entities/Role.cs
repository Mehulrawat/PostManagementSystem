using System.Collections.Generic;
using PostManagement.Domain.Enums;

namespace PostManagement.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public UserRoleName Name { get; set; }
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
