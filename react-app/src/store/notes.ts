import randomId from '@/utils/randomId'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface Note {
  id: number
  createdAt: string
  updatedAt: string
  title: string
  section: string
  content: string
  preview: string
  tags: string[]
  book: string
  sources: NoteSource[]
}

export interface NoteSource {
  id: string
  title: string
  link: string
  description: string
  image: string
  showImage: boolean
}

export interface NotesFilters {
  book: string
  tags: string[]
  withoutBook: boolean
  withoutTags: boolean
  inRandomOrder: boolean
  fromDate: string
  toDate: string
}

export interface NotesMetadata {
  total: number
  canLoadMore: boolean
  page: number
  keywords: string[]
  foundWholePhrase: boolean
}

type LoadingState =
  | 'isLoading'
  | 'isGetNoteLoading'
  | 'isAddNewNoteLoading'
  | 'isUpdateNoteLoading'
  | 'isDeleteNoteLoading'
  | 'isAddNewNoteSourceLoading'
  | 'isUpdateNoteSourceLoading'
  | 'isRemoveNoteSourceLoading'
  | 'isReorderNoteSourcesLoading'
  | 'isUpdateNoteBookLoading'
  | 'isUpdateNoteTagsLoading'
  | 'isUpdateNoteSectionLoading'

type State = NotesFilters &
  NotesMetadata & {
    ids: number[]
    notes: Record<string, Note>
    resetId: string
    isLoading: boolean
    isGetNoteLoading: boolean
    isAddNewNoteLoading: boolean
    isUpdateNoteLoading: boolean
    isDeleteNoteLoading: boolean
    isAddNewNoteSourceLoading: boolean
    isUpdateNoteSourceLoading: boolean
    isRemoveNoteSourceLoading: boolean
    isReorderNoteSourcesLoading: boolean
    isUpdateNoteBookLoading: boolean
    isUpdateNoteTagsLoading: boolean
    isUpdateNoteSectionLoading: boolean
  }

type Actions = {
  setIsLoading: (stateName: LoadingState, isLoading: boolean) => void
  setBook: (book: string) => void
  setTags: (tags: string[]) => void
  setWithoutBook: (withoutBook: boolean) => void
  setWithoutTags: (withoutTags: boolean) => void
  setInRandomOrder: (inRandomOrder: boolean) => void
  setDateRange: (date: { fromDate: string; toDate: string }) => void
  setNotes: (notes: Note[]) => void
  appendNotes: (notes: Note[]) => void
  removeNote: (noteId: number) => void
  setMetaData: (metadata: NotesMetadata) => void
  setFilters: (filters: NotesFilters) => void
  setNote: (noteId: number, note: Note) => void
  setNoteSources: (noteId: number, sources: NoteSource[]) => void
  reorderNotesSources: (noteId: number, sourceIds: string[]) => void
  reset: () => void
}

export default create<State & Actions>()(
  immer((set) => ({
    ids: [],
    notes: {},
    book: '',
    tags: [],
    withoutBook: false,
    withoutTags: false,
    inRandomOrder: false,
    fromDate: '',
    toDate: '',
    total: 0,
    canLoadMore: false,
    page: 1,
    keywords: [],
    foundWholePhrase: false,
    resetId: '',
    isLoading: false,
    isGetNoteLoading: false,
    isAddNewNoteLoading: false,
    isUpdateNoteLoading: false,
    isDeleteNoteLoading: false,
    isAddNewNoteSourceLoading: false,
    isUpdateNoteSourceLoading: false,
    isRemoveNoteSourceLoading: false,
    isReorderNoteSourcesLoading: false,
    isUpdateNoteBookLoading: false,
    isUpdateNoteTagsLoading: false,
    isUpdateNoteSectionLoading: false,
    setIsLoading: (stateName, isLoading) => {
      set((state) => {
        state[stateName] = isLoading
      })
    },
    setBook: (book) => {
      set((state) => {
        state.book = book
      })
    },
    setTags: (tags) => {
      set((state) => {
        state.tags = tags
      })
    },
    setWithoutBook: (withoutBook) => {
      set((state) => {
        state.withoutBook = withoutBook
      })
    },
    setWithoutTags: (withoutTags) => {
      set((state) => {
        state.withoutTags = withoutTags
      })
    },
    setInRandomOrder: (inRandomOrder) => {
      set((state) => {
        state.inRandomOrder = inRandomOrder
      })
    },
    setDateRange: ({ fromDate, toDate }) => {
      set((state) => {
        state.fromDate = fromDate
        state.toDate = toDate
      })
    },
    setNotes: (notes) => {
      set((state) => {
        state.ids = notes.map((note) => note.id)
        state.notes = {}
        notes.forEach((note) => {
          state.notes[note.id] = note
        })
      })
    },
    appendNotes: (notes) => {
      set((state) => {
        notes.forEach((note) => {
          state.ids.push(note.id)
          state.notes[note.id] = note
        })
      })
    },
    removeNote: (id) => {
      set((state) => {
        state.ids = state.ids.filter((noteId) => noteId !== id)
        delete state.notes[id]
        state.total -= 1
      })
    },
    setMetaData: (metadata) => {
      set((state) => {
        state.total = metadata.total
        state.canLoadMore = metadata.canLoadMore
        state.page = metadata.page
        state.keywords = metadata.keywords
        state.foundWholePhrase = metadata.foundWholePhrase
      })
    },
    setFilters: (filters) => {
      set((state) => {
        state.book = filters.book
        state.tags = filters.tags
        state.withoutBook = filters.withoutBook
        state.withoutTags = filters.withoutTags
        state.inRandomOrder = filters.inRandomOrder
        state.fromDate = filters.fromDate
        state.toDate = filters.toDate
      })
    },
    setNote: (id, note) => {
      set((state) => {
        state.notes[id] = note
      })
    },
    setNoteSources(id, sources) {
      set((state) => {
        state.notes[id].sources = sources
      })
    },
    reorderNotesSources(id, sourceIds) {
      set((state) => {
        const sources = sourceIds
          .map((sourceId) => state.notes[id].sources.find((source) => source.id === sourceId))
          .filter((source) => source !== undefined)
        state.notes[id].sources = sources
      })
    },
    reset: () => {
      set((state) => {
        state.resetId = randomId()
        state.isLoading = true
        state.ids = []
        state.notes = {}
        state.book = ''
        state.tags = []
        state.withoutBook = false
        state.withoutTags = false
        state.inRandomOrder = false
        state.fromDate = ''
        state.toDate = ''
        state.total = 0
        state.canLoadMore = false
        state.page = 1
        state.keywords = []
        state.foundWholePhrase = false
      })
    },
  })),
)
