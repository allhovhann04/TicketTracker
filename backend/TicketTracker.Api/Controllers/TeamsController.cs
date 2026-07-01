using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Contracts.Teams;
using TicketTracker.Api.Services.Teams;

namespace TicketTracker.Api.Controllers;

// No [AllowAnonymous] here: the global fallback authorization policy (Program.cs) requires
// an authenticated, verified user for every endpoint that doesn't opt out.
[ApiController]
[Route("api/teams")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
    {
        var result = await _teamService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var teams = await _teamService.ListAsync(cancellationToken);
        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _teamService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        var result = await _teamService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _teamService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return NoContent();
    }

    private IActionResult MapError(TeamResult result)
    {
        var statusCode = result.ErrorCode switch
        {
            TeamErrorCode.NotFound => StatusCodes.Status404NotFound,
            TeamErrorCode.InvalidName => StatusCodes.Status400BadRequest,
            TeamErrorCode.NameAlreadyExists => StatusCodes.Status409Conflict,
            TeamErrorCode.HasDependentEntities => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new { message = result.ErrorMessage });
    }
}
