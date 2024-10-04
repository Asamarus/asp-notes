import { createSlice, PayloadAction } from '@reduxjs/toolkit'

import type { Section, AllNotesSection } from './types'

type State = {
  list: Section[]
  current: Section | null
  allNotesSection: AllNotesSection
}

const initialState: State = {
  list: [],
  current: null,
  allNotesSection: {
    name: '',
    displayName: '',
    color: '',
  },
}

export const sectionsSlice = createSlice({
  name: 'notes',
  initialState,
  reducers: {
    setSections: (state, action: PayloadAction<Section[]>) => {
      state.list = action.payload
    },
    setCurrentSection: (state, action: PayloadAction<Section | null>) => {
      state.current = action.payload
    },
    reorderSections: (state, action: PayloadAction<number[]>) => {
      const sections = action.payload
        .map((id) => state.list.find((section) => section.id === id))
        .filter((section) => section !== undefined)
      state.list = sections
    },
  },
})

export const { setSections, setCurrentSection, reorderSections } = sectionsSlice.actions
