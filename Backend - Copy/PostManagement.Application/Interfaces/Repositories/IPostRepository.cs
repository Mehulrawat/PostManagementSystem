using System.Collections.Generic;
using PostManagement.Domain.Entities;
using PostManagement.Domain.Enums;

namespace PostManagement.Application.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task AddAsync(Post post);
    Task<List<Post>> GetByUserAsync(Guid userId);
    Task<List<Post>> GetAllAsync(PostStatus? status = null);
    Task<List<Post>> GetAllByStatusesAsync(IEnumerable<PostStatus> statuses);
    void AddStatusHistory(PostStatusHistory history);
    Task DeleteAsync(Post post);

}
