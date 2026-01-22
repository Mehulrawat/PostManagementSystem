using PostManagement.Domain.Enums;

namespace PostManagement.Application.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task AddAsync(PostManagement.Domain.Entities.UserRole userRole);
    Task<List<string>> GetRoleNamesForUserAsync(Guid userId);
    Task<bool> UserHasRoleAsync(Guid userId, UserRoleName roleName);
    Task<int> CountUsersInRoleAsync(UserRoleName roleName);
    Task RemoveRoleAsync(Guid userId, UserRoleName roleName);
}
