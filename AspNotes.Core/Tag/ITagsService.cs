using AspNotes.Core.Tag.Models;

namespace AspNotes.Core.Tag;

public interface ITagsService
{
    Task<bool> UpdateNoteTags(long noteId, HashSet<string> newTags);
    Task<List<TagsServiceAutocompleteResultItem>> Autocomplete(string searchTerm, string? section = null);
    Task<List<TagsServiceGetTagsResultItem>> GetTags(string? section = null);
}
