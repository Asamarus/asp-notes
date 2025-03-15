using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace AspNotes.Helpers;

/// <summary>
/// Provides helper methods for JSON serialization and deserialization.
/// </summary>
public static class JsonHelper
{
    private static readonly JsonSerializerOptions DeserializationOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static readonly JsonSerializerOptions SerializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>,
    /// or default if the JSON string is null or whitespace, or if deserialization fails.</returns>
    public static T? DeserializeJson<T>(this string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonString, DeserializationOptions);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="objectToSerialize">The object to serialize.</param>
    /// <returns>The JSON string representation of the object,
    /// or null if the object is null or if serialization fails.</returns>
    public static string? SerializeJson<T>(this T objectToSerialize)
    {
        if (objectToSerialize is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Serialize(objectToSerialize, SerializationOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a value converter that converts an object
    /// of type <typeparamref name="T"/> to a JSON string and vice versa.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert.</typeparam>
    /// <returns>A value converter that converts an object
    /// of type <typeparamref name="T"/> to a JSON string and vice versa.</returns>
    public static ValueConverter<T, string> GetJsonConverter<T>()
        where T : class, new()
    {
        return new ValueConverter<T, string>(
            v => v.SerializeJson() ?? string.Empty,
            v => v.DeserializeJson<T>() ?? new T());
    }
}
