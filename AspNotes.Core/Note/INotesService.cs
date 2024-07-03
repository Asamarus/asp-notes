using AspNotes.Core.Note.Models;

namespace AspNotes.Core.Note;

public interface INotesService
{
    Task<bool> IsNoteIdPresent(long id);
    Task<NotesServiceSearchResult> Search(NotesServiceSearchRequest request);
    Task<List<NotesServiceAutocompleteResult>> Autocomplete(string searchTerm, string? section = null);
    Task<List<NotesServiceGetCalendarDaysResult>> GetCalendarDays(int month, int year, string? section = null);
    Task<NoteDto?> GetNoteById(long id);
    Task<NoteDto> CreateNote(NoteDto note);
    Task<NoteDto> UpdateNote(NoteDto note);
    Task<bool> DeleteNote(long id);
}
