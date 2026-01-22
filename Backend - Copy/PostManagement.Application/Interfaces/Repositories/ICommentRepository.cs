using PostManagement.Domain.Entities;

namespace PostManagement.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task AddAsync(Comment comment);
    Task<List<Comment>> GetByPostIdAsync(Guid postId);
    Task<bool> ExistsAsync(Guid id);
}
