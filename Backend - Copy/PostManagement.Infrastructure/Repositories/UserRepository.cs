using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostManagementDbContext _db;
    private readonly IUnitOfWork _unitOfWork;


    public UserRepository(PostManagementDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
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
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
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

    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }
    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await _db.Users
           .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }

    //public async Task UpdateAsync(User user)
    //{
    //    await _unitOfWork.SaveChangesAsync();
    //    //_db.Users.Update(user);
    //    //await _db.SaveChangesAsync();
    //}

}
