using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly PostManagementDbContext _db;

    public RoleRepository(PostManagementDbContext db)
    {
        _db = db;
    }

    public Task<Role?> GetByNameAsync(UserRoleName name)
    {
        return _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
