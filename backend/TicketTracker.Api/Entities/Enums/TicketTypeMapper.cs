namespace TicketTracker.Api.Entities.Enums;

// Single source of truth for the TicketType <-> wire-string mapping, used by both
// the EF Core value converter (Data/Conversions/TicketTypeConverter) and the JSON
// converter (TicketTypeJsonConverter), so the database, request bodies, and query
// string filters all agree on the exact lowercase tokens required by the specification.
public static class TicketTypeMapper
{
    private static readonly Dictionary<TicketType, string> ToWireMap = new()
    {
        [TicketType.Bug] = "bug",
        [TicketType.Feature] = "feature",
        [TicketType.Fix] = "fix"
    };

    private static readonly Dictionary<string, TicketType> FromWireMap =
        ToWireMap.ToDictionary(pair => pair.Value, pair => pair.Key);

    public static string ToWireString(TicketType type) => ToWireMap[type];

    public static bool TryParse(string? wireValue, out TicketType type)
    {
        if (wireValue is not null && FromWireMap.TryGetValue(wireValue, out type))
        {
            return true;
        }

        type = default;
        return false;
    }

    public static TicketType Parse(string wireValue) =>
        FromWireMap.TryGetValue(wireValue, out var type)
            ? type
            : throw new FormatException($"'{wireValue}' is not a valid ticket type.");
}
