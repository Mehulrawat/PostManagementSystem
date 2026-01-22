using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Enums;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly PostManagementDbContext _db;

    public UserRoleRepository(PostManagementDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(PostManagement.Domain.Entities.UserRole userRole)
    {
        await _db.UserRoles.AddAsync(userRole);
    }

    public async Task<List<string>> GetRoleNamesForUserAsync(Guid userId)
    {
        return await _db.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name.ToString())
            .ToListAsync();
    }

    public Task<bool> UserHasRoleAsync(Guid userId, UserRoleName roleName)
    {
        return _db.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
    }

    public Task<int> CountUsersInRoleAsync(UserRoleName roleName)
    {
        return _db.UserRoles
            .Include(ur => ur.Role)
            .CountAsync(ur => ur.Role.Name == roleName);
    }

    public async Task RemoveRoleAsync(Guid userId, UserRoleName roleName)
    {
        var match = await _db.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);

        if (match != null)
        {
            _db.UserRoles.Remove(match);
        }
    }
}
