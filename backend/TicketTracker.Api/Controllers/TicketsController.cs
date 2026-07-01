using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Common;
using TicketTracker.Api.Contracts.Tickets;
using TicketTracker.Api.Entities.Enums;
using TicketTracker.Api.Services.Tickets;

namespace TicketTracker.Api.Controllers;

// No [AllowAnonymous] here: the global fallback authorization policy (Program.cs) requires
// an authenticated, verified user for every endpoint that doesn't opt out.
[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.CreateAsync(request, User.GetUserId(), cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? teamId,
        [FromQuery] string? type,
        [FromQuery] Guid? epicId,
        [FromQuery] string? state,
        [FromQuery] string? titleSearch,
        CancellationToken cancellationToken)
    {
        TicketType? parsedType = null;
        if (!string.IsNullOrEmpty(type))
        {
            if (!TicketTypeMapper.TryParse(type, out var typeValue))
            {
                return BadRequest(new { message = $"'{type}' is not a valid ticket type." });
            }

            parsedType = typeValue;
        }

        TicketState? parsedState = null;
        if (!string.IsNullOrEmpty(state))
        {
            if (!TicketStateMapper.TryParse(state, out var stateValue))
            {
                return BadRequest(new { message = $"'{state}' is not a valid ticket state." });
            }

            parsedState = stateValue;
        }

        var filter = new TicketListFilter(teamId, parsedType, epicId, parsedState, titleSearch);
        var tickets = await _ticketService.ListAsync(filter, cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ticketService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        var result = await _ticketService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ticketService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return NoContent();
    }

    private IActionResult MapError(TicketResult result)
    {
        var statusCode = result.ErrorCode switch
        {
            TicketErrorCode.NotFound => StatusCodes.Status404NotFound,
            TicketErrorCode.TeamNotFound => StatusCodes.Status400BadRequest,
            TicketErrorCode.EpicNotFound => StatusCodes.Status400BadRequest,
            TicketErrorCode.EpicNotInTeam => StatusCodes.Status400BadRequest,
            TicketErrorCode.InvalidTitle => StatusCodes.Status400BadRequest,
            TicketErrorCode.InvalidBody => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new { message = result.ErrorMessage });
    }
}
