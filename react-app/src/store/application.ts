import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface AllNotesSection {
  name: string
  displayName: string
  color: string
}

type State = {
  isLoading: boolean
  allNotesSection: AllNotesSection | null
}

type Actions = {
  setIsLoading: (isLoading: boolean) => void
  setAllNotesSection: (allNotesSection: AllNotesSection) => void
}

export default create<State & Actions>()(
  immer((set) => ({
    isLoading: true,
    allNotesSection: null,
    setIsLoading: (isLoading) => {
      set((state) => {
        state.isLoading = isLoading
      })
    },
    setAllNotesSection: (allNotesSection) => {
      set((state) => {
        state.allNotesSection = allNotesSection
      })
    },
  })),
)
