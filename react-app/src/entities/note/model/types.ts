import { components } from '@/shared/api'

export type Note = components['schemas']['NotesItemResponse']

export type NoteSource = components['schemas']['NotesSource']

export interface NotesFilters {
  searchTerm: string
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

export type AutocompleteNote = components['schemas']['NotesAutocompleteResultItem']
