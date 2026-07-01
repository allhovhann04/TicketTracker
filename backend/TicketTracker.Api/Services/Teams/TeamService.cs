using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Contracts.Teams;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Services.Teams;

public class TeamService : ITeamService
{
    private readonly AppDbContext _db;

    public TeamService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TeamResult<TeamResponse>> CreateAsync(CreateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        if (name.Length == 0)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.InvalidName, "Team name must not be empty.");
        }

        var nameExists = await _db.Teams.AnyAsync(t => t.Name == name, cancellationToken);
        if (nameExists)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.NameAlreadyExists, "A team with this name already exists.");
        }

        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync(cancellationToken);

        return TeamResult<TeamResponse>.Ok(ToResponse(team));
    }

    public async Task<IReadOnlyList<TeamResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Teams
            .OrderBy(t => t.Name)
            .Select(t => new TeamResponse
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TeamResult<TeamResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _db.Teams.FindAsync([id], cancellationToken);
        if (team is null)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.NotFound, "Team not found.");
        }

        return TeamResult<TeamResponse>.Ok(ToResponse(team));
    }

    public async Task<TeamResult<TeamResponse>> UpdateAsync(Guid id, UpdateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var team = await _db.Teams.FindAsync([id], cancellationToken);
        if (team is null)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.NotFound, "Team not found.");
        }

        var name = request.Name.Trim();
        if (name.Length == 0)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.InvalidName, "Team name must not be empty.");
        }

        var nameExists = await _db.Teams.AnyAsync(t => t.Id != id && t.Name == name, cancellationToken);
        if (nameExists)
        {
            return TeamResult<TeamResponse>.Fail(TeamErrorCode.NameAlreadyExists, "A team with this name already exists.");
        }

        team.Name = name;
        await _db.SaveChangesAsync(cancellationToken);

        return TeamResult<TeamResponse>.Ok(ToResponse(team));
    }

    public async Task<TeamResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var team = await _db.Teams.FindAsync([id], cancellationToken);
        if (team is null)
        {
            return TeamResult.Fail(TeamErrorCode.NotFound, "Team not found.");
        }

        var hasEpics = await _db.Epics.AnyAsync(e => e.TeamId == id, cancellationToken);
        var hasTickets = await _db.Tickets.AnyAsync(t => t.TeamId == id, cancellationToken);
        if (hasEpics || hasTickets)
        {
            return TeamResult.Fail(TeamErrorCode.HasDependentEntities, "Cannot delete a team that still has epics or tickets.");
        }

        _db.Teams.Remove(team);
        await _db.SaveChangesAsync(cancellationToken);

        return TeamResult.Ok();
    }

    private static TeamResponse ToResponse(Team team) => new()
    {
        Id = team.Id,
        Name = team.Name,
        CreatedAt = team.CreatedAt
    };
}
