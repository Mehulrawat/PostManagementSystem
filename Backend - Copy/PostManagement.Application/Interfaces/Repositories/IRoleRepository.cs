using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(UserRoleName name);
}
