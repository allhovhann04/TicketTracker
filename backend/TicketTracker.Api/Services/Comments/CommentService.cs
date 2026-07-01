using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Contracts.Comments;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Services.Comments;

public class CommentService : ICommentService
{
    private readonly AppDbContext _db;

    public CommentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CommentResult<CommentResponse>> CreateAsync(Guid ticketId, CreateCommentRequest request, Guid authorUserId, CancellationToken cancellationToken = default)
    {
        // A read-only existence check only - it does not load or track the Ticket entity,
        // so adding a comment can never inadvertently touch Ticket.UpdatedAt.
        var ticketExists = await _db.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
        if (!ticketExists)
        {
            return CommentResult<CommentResponse>.Fail(CommentErrorCode.TicketNotFound, "Ticket not found.");
        }

        var body = request.Body.Trim();
        if (body.Length == 0)
        {
            return CommentResult<CommentResponse>.Fail(CommentErrorCode.InvalidBody, "Comment body must not be empty.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            AuthorUserId = authorUserId,
            Content = body,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);

        return CommentResult<CommentResponse>.Ok(ToResponse(comment));
    }

    public async Task<CommentResult<IReadOnlyList<CommentResponse>>> ListAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var ticketExists = await _db.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
        if (!ticketExists)
        {
            return CommentResult<IReadOnlyList<CommentResponse>>.Fail(CommentErrorCode.TicketNotFound, "Ticket not found.");
        }

        var comments = await _db.Comments
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                TicketId = c.TicketId,
                AuthorUserId = c.AuthorUserId,
                Body = c.Content,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return CommentResult<IReadOnlyList<CommentResponse>>.Ok(comments);
    }

    private static CommentResponse ToResponse(Comment comment) => new()
    {
        Id = comment.Id,
        TicketId = comment.TicketId,
        AuthorUserId = comment.AuthorUserId,
        Body = comment.Content,
        CreatedAt = comment.CreatedAt
    };
}
