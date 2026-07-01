using Microsoft.EntityFrameworkCore;
using TicketTracker.Api.Contracts.Teams;
using TicketTracker.Api.Contracts.Tickets;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;
using TicketTracker.Api.Entities.Enums;
using TicketTracker.Api.Services.Teams;
using TicketTracker.Api.Services.Tickets;
using Xunit;

namespace TicketTracker.Api.Tests;

// Business-flow test: seed a user, create a team, create a ticket, change its state,
// and confirm ModifiedAt only advances when a field/state actually changes.
// Uses EF Core's InMemory provider so the test runs anywhere with no Docker/Postgres
// dependency - fast and reliable for hackathon iteration. Postgres-only behavior
// (citext case-insensitive uniqueness, ON DELETE CASCADE) is not exercised here and
// remains covered by the manual testing steps from Steps 5-8.
public class TicketFlowTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task TicketFlow_UpdatesModifiedAtOnlyWhenFieldsOrStateActuallyChange()
    {
        await using var db = CreateDbContext();

        var teamService = new TeamService(db);
        var ticketService = new TicketService(db);

        // 1. Create user (seeded directly - the auth pipeline itself is covered by Step 4's tests).
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "flow-test@example.com",
            PasswordHash = "not-used-in-this-test",
            DisplayName = "Flow Test User",
            IsEmailVerified = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // 2. Create team.
        var teamResult = await teamService.CreateAsync(new CreateTeamRequest { Name = "Flow Test Team" });
        Assert.True(teamResult.Success);
        var teamId = teamResult.Value!.Id;

        // 3. Create ticket.
        var createResult = await ticketService.CreateAsync(
            new CreateTicketRequest
            {
                TeamId = teamId,
                Title = "Fix the login bug",
                Body = "Steps to reproduce the issue.",
                Type = TicketType.Bug
            },
            user.Id);

        Assert.True(createResult.Success);
        var ticket = createResult.Value!;
        Assert.Equal(TicketState.New, ticket.State);
        Assert.Equal(user.Id, ticket.CreatedByUserId);

        var initialUpdatedAt = ticket.UpdatedAt;

        // Ensure enough real time passes for UpdatedAt to be measurably different below.
        await Task.Delay(15);

        // 4. Update ticket state - an actual change, so ModifiedAt must advance.
        var stateChangeResult = await ticketService.UpdateAsync(
            ticket.Id,
            new UpdateTicketRequest
            {
                TeamId = ticket.TeamId,
                EpicId = ticket.EpicId,
                Title = ticket.Title,
                Body = ticket.Body,
                Type = ticket.Type,
                State = TicketState.InProgress
            });

        Assert.True(stateChangeResult.Success);
        var updatedTicket = stateChangeResult.Value!;
        Assert.Equal(TicketState.InProgress, updatedTicket.State);
        Assert.True(updatedTicket.UpdatedAt > initialUpdatedAt);

        var updatedAtAfterStateChange = updatedTicket.UpdatedAt;

        // 5. Save identical values again - ModifiedAt must NOT change.
        var noOpResult = await ticketService.UpdateAsync(
            ticket.Id,
            new UpdateTicketRequest
            {
                TeamId = updatedTicket.TeamId,
                EpicId = updatedTicket.EpicId,
                Title = updatedTicket.Title,
                Body = updatedTicket.Body,
                Type = updatedTicket.Type,
                State = updatedTicket.State
            });

        Assert.True(noOpResult.Success);
        Assert.Equal(updatedAtAfterStateChange, noOpResult.Value!.UpdatedAt);
    }
}
