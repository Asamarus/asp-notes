import createFetch from '@/utils/createFetch'
import { sourcesService } from '@/services'
import { useNotesStore } from '@/store'
import events from '@/events'
import { dispatch as dispatchCrossTabEvent } from '@/hooks/useCrossTabEventListener'

import type { components } from '@/misc/openapi'
import type { NoteSource } from '@/store/notes'

function getSourceFromResponse(source: components['schemas']['SourceItemResponse']): NoteSource {
  return {
    id: source.id ?? '',
    title: source.title ?? '',
    link: source.link ?? '',
    description: source.description ?? '',
    image: source.image ?? '',
    showImage: source.showImage ?? false,
  }
}

const addNoteSourceRequest = createFetch(sourcesService.addNoteSource, (isLoading) => {
  useNotesStore.getState().setIsLoading('isAddNewNoteSourceLoading', isLoading)
})

const updateNoteSourceRequest = createFetch(sourcesService.updateNoteSource, (isLoading) => {
  useNotesStore.getState().setIsLoading('isUpdateNoteSourceLoading', isLoading)
})

const removeNoteSourceRequest = createFetch(sourcesService.removeNoteSource, (isLoading) => {
  useNotesStore.getState().setIsLoading('isRemoveNoteSourceLoading', isLoading)
})

const reorderSectionsRequest = createFetch(
  sourcesService.reorderNoteSources,
  (isLoading) => {
    useNotesStore.getState().setIsLoading('isReorderNoteSourcesLoading', isLoading)
  },
  {
    debounce: true,
  },
)

function addNewNoteSource(noteId: number, link: string) {
  addNoteSourceRequest({ noteId, link }, ({ data }) => {
    if (data) {
      const sources = data.sources?.map(getSourceFromResponse) ?? []

      useNotesStore.getState().setNoteSources(noteId, sources)

      dispatchCrossTabEvent(events.note.updated, useNotesStore.getState().notes[noteId])
    }
  })
}

function updateNoteSource(noteId: number, source: NoteSource) {
  updateNoteSourceRequest(
    {
      noteId,
      sourceId: source.id,
      link: source.link,
      title: source.title,
      description: source.description,
      image: source.image,
      showImage: source.showImage,
    },
    ({ data }) => {
      if (data) {
        const sources =
          data.sources?.map((source) => ({
            id: source.id ?? '',
            title: source.title ?? '',
            link: source.link ?? '',
            description: source.description ?? '',
            image: source.image ?? '',
            showImage: source.showImage ?? false,
          })) ?? []

        useNotesStore.getState().setNoteSources(noteId, sources)

        dispatchCrossTabEvent(events.note.updated, useNotesStore.getState().notes[noteId])
      }
    },
  )
}

function removeNoteSource(noteId: number, sourceId: string) {
  removeNoteSourceRequest({ noteId, sourceId }, ({ data }) => {
    if (data) {
      const sources =
        data.sources?.map((source) => ({
          id: source.id ?? '',
          title: source.title ?? '',
          link: source.link ?? '',
          description: source.description ?? '',
          image: source.image ?? '',
          showImage: source.showImage ?? false,
        })) ?? []

      useNotesStore.getState().setNoteSources(noteId, sources)

      dispatchCrossTabEvent(events.note.updated, useNotesStore.getState().notes[noteId])
    }
  })
}

function reorderNoteSources(noteId: number, sourceIds: string[]) {
  useNotesStore.getState().reorderNotesSources(noteId, sourceIds)
  reorderSectionsRequest({ noteId, sourceIds })
}

export default {
  addNewNoteSource,
  updateNoteSource,
  removeNoteSource,
  reorderNoteSources,
}
