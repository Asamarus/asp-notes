using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNotes.Web.Common.Converters;

/// <summary>
/// A custom JSON converter that trims leading and trailing whitespace from string values during deserialization.
/// </summary>
public class TrimmingStringJsonConverter : JsonConverter<string>
{
    /// <summary>
    /// Reads and trims the string value from the JSON.
    /// </summary>
    /// <param name="reader">The reader to read the JSON from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>The trimmed string value.</returns>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value?.Trim();
    }

    /// <summary>
    /// Writes the string value to the JSON.
    /// </summary>
    /// <param name="writer">The writer to write the JSON to.</param>
    /// <param name="value">The string value to write.</param>
    /// <param name="options">Options to control the conversion behavior.</param>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
