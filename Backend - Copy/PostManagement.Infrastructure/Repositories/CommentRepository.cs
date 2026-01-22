using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly PostManagementDbContext _db;

    public CommentRepository(PostManagementDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Comment comment)
    {
        await _db.Comments.AddAsync(comment);
    }

    public async Task<List<Comment>> GetByPostIdAsync(Guid postId)
    {
        return await _db.Comments
            .Include(c => c.CreatedByUser)
            .Where(c => c.PostId == postId)
            .ToListAsync();
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _db.Comments.AnyAsync(c => c.Id == id);
    }
}
