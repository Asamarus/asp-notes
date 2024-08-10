import createFetch from '@/utils/createFetch'
import { notesService } from '@/services'
import { useAutocompleteStore, useSectionsStore } from '@/store'

import type { AutocompleteNote } from '@/store/autocomplete'

const autocompleteRequest = createFetch(
  notesService.autocompleteNotes,
  useAutocompleteStore.getState().setIsLoading,
  {
    debounce: true,
    concurrent: true,
  },
)

function setSearchTerm(searchTerm: string) {
  useAutocompleteStore.getState().setSearchTerm(searchTerm)
  if (searchTerm.length > 2) {
    const currentResetId = useAutocompleteStore.getState().resetId
    autocompleteRequest(
      { searchTerm: searchTerm, section: useSectionsStore.getState().currentSection?.name },
      ({ data }) => {
        if (data && currentResetId === useAutocompleteStore.getState().resetId) {
          const notes: AutocompleteNote[] = []

          data.notes?.forEach((note) => {
            notes.push({
              id: note.id ?? 0,
              title: note.title ?? '',
            })
          })

          useAutocompleteStore.getState().setResponse({
            notes: notes,
            books: data.books ?? [],
            tags: data.tags ?? [],
          })
        }
      },
    )
  }
}

export default { setSearchTerm }
