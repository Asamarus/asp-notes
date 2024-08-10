using AspNotes.Core.Note.Models;

namespace AspNotes.Core.Note;

public interface INotesService
{
    Task<bool> IsNoteIdPresent(long id);
    Task<NotesServiceSearchResult> Search(NotesServiceSearchRequest request);
    Task<List<NotesServiceAutocompleteResult>> Autocomplete(string searchTerm, string? section = null);
    Task<List<NotesServiceGetCalendarDaysResult>> GetCalendarDays(int month, int year, string? section = null);
    Task<NoteDto?> GetNoteById(long id);
    Task<NoteDto> CreateNote(string section);
    Task<NoteDto> UpdateNoteSection(long id, string section);
    Task<NoteDto> UpdateNoteTitleAndContent(long id, string title, string content);
    Task<NoteDto> UpdateNoteSources(long id, List<NoteSource> sources);
    Task<bool> DeleteNote(long id);
}
