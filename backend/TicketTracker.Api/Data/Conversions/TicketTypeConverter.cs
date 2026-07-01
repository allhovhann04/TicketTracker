using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Data.Conversions;

// Maps TicketType to the exact lowercase tokens required by the specification (bug/feature/fix),
// independent of the C# enum member names. See TicketTypeMapper for the shared mapping also used
// by TicketTypeJsonConverter, so the DB, request bodies, and query filters all agree.
public class TicketTypeConverter : ValueConverter<TicketType, string>
{
    public TicketTypeConverter()
        : base(type => TicketTypeMapper.ToWireString(type), wire => TicketTypeMapper.Parse(wire))
    {
    }
}
