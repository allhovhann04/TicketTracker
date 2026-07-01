using TicketTracker.Api.Contracts.Tickets;

namespace TicketTracker.Api.Services.Tickets;

public interface ITicketService
{
    Task<TicketResult<TicketResponse>> CreateAsync(CreateTicketRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketResponse>> ListAsync(TicketListFilter filter, CancellationToken cancellationToken = default);
    Task<TicketResult<TicketResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TicketResult<TicketResponse>> UpdateAsync(Guid id, UpdateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
