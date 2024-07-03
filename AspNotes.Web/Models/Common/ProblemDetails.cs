namespace AspNotes.Web.Models.Common;

public class ProblemDetails
{
    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Status { get; set; }

    public Dictionary<string, string[]> Errors { get; set; } = [];

    public string TraceId { get; set; } = null!;

    public string Detail { get; set; } = null!;
}
