using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Contracts.Epics;
using TicketTracker.Api.Services.Epics;

namespace TicketTracker.Api.Controllers;

// No [AllowAnonymous] here: the global fallback authorization policy (Program.cs) requires
// an authenticated, verified user for every endpoint that doesn't opt out.
[ApiController]
[Route("api/epics")]
public class EpicsController : ControllerBase
{
    private readonly IEpicService _epicService;

    public EpicsController(IEpicService epicService)
    {
        _epicService = epicService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpicRequest request, CancellationToken cancellationToken)
    {
        var result = await _epicService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? teamId, CancellationToken cancellationToken)
    {
        var epics = await _epicService.ListAsync(teamId, cancellationToken);
        return Ok(epics);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _epicService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEpicRequest request, CancellationToken cancellationToken)
    {
        var result = await _epicService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _epicService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return NoContent();
    }

    private IActionResult MapError(EpicResult result)
    {
        var statusCode = result.ErrorCode switch
        {
            EpicErrorCode.NotFound => StatusCodes.Status404NotFound,
            EpicErrorCode.TeamNotFound => StatusCodes.Status400BadRequest,
            EpicErrorCode.InvalidTitle => StatusCodes.Status400BadRequest,
            EpicErrorCode.HasDependentTickets => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new { message = result.ErrorMessage });
    }
}
