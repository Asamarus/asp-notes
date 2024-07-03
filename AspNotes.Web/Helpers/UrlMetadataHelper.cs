using AspNotes.Core.Common.Helpers;
using AspNotes.Core.Common.Models;

namespace AspNotes.Web.Helpers;

public class UrlMetadataHelper(IHttpClientFactory clientFactory)
{
    public async Task<HtmlDocumentMetadata> GetUrlMetadata(string url)
    {
        var client = clientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537");

        var metadata = new HtmlDocumentMetadata();

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return metadata;
        }

        var htmlContent = await response.Content.ReadAsStringAsync();

        return HtmlHelper.ParseHtmlForMetadata(htmlContent);
    }
}
