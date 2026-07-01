using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Contracts.Epics;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;

namespace TicketTracker.Api.Services.Epics;

public class EpicService : IEpicService
{
    private readonly AppDbContext _db;

    public EpicService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EpicResult<EpicResponse>> CreateAsync(CreateEpicRequest request, CancellationToken cancellationToken = default)
    {
        var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
        if (!teamExists)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.TeamNotFound, "Team not found.");
        }

        var title = request.Title.Trim();
        if (title.Length == 0)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.InvalidTitle, "Epic title must not be empty.");
        }

        var titleExists = await _db.Epics.AnyAsync(
            e => e.TeamId == request.TeamId && e.Title == title, cancellationToken);
        if (titleExists)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.TitleAlreadyExistsInTeam, "An epic with this title already exists in this team.");
        }

        var now = DateTimeOffset.UtcNow;
        var epic = new Epic
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            Title = title,
            Description = NormalizeDescription(request.Description),
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Epics.Add(epic);
        await _db.SaveChangesAsync(cancellationToken);

        return EpicResult<EpicResponse>.Ok(ToResponse(epic));
    }

    public async Task<IReadOnlyList<EpicResponse>> ListAsync(Guid? teamId, CancellationToken cancellationToken = default)
    {
        var query = _db.Epics.AsQueryable();
        if (teamId.HasValue)
        {
            query = query.Where(e => e.TeamId == teamId.Value);
        }

        return await query
            .OrderBy(e => e.Title)
            .Select(e => new EpicResponse
            {
                Id = e.Id,
                TeamId = e.TeamId,
                Title = e.Title,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<EpicResult<EpicResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var epic = await _db.Epics.FindAsync([id], cancellationToken);
        if (epic is null)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.NotFound, "Epic not found.");
        }

        return EpicResult<EpicResponse>.Ok(ToResponse(epic));
    }

    public async Task<EpicResult<EpicResponse>> UpdateAsync(Guid id, UpdateEpicRequest request, CancellationToken cancellationToken = default)
    {
        var epic = await _db.Epics.FindAsync([id], cancellationToken);
        if (epic is null)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.NotFound, "Epic not found.");
        }

        var title = request.Title.Trim();
        if (title.Length == 0)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.InvalidTitle, "Epic title must not be empty.");
        }

        var titleExists = await _db.Epics.AnyAsync(
            e => e.Id != id && e.TeamId == epic.TeamId && e.Title == title, cancellationToken);
        if (titleExists)
        {
            return EpicResult<EpicResponse>.Fail(EpicErrorCode.TitleAlreadyExistsInTeam, "An epic with this title already exists in this team.");
        }

        epic.Title = title;
        epic.Description = NormalizeDescription(request.Description);
        epic.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return EpicResult<EpicResponse>.Ok(ToResponse(epic));
    }

    public async Task<EpicResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var epic = await _db.Epics.FindAsync([id], cancellationToken);
        if (epic is null)
        {
            return EpicResult.Fail(EpicErrorCode.NotFound, "Epic not found.");
        }

        var hasTickets = await _db.Tickets.AnyAsync(t => t.EpicId == id, cancellationToken);
        if (hasTickets)
        {
            return EpicResult.Fail(EpicErrorCode.HasDependentTickets, "Cannot delete an epic that still has tickets referencing it.");
        }

        _db.Epics.Remove(epic);
        await _db.SaveChangesAsync(cancellationToken);

        return EpicResult.Ok();
    }

    private static string? NormalizeDescription(string? description)
    {
        var trimmed = description?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static EpicResponse ToResponse(Epic epic) => new()
    {
        Id = epic.Id,
        TeamId = epic.TeamId,
        Title = epic.Title,
        Description = epic.Description,
        CreatedAt = epic.CreatedAt,
        UpdatedAt = epic.UpdatedAt
    };
}
