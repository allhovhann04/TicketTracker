namespace TicketTracker.Api.Services.Teams;

public enum TeamErrorCode
{
    None,
    NotFound,
    InvalidName,
    NameAlreadyExists,
    HasDependentEntities
}
