using TicketTracker.Api.Contracts.Epics;

namespace TicketTracker.Api.Services.Epics;

public interface IEpicService
{
    Task<EpicResult<EpicResponse>> CreateAsync(CreateEpicRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EpicResponse>> ListAsync(Guid? teamId, CancellationToken cancellationToken = default);
    Task<EpicResult<EpicResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EpicResult<EpicResponse>> UpdateAsync(Guid id, UpdateEpicRequest request, CancellationToken cancellationToken = default);
    Task<EpicResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
