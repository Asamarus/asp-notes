using AspNotes.Core.Section.Models;

namespace AspNotes.Core.Section;

public interface ISectionsService
{
    Task<bool> IsSectionNameUnique(string name);
    Task<bool> IsSectionIdPresent(long id);
    Task<bool> IsSectionNameValid(string name);
    Task<bool> IsSectionHavingNotes(string name);
    Task<List<SectionDto>> GetSections();
    Task<long> CreateSection(SectionDto section);
    Task<bool> UpdateSection(SectionDto section);
    Task<bool> DeleteSection(long id);
    Task<bool> ReorderSections(List<long> sectionIds);
}
