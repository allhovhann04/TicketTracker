using System.Text.Json.Serialization;

namespace TicketTracker.Api.Entities.Enums;

[JsonConverter(typeof(TicketTypeJsonConverter))]
public enum TicketType
{
    Bug,
    Feature,
    Fix
}
