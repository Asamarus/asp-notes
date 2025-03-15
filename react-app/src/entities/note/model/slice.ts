import { createSlice, PayloadAction } from '@reduxjs/toolkit'

import type { Note, NoteSource, NotesFilters, NotesMetadata } from './types'

type State = {
  ids: number[]
  collection: Record<string, Note>
  metadata: NotesMetadata
  filters: NotesFilters
  noteIsNotSaved: boolean
}

const initialState: State = {
  ids: [],
  collection: {},
  metadata: {
    total: 0,
    canLoadMore: false,
    page: 1,
    keywords: [],
    foundWholePhrase: false,
  },
  filters: {
    searchTerm: '',
    book: '',
    tags: [],
    withoutBook: false,
    withoutTags: false,
    inRandomOrder: false,
    fromDate: '',
    toDate: '',
  },
  noteIsNotSaved: false,
}

export const notesSlice = createSlice({
  name: 'notes',
  initialState,
  reducers: {
    setNotes: (state, action: PayloadAction<Note[]>) => {
      state.ids = action.payload.map((note) => note.id)
      state.collection = {}
      action.payload.forEach((note) => {
        state.collection[note.id] = note
      })
    },
    appendNotes: (state, action: PayloadAction<Note[]>) => {
      action.payload.forEach((note) => {
        state.ids.push(note.id)
        state.collection[note.id] = note
      })
    },
    removeNote: (state, action: PayloadAction<number>) => {
      delete state.collection[action.payload]
      const idIndex = state.ids.indexOf(action.payload)
      if (idIndex !== -1) {
        state.ids = state.ids.filter((noteId) => noteId !== action.payload)
        state.metadata.total -= 1
      }
    },
    setMetaData: (state, action: PayloadAction<NotesMetadata>) => {
      state.metadata = action.payload
    },
    setFilters: (state, action: PayloadAction<Partial<NotesFilters>>) => {
      state.filters = {
        ...state.filters,
        ...action.payload,
      }
    },
    setNote: (state, action: PayloadAction<{ id: number; note: Note }>) => {
      state.collection[action.payload.id] = action.payload.note
    },
    setNoteSources(state, action: PayloadAction<{ id: number; sources: NoteSource[] }>) {
      state.collection[action.payload.id].sources = action.payload.sources
    },
    reorderNotesSources(state, action: PayloadAction<{ id: number; sourceIds: string[] }>) {
      const sources = action.payload.sourceIds
        .map((sourceId) =>
          state.collection[action.payload.id].sources?.find((source) => source.id === sourceId),
        )
        .filter((source) => source !== undefined)
      state.collection[action.payload.id].sources = sources
    },
    reset: (state) => {
      state.ids = []
      state.collection = {}
      state.metadata = {
        total: 0,
        canLoadMore: false,
        page: 1,
        keywords: [],
        foundWholePhrase: false,
      }
      state.filters = {
        searchTerm: '',
        book: '',
        tags: [],
        withoutBook: false,
        withoutTags: false,
        inRandomOrder: false,
        fromDate: '',
        toDate: '',
      }
    },
    setNoteIsNotSaved: (state, action: PayloadAction<boolean>) => {
      state.noteIsNotSaved = action.payload
    },
    setIds: (state, action: PayloadAction<number[]>) => {
      state.ids = action.payload
    },
  },
})

export const {
  setNotes,
  appendNotes,
  removeNote,
  setMetaData,
  setFilters,
  setNote,
  setNoteSources,
  reorderNotesSources,
  reset,
  setNoteIsNotSaved,
  setIds,
} = notesSlice.actions
