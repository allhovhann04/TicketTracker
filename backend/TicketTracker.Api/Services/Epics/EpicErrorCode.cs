namespace TicketTracker.Api.Services.Epics;

public enum EpicErrorCode
{
    None,
    NotFound,
    TeamNotFound,
    InvalidTitle,
    HasDependentTickets
}
