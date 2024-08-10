import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface Book {
  count: number
  name: string
}

type State = {
  books: Book[]
  isLoading: boolean
  isMounted: boolean
}

type Actions = {
  setIsLoading: (isLoading: boolean) => void
  setIsMounted: (isMounted: boolean) => void
  setBooks: (books: Book[]) => void
  reset: () => void
}

export default create<State & Actions>()(
  immer((set) => ({
    books: [],
    isLoading: true,
    isMounted: false,
    setIsLoading: (isLoading) => {
      set((state) => {
        state.isLoading = isLoading
      })
    },
    setIsMounted: (isMounted) => {
      set((state) => {
        state.isMounted = isMounted
      })
    },
    setBooks: (books) => {
      set((state) => {
        state.books = books
      })
    },
    reset: () => {
      set((state) => {
        state.books = []
        state.isLoading = true
      })
    },
  })),
)
