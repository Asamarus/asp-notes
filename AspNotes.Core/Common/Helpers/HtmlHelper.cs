using AspNotes.Core.Common.Models;
using HtmlAgilityPack;

namespace AspNotes.Core.Common.Helpers;

/// <summary>
/// Provides helper methods for handling HTML content.
/// </summary>
public static class HtmlHelper
{
    /// <summary>
    /// Strips HTML tags from the given HTML string.
    /// </summary>
    /// <param name="html">The HTML string to strip tags from.</param>
    /// <returns>The text content of the HTML string, without any HTML tags.</returns>
    public static string StripTags(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        return htmlDoc.DocumentNode.InnerText;
    }

    /// <summary>
    /// Parses the given HTML content for metadata.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <returns>An HtmlDocumentMetadata object containing the parsed metadata.</returns>
    public static HtmlDocumentMetadata ParseHtmlForMetadata(string htmlContent)
    {
        var metadata = new HtmlDocumentMetadata();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        var metaTags = htmlDocument.DocumentNode.SelectNodes("//meta");

        if (metaTags != null)
        {
            foreach (var tag in metaTags)
            {
                var property = tag.GetAttributeValue("property", "");
                var content = tag.GetAttributeValue("content", "");

                switch (property)
                {
                    case "og:title":
                        metadata.Title = content;
                        break;
                    case "og:description":
                        metadata.Description = content;
                        break;
                    case "og:image":
                        metadata.Image = content;
                        break;
                }
            }
        }

        if (string.IsNullOrEmpty(metadata.Title))
        {
            var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");
            if (titleNode != null)
            {
                metadata.Title = titleNode.InnerText;
            }
        }

        if (string.IsNullOrEmpty(metadata.Description))
        {
            var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='description']");
            if (descriptionNode != null)
            {
                metadata.Description = descriptionNode.GetAttributeValue("content", "");
            }
        }

        return metadata;
    }
}
