using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketTracker.Api.Entities.Enums;

public class TicketTypeJsonConverter : JsonConverter<TicketType>
{
    public override TicketType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (!TicketTypeMapper.TryParse(value, out var type))
        {
            throw new JsonException($"'{value}' is not a valid ticket type.");
        }

        return type;
    }

    public override void Write(Utf8JsonWriter writer, TicketType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(TicketTypeMapper.ToWireString(value));
    }
}
