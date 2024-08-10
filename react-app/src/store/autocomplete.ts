import randomId from '@/utils/randomId'
import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface AutocompleteNote {
  id: number
  title: string
}

interface AutocompleteResponse {
  notes: AutocompleteNote[]
  books: string[]
  tags: string[]
}

type State = {
  isLoading: boolean
  searchTerm: string
  resetId: string
  notes: AutocompleteNote[]
  books: string[]
  tags: string[]
}

type Actions = {
  setIsLoading: (isLoading: boolean) => void
  setSearchTerm: (searchTerm: string) => void
  setResponse: (response: AutocompleteResponse) => void
  reset: () => void
}

export default create<State & Actions>()(
  immer((set) => ({
    isLoading: false,
    resetId: '',
    searchTerm: '',
    notes: [],
    books: [],
    tags: [],
    setIsLoading: (isLoading) => {
      set((state) => {
        state.isLoading = isLoading
      })
    },
    setSearchTerm: (searchTerm) => {
      set((state) => {
        state.searchTerm = searchTerm
      })
    },
    setResponse: ({ notes, books, tags }) => {
      set((state) => {
        state.notes = notes
        state.books = books
        state.tags = tags
      })
    },
    reset: () => {
      set((state) => {
        state.isLoading = false
        state.resetId = randomId()
        state.tags = []
        state.books = []
        state.notes = []
      })
    },
  })),
)
