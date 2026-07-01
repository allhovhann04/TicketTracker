using TicketTracker.Api.Contracts.Comments;

namespace TicketTracker.Api.Services.Comments;

public interface ICommentService
{
    Task<CommentResult<CommentResponse>> CreateAsync(Guid ticketId, CreateCommentRequest request, Guid authorUserId, CancellationToken cancellationToken = default);
    Task<CommentResult<IReadOnlyList<CommentResponse>>> ListAsync(Guid ticketId, CancellationToken cancellationToken = default);
}
