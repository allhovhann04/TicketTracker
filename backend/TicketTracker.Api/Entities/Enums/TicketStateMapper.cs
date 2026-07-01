namespace TicketTracker.Api.Entities.Enums;

// Single source of truth for the TicketState <-> wire-string mapping, used by both
// the EF Core value converter (Data/Conversions/TicketStateConverter) and the JSON
// converter (TicketStateJsonConverter), so the database, request bodies, and query
// string filters all agree on the same required workflow tokens.
public static class TicketStateMapper
{
    private static readonly Dictionary<TicketState, string> ToWireMap = new()
    {
        [TicketState.New] = "new",
        [TicketState.ReadyForImplementation] = "ready_for_implementation",
        [TicketState.InProgress] = "in_progress",
        [TicketState.ReadyForAcceptance] = "ready_for_acceptance",
        [TicketState.Done] = "done"
    };

    private static readonly Dictionary<string, TicketState> FromWireMap =
        ToWireMap.ToDictionary(pair => pair.Value, pair => pair.Key);

    public static string ToWireString(TicketState state) => ToWireMap[state];

    public static bool TryParse(string? wireValue, out TicketState state)
    {
        if (wireValue is not null && FromWireMap.TryGetValue(wireValue, out state))
        {
            return true;
        }

        state = default;
        return false;
    }

    public static TicketState Parse(string wireValue) =>
        FromWireMap.TryGetValue(wireValue, out var state)
            ? state
            : throw new FormatException($"'{wireValue}' is not a valid ticket state.");
}
