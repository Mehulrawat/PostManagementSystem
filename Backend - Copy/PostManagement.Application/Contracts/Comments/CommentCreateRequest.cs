namespace PostManagement.Application.Contracts.Comments;

public class CommentCreateRequest
{
    public string Content { get; set; } = default!;
    public Guid? ParentCommentId { get; set; }
}
