using TicketTracker.Api.Contracts.Teams;

namespace TicketTracker.Api.Services.Teams;

public interface ITeamService
{
    Task<TeamResult<TeamResponse>> CreateAsync(CreateTeamRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeamResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<TeamResult<TeamResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TeamResult<TeamResponse>> UpdateAsync(Guid id, UpdateTeamRequest request, CancellationToken cancellationToken = default);
    Task<TeamResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
