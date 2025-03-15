using AspNotes.Models;

namespace AspNotes.Interfaces;

/// <summary>
/// Interface for URL metadata helper.
/// </summary>
public interface IUrlMetadataHelper
{
    /// <summary>
    /// Gets the metadata of the specified URL.
    /// </summary>
    /// <param name="url">The URL to get metadata for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>"
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains the HTML document metadata.</returns>
    Task<HtmlDocumentMetadata> GetUrlMetadata(string url, CancellationToken cancellationToken);
}