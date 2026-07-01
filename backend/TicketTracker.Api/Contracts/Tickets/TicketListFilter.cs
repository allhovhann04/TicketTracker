using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Contracts.Tickets;

public record TicketListFilter(
    Guid? TeamId,
    TicketType? Type,
    Guid? EpicId,
    TicketState? State,
    string? TitleSearch);
