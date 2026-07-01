namespace TicketTracker.Api.Services.Tickets;

public enum TicketErrorCode
{
    None,
    NotFound,
    TeamNotFound,
    EpicNotFound,
    EpicNotInTeam,
    InvalidTitle,
    InvalidBody
}
