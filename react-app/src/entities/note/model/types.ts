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

export interface AutocompleteNote {
  id: number
  title: string
}
