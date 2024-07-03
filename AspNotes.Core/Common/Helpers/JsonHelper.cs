using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace AspNotes.Core.Common.Helpers;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions deserializationOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions serializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static T? DeserializeJson<T>(this string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(jsonString, deserializationOptions);
        }
        catch (Exception) {
            return default;
        }
    }

    public static string? SerializeJson<T>(this T objectToSerialize)
    {
        if (objectToSerialize == null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Serialize(objectToSerialize, serializationOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static ValueConverter<T, string> GetJsonConverter<T>() where T : class, new()
    {
        return new ValueConverter<T, string>(
            v => v.SerializeJson() ?? string.Empty,
            v => v.DeserializeJson<T>() ?? new T());
    }
}
