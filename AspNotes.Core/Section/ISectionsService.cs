using AspNotes.Core.Section.Models;

namespace AspNotes.Core.Section;

public interface ISectionsService
{
    Task<bool> IsSectionNameUnique(string name);
    Task<bool> IsSectionIdPresent(long id);
    Task<bool> IsSectionNameValid(string name);
    Task<bool> IsSectionHavingNotes(string name);
    Task<List<SectionDto>> GetSections();
    Task<long> CreateSection(string name, string displayName, string color);
    Task<bool> UpdateSection(long id, string displayName, string color);
    Task<bool> DeleteSection(long id);
    Task<bool> ReorderSections(List<long> sectionIds);
}
