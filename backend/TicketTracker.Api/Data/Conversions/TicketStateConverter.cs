using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Data.Conversions;

// Maps TicketState to the exact workflow state tokens required by the specification,
// independent of the C# enum member names. See TicketStateMapper for the shared mapping
// also used by TicketStateJsonConverter, so the DB, request bodies, and query filters agree.
public class TicketStateConverter : ValueConverter<TicketState, string>
{
    public TicketStateConverter()
        : base(state => TicketStateMapper.ToWireString(state), wire => TicketStateMapper.Parse(wire))
    {
    }
}
