using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostManagementDbContext _db;

    public UserRepository(PostManagementDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        return _db.Users.AnyAsync(u => u.Username == username);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        return _db.Users.AnyAsync(u => u.Email == email);
    }

    public Task<List<User>> GetAllAsync()
    {
        return _db.Users.AsNoTracking().ToListAsync();
    }

    public Task DeleteAsync(User user)
    {
        _db.Users.Remove(user);
        return Task.CompletedTask;
    }
}
