using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;

namespace AspNotes.Core.Tag;

/// <summary>
/// Provides utilities for working with tag lists, including methods to compress and extract tags.
/// </summary>
public partial class TagsHelper
{
    /// <summary>
    /// A regular expression for replacing brackets in tag strings.
    /// </summary>
    [GeneratedRegex(@"\[|\]")]
    private static partial Regex TagsReplaceRegex();

    /// <summary>
    /// Compresses a list of tags into a single string.
    /// </summary>
    /// <param name="tags">The list of tags to compress.</param>
    /// <returns>A string representing the compressed tags. For example, "[tag1],[tag2],[tag3]".</returns>
    public static string Compress(List<string> tags)
    {
        var result = new List<string>();
        tags = [.. tags.OrderBy(t => t)];

        foreach (string tag in tags)
        {
            result.Add($"[{tag.Trim()}]");
        }

        return string.Join(",", result);
    }

    /// <summary>
    /// Extracts a list of tags from a single string.
    /// </summary>
    /// <param name="tags">A string representing the compressed tags. For example, "[tag1],[tag2],[tag3]".</param>
    /// <returns>A list of tags extracted from the input string. If the input string is null or empty, returns an empty list.</returns>
    public static List<string> Extract(string tags)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(tags))
        {
            return result;
        }

        foreach (string tag in tags.Split(','))
        {
            string cleanedTag = TagsReplaceRegex().Replace(tag, "");
            if (!string.IsNullOrEmpty(cleanedTag))
            {
                result.Add(cleanedTag);
            }
        }

        return result;
    }

    /// <summary>
    /// Provides a <see cref="ValueConverter{TModel,TProvider}"/> for converting between lists of tags and their string representation.
    /// </summary>
    /// <returns>A <see cref="ValueConverter{TModel,TProvider}"/> that can be used to convert between a list of strings and a single string representation.</returns>
    public static ValueConverter<List<string>, string> GetTagsConverter()
    {
        return new ValueConverter<List<string>, string>(
            v => Compress(v),
            v => Extract(v));
    }
}
