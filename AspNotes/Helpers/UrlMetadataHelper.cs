using AspNotes.Interfaces;
using AspNotes.Models;
using System.Text;

namespace AspNotes.Helpers;

/// <summary>
/// Helper class for retrieving URL metadata.
/// </summary>
public class UrlMetadataHelper(IHttpClientFactory clientFactory)
    : IUrlMetadataHelper
{
    /// <inheritdoc />
    public async Task<HtmlDocumentMetadata> GetUrlMetadata(string url, CancellationToken cancellationToken)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var client = clientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537");

        var metadata = new HtmlDocumentMetadata();

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return metadata;
        }

        var htmlContent = await response.Content.ReadAsStringAsync();

        metadata = HtmlHelper.ParseHtmlForMetadata(htmlContent);

        return metadata;
    }
}