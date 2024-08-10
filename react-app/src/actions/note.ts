import createFetch from '@/utils/createFetch'
import { notesService } from '@/services'
import { useNotesStore } from '@/store'
import { dispatch as dispatchCrossTabEvent } from '@/hooks/useCrossTabEventListener'
import { getNoteFromResponse } from './notes'

import events from '@/events'

const getNoteRequest = createFetch(notesService.getNote, (isLoading) => {
  useNotesStore.getState().setIsLoading('isGetNoteLoading', isLoading)
})

const updateNoteRequest = createFetch(notesService.updateNote, (isLoading) => {
  useNotesStore.getState().setIsLoading('isUpdateNoteLoading', isLoading)
})

const deleteNoteRequest = createFetch(notesService.deleteNote, (isLoading) => {
  useNotesStore.getState().setIsLoading('isDeleteNoteLoading', isLoading)
})

const updateNoteBookRequest = createFetch(notesService.updateNoteBook, (isLoading) => {
  useNotesStore.getState().setIsLoading('isUpdateNoteBookLoading', isLoading)
})

const updateNoteTagsRequest = createFetch(notesService.updateNoteTags, (isLoading) => {
  useNotesStore.getState().setIsLoading('isUpdateNoteTagsLoading', isLoading)
})

const updateNoteSectionRequest = createFetch(notesService.updateNoteSection, (isLoading) => {
  useNotesStore.getState().setIsLoading('isUpdateNoteSectionLoading', isLoading)
})

function getNoteById(id: number) {
  getNoteRequest(
    {
      id,
    },
    ({ data }) => {
      if (data) {
        const note = getNoteFromResponse(data.note)
        useNotesStore.getState().setNote(id, note)
      }
    },
  )
}

function updateNote(id: number, { title, content }: { title: string; content: string }) {
  updateNoteRequest({ id, title, content }, ({ data }) => {
    if (data) {
      const note = getNoteFromResponse(data.note)
      useNotesStore.getState().setNote(note.id, note)
      dispatchCrossTabEvent(events.note.updated, note)
    }
  })
}

function deleteNote(id: number) {
  deleteNoteRequest({ id }, ({ data }) => {
    if (data) {
      useNotesStore.getState().removeNote(id)

      dispatchCrossTabEvent(events.note.deleted, id)
    }
  })
}

function updateNoteBook(id: number, book: string) {
  updateNoteBookRequest({ id, book }, ({ data }) => {
    if (data) {
      const note = getNoteFromResponse(data.note)
      useNotesStore.getState().setNote(note.id, note)
      dispatchCrossTabEvent(events.note.updated, note)
    }
  })
}

function updateNoteTags(id: number, tags: string[]) {
  updateNoteTagsRequest({ id, tags }, ({ data }) => {
    if (data) {
      const note = getNoteFromResponse(data.note)
      useNotesStore.getState().setNote(note.id, note)
      dispatchCrossTabEvent(events.note.updated, note)
    }
  })
}

function updateNoteSection(id: number, section: string) {
  updateNoteSectionRequest({ id, section }, ({ data }) => {
    if (data) {
      const note = getNoteFromResponse(data.note)
      useNotesStore.getState().setNote(note.id, note)
      dispatchCrossTabEvent(events.note.updated, note)
    }
  })
}

export default {
  getNoteById,
  updateNote,
  deleteNote,
  updateNoteBook,
  updateNoteTags,
  updateNoteSection,
}
