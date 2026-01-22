using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PostManagement.Application.Interfaces.Repositories;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;
using PostManagement.Infrastructure.Persistence;

namespace PostManagement.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly PostManagementDbContext _db;

    public PostRepository(PostManagementDbContext db)
    {
        _db = db;
    }

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _db.Posts
            .Include(p => p.CreatedByUser)
            .Include(p => p.AssignedToUser)
            .Include(p => p.StatusHistory)
            .ThenInclude(h => h.ChangedByUser)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Post post)
    {
        await _db.Posts.AddAsync(post);
    }

    public async Task<List<Post>> GetByUserAsync(Guid userId)
    {
        return await _db.Posts
            .Include(p => p.AssignedToUser)
            .Where(p => p.CreatedByUserId == userId)
            .ToListAsync();
    }

    public async Task<List<Post>> GetAllAsync(PostStatus? status = null)
    {
        var query = _db.Posts
            .Include(p => p.CreatedByUser)
            .Include(p => p.AssignedToUser)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Post>> GetAllByStatusesAsync(IEnumerable<PostStatus> statuses)
    {
        var statusSet = statuses.ToList();
        return await _db.Posts
            .Include(p => p.CreatedByUser)
            .Include(p => p.AssignedToUser)
            .Where(p => statusSet.Contains(p.Status))
            .ToListAsync();
    }
    public void AddStatusHistory(PostStatusHistory history)
    {
        _db.PostStatusHistories.Add(history);
    }


    public async Task DeleteAsync(Post post)
    {
        _db.Posts.Remove(post);
    }


}
