using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketTracker.Api.Entities.Enums;

namespace TicketTracker.Api.Data.Conversions;

// Maps TicketState to the exact workflow state tokens required by the specification,
// independent of the C# enum member names.
public class TicketStateConverter : ValueConverter<TicketState, string>
{
    private static readonly Dictionary<TicketState, string> ToWire = new()
    {
        [TicketState.New] = "new",
        [TicketState.ReadyForImplementation] = "ready_for_implementation",
        [TicketState.InProgress] = "in_progress",
        [TicketState.ReadyForAcceptance] = "ready_for_acceptance",
        [TicketState.Done] = "done"
    };

    private static readonly Dictionary<string, TicketState> FromWire =
        ToWire.ToDictionary(pair => pair.Value, pair => pair.Key);

    public TicketStateConverter()
        : base(state => ToWire[state], wire => FromWire[wire])
    {
    }
}
