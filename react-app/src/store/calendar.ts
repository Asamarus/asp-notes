import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface CalendarDay {
  count: number
  date: string
}

type State = {
  isLoading: boolean
  isMounted: boolean
  days: CalendarDay[]
}

type Actions = {
  setIsLoading: (isLoading: boolean) => void
  setIsMounted: (isMounted: boolean) => void
  setDays: (days: CalendarDay[]) => void
  reset: () => void
}

export default create<State & Actions>()(
  immer((set) => ({
    isLoading: false,
    isMounted: false,
    days: [],
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
    setDays: (days) => {
      set((state) => {
        state.days = days
      })
    },
    reset: () => {
      set((state) => {
        state.isLoading = true
        state.days = []
      })
    },
  })),
)
