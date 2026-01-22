using PostManagement.Domain.Entities;

namespace PostManagement.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task DeleteAsync(User user);
}
