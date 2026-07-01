using System.Text.Json.Serialization;

namespace TicketTracker.Api.Entities.Enums;

[JsonConverter(typeof(TicketStateJsonConverter))]
public enum TicketState
{
    New,
    ReadyForImplementation,
    InProgress,
    ReadyForAcceptance,
    Done
}
