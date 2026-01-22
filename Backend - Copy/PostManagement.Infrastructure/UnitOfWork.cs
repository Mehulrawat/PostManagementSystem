using PostManagement.Application.Interfaces;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly PostManagementDbContext _db;

    public UnitOfWork(PostManagementDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
