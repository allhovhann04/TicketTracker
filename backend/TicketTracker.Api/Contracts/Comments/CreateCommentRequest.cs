using System.ComponentModel.DataAnnotations;

namespace TicketTracker.Api.Contracts.Comments;

public class CreateCommentRequest
{
    [Required]
    public required string Body { get; set; }
}
