using PostManagement.Domain.Enums;

namespace PostManagement.Application.Contracts.Posts;

public class PostCreateRequest
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public PostCategory Category { get; set; }
}
