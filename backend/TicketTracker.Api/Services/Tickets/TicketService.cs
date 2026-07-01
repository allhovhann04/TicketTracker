using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Contracts.Tickets;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;
using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Services.Tickets;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TicketResult<TicketResponse>> CreateAsync(CreateTicketRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
    {
        var title = request.Title.Trim();
        if (title.Length == 0)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.InvalidTitle, "Ticket title must not be empty.");
        }

        var body = request.Body.Trim();
        if (body.Length == 0)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.InvalidBody, "Ticket body must not be empty.");
        }

        var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
        if (!teamExists)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.TeamNotFound, "Team not found.");
        }

        if (request.EpicId.HasValue)
        {
            var epic = await _db.Epics.FindAsync([request.EpicId.Value], cancellationToken);
            if (epic is null)
            {
                return TicketResult<TicketResponse>.Fail(TicketErrorCode.EpicNotFound, "Epic not found.");
            }

            if (epic.TeamId != request.TeamId)
            {
                return TicketResult<TicketResponse>.Fail(TicketErrorCode.EpicNotInTeam, "The epic does not belong to the specified team.");
            }
        }

        var now = DateTimeOffset.UtcNow;
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TeamId = request.TeamId,
            EpicId = request.EpicId,
            Title = title,
            Description = body,
            Type = request.Type,
            State = TicketState.New,
            CreatedByUserId = createdByUserId,
            AssigneeUserId = null,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        return TicketResult<TicketResponse>.Ok(ToResponse(ticket));
    }

    public async Task<IReadOnlyList<TicketResponse>> ListAsync(TicketListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Tickets.AsQueryable();

        if (filter.TeamId.HasValue)
        {
            query = query.Where(t => t.TeamId == filter.TeamId.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(t => t.Type == filter.Type.Value);
        }

        if (filter.EpicId.HasValue)
        {
            query = query.Where(t => t.EpicId == filter.EpicId.Value);
        }

        if (filter.State.HasValue)
        {
            query = query.Where(t => t.State == filter.State.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.TitleSearch))
        {
            var search = filter.TitleSearch.Trim();
            query = query.Where(t => EF.Functions.ILike(t.Title, $"%{search}%"));
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                TeamId = t.TeamId,
                EpicId = t.EpicId,
                Title = t.Title,
                Body = t.Description!,
                Type = t.Type,
                State = t.State,
                CreatedByUserId = t.CreatedByUserId,
                AssigneeUserId = t.AssigneeUserId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TicketResult<TicketResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FindAsync([id], cancellationToken);
        if (ticket is null)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.NotFound, "Ticket not found.");
        }

        return TicketResult<TicketResponse>.Ok(ToResponse(ticket));
    }

    public async Task<TicketResult<TicketResponse>> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FindAsync([id], cancellationToken);
        if (ticket is null)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.NotFound, "Ticket not found.");
        }

        var title = request.Title.Trim();
        if (title.Length == 0)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.InvalidTitle, "Ticket title must not be empty.");
        }

        var body = request.Body.Trim();
        if (body.Length == 0)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.InvalidBody, "Ticket body must not be empty.");
        }

        var teamExists = await _db.Teams.AnyAsync(t => t.Id == request.TeamId, cancellationToken);
        if (!teamExists)
        {
            return TicketResult<TicketResponse>.Fail(TicketErrorCode.TeamNotFound, "Team not found.");
        }

        var teamChanged = request.TeamId != ticket.TeamId;
        Guid? resolvedEpicId = request.EpicId;

        if (resolvedEpicId.HasValue)
        {
            var epic = await _db.Epics.FindAsync([resolvedEpicId.Value], cancellationToken);
            if (epic is null)
            {
                return TicketResult<TicketResponse>.Fail(TicketErrorCode.EpicNotFound, "Epic not found.");
            }

            if (epic.TeamId != request.TeamId)
            {
                if (!teamChanged)
                {
                    return TicketResult<TicketResponse>.Fail(TicketErrorCode.EpicNotInTeam, "The epic does not belong to the ticket's team.");
                }

                // Team is changing and the referenced epic belongs to neither the old nor the
                // new team: clear it instead of rejecting the whole update.
                resolvedEpicId = null;
            }
        }

        var hasChanges = false;

        if (ticket.Title != title)
        {
            ticket.Title = title;
            hasChanges = true;
        }

        if (ticket.Description != body)
        {
            ticket.Description = body;
            hasChanges = true;
        }

        if (ticket.Type != request.Type)
        {
            ticket.Type = request.Type;
            hasChanges = true;
        }

        if (ticket.State != request.State)
        {
            ticket.State = request.State;
            hasChanges = true;
        }

        if (ticket.TeamId != request.TeamId)
        {
            ticket.TeamId = request.TeamId;
            hasChanges = true;
        }

        if (ticket.EpicId != resolvedEpicId)
        {
            ticket.EpicId = resolvedEpicId;
            hasChanges = true;
        }

        if (hasChanges)
        {
            ticket.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return TicketResult<TicketResponse>.Ok(ToResponse(ticket));
    }

    public async Task<TicketResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.FindAsync([id], cancellationToken);
        if (ticket is null)
        {
            return TicketResult.Fail(TicketErrorCode.NotFound, "Ticket not found.");
        }

        // Comments are deleted by the database via ON DELETE CASCADE (configured in Step 3).
        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        return TicketResult.Ok();
    }

    private static TicketResponse ToResponse(Ticket ticket) => new()
    {
        Id = ticket.Id,
        TeamId = ticket.TeamId,
        EpicId = ticket.EpicId,
        Title = ticket.Title,
        Body = ticket.Description!,
        Type = ticket.Type,
        State = ticket.State,
        CreatedByUserId = ticket.CreatedByUserId,
        AssigneeUserId = ticket.AssigneeUserId,
        CreatedAt = ticket.CreatedAt,
        UpdatedAt = ticket.UpdatedAt
    };
}
