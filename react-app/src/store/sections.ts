import { create } from 'zustand'
import { immer } from 'zustand/middleware/immer'

export interface Section {
  id: number
  name: string
  displayName: string
  color: string
}

type LoadingState =
  | 'isSectionsListLoading'
  | 'isAddNewSectionLoading'
  | 'isUpdateSectionLoading'
  | 'isDeleteSectionLoading'
  | 'isReorderSectionsLoading'

type State = {
  sections: Section[]
  currentSection: Section | null
  isSectionsListLoading: boolean
  isAddNewSectionLoading: boolean
  isUpdateSectionLoading: boolean
  isDeleteSectionLoading: boolean
  isReorderSectionsLoading: boolean
  sectionsLoaded: boolean
}

type Actions = {
  setIsLoading: (stateName: LoadingState, isLoading: boolean) => void
  setSections: (sections: Section[]) => void
  setCurrentSection: (section: Section | null) => void
  setSectionsLoaded: (loaded: boolean) => void
  reorderSections: (ids: number[]) => void
}

export default create<State & Actions>()(
  immer((set) => ({
    sectionsLoaded: false,
    sections: [],
    currentSection: null,
    isSectionsListLoading: false,
    isAddNewSectionLoading: false,
    isUpdateSectionLoading: false,
    isDeleteSectionLoading: false,
    isReorderSectionsLoading: false,
    setIsLoading: (stateName, isLoading) => {
      set((state) => {
        state[stateName] = isLoading
      })
    },
    setSectionsLoaded: (loaded) => {
      set((state) => {
        state.sectionsLoaded = loaded
      })
    },
    setSections: (sections) => {
      set((state) => {
        state.sections = sections
      })
    },
    setCurrentSection: (section) => {
      set((state) => {
        state.currentSection = section
      })
    },
    reorderSections(ids) {
      set((state) => {
        const sections = ids
          .map((id) => state.sections.find((section) => section.id === id))
          .filter((section) => section !== undefined)
        state.sections = sections
      })
    },
  })),
)
