using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Common;
using TicketTracker.Api.Contracts.Comments;
using TicketTracker.Api.Services.Comments;

namespace TicketTracker.Api.Controllers;

// No [AllowAnonymous] here: the global fallback authorization policy (Program.cs) requires
// an authenticated, verified user for every endpoint that doesn't opt out.
// Comments are immutable once created: only create and list are exposed, no update/delete.
[ApiController]
[Route("api/tickets/{ticketId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid ticketId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await _commentService.CreateAsync(ticketId, request, User.GetUserId(), cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return CreatedAtAction(nameof(List), new { ticketId }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(Guid ticketId, CancellationToken cancellationToken)
    {
        var result = await _commentService.ListAsync(ticketId, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    private IActionResult MapError(CommentResult result)
    {
        var statusCode = result.ErrorCode switch
        {
            CommentErrorCode.TicketNotFound => StatusCodes.Status404NotFound,
            CommentErrorCode.InvalidBody => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new { message = result.ErrorMessage });
    }
}
