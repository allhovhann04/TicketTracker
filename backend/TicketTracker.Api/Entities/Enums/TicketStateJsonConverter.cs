using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketTracker.Api.Entities.Enums;

public class TicketStateJsonConverter : JsonConverter<TicketState>
{
    public override TicketState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (!TicketStateMapper.TryParse(value, out var state))
        {
            throw new JsonException($"'{value}' is not a valid ticket state.");
        }

        return state;
    }

    public override void Write(Utf8JsonWriter writer, TicketState value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(TicketStateMapper.ToWireString(value));
    }
}
