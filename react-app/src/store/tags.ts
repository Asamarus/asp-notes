import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface Tag {
  count: number
  name: string
  selected: boolean
}

type State = {
  tags: Tag[]
  isLoading: boolean
  isMounted: boolean
}

type Actions = {
  setIsLoading: (isLoading: boolean) => void
  setIsMounted: (isMounted: boolean) => void
  setTags: (tags: Tag[]) => void
  setTagIsSelected: (name: string, isSelected: boolean) => void
  setSelectedTags: (tags: string[]) => void
  reset: () => void
}

export default create<State & Actions>()(
  immer((set) => ({
    tags: [],
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
    setTags: (tags: Tag[]) => {
      set((state) => {
        state.tags = tags
      })
    },
    setTagIsSelected: (name, isSelected) => {
      set((state) => {
        state.tags = state.tags.map((tag) => {
          if (tag.name === name) {
            tag.selected = isSelected
          }
          return tag
        })
      })
    },
    setSelectedTags: (tags: string[]) => {
      set((state) => {
        state.tags = state.tags.map((tag) => {
          tag.selected = tags.includes(tag.name)
          return tag
        })
      })
    },
    reset: () => {
      set((state) => {
        state.tags = []
        state.isLoading = true
      })
    },
  })),
)
