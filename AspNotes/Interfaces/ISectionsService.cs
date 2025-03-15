using AspNotes.Entities;

namespace AspNotes.Interfaces;

/// <summary>
/// Provides methods to interact with the Sections in the database.
/// </summary>
public interface ISectionsService
{
    /// <summary>
    /// Checks if a section name is unique in the database.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the section name is unique.</returns>
    Task<bool> IsSectionNameUnique(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a section ID is present in the database.
    /// </summary>
    /// <param name="id">The ID of the section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the section ID is present.</returns>
    Task<bool> IsSectionIdPresent(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a section name is valid.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the section name is valid.</returns>
    Task<bool> IsSectionNameValid(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a section has any notes.
    /// </summary>
    /// <param name="id">The section id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the section has any notes.</returns>
    Task<bool> IsSectionHavingNotes(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all sections from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of sections.</returns>
    Task<List<SectionEntity>> GetSections(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new section with the specified details and adds it to the database.
    /// </summary>
    /// <param name="name">The unique name of the section.</param>
    /// <param name="displayName">The display name of the section.</param>
    /// <param name="color">The color associated with the section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ID of the newly created section.</returns>
    /// <remarks>
    /// This method automatically assigns the next available position to the new section.
    /// It also updates the sections cache after adding the new section.
    /// </remarks>
    Task<long> CreateSection(string name, string displayName, string color, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the details of an existing section.
    /// </summary>
    /// <param name="id">The ID of the section to update.</param>
    /// <param name="displayName">The new display name for the section.</param>
    /// <param name="color">The new color for the section.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified section cannot be found.</exception>
    /// <remarks>
    /// This method updates the display name and color of the specified section.
    /// It also clears and updates the sections cache after the update.
    /// </remarks>
    Task UpdateSection(long id, string displayName, string color, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a section from the database.
    /// </summary>
    /// <param name="id">The ID of the section to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteSection(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Reorders the sections in the database.
    /// </summary>
    /// <param name="sectionIds">The IDs of the sections in the new order.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a boolean indicating whether the reordering was successful.</returns>
    Task<bool> ReorderSections(List<long> sectionIds, CancellationToken cancellationToken);
}