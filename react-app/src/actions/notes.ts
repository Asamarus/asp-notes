import createFetch from '@/utils/createFetch'
import { notesService } from '@/services'
import { useNotesStore, useSectionsStore, useAutocompleteStore } from '@/store'
import rison from 'rison'
import { dispatch as dispatchCustomEvent } from '@/hooks/useCustomEventListener'
import events from '@/events'

import type { components } from '@/misc/openapi'
import type { Note, NotesFilters } from '@/store/notes'

export function getNoteFromResponse(note: components['schemas']['NoteItemResponse']): Note {
  return {
    id: note.id ?? 0,
    createdAt: note.createdAt ?? '',
    updatedAt: note.updatedAt ?? '',
    title: note.title ?? '',
    section: note.section ?? '',
    content: note.content ?? '',
    preview: note.preview ?? '',
    tags: note.tags ?? [],
    book: note.book ?? '',
    sources:
      note.sources?.map((source) => ({
        id: source.id ?? '',
        title: source.title ?? '',
        link: source.link ?? '',
        description: source.description ?? '',
        image: source.image ?? '',
        showImage: source.showImage ?? false,
      })) ?? [],
  }
}

interface UrlListParams {
  searchTerm?: string
  book?: string
  tags?: string[]
  fromDate?: string
  toDate?: string
  inRandomOrder?: boolean
  withoutBook?: boolean
  withoutTags?: boolean
}

const searchNotesRequest = createFetch(
  notesService.searchNotes,
  (isLoading) => {
    useNotesStore.getState().setIsLoading('isLoading', isLoading)
  },
  {
    concurrent: true,
  },
)

const createNoteRequest = createFetch(notesService.createNote, (isLoading) => {
  useNotesStore.getState().setIsLoading('isAddNewNoteLoading', isLoading)
})

function search(page: number = 1) {
  const searchTerm = useAutocompleteStore.getState().searchTerm

  const searchParams: components['schemas']['SearchNotesRequest'] = {
    page: page,
  }

  const section = useSectionsStore.getState().currentSection

  if (section) {
    searchParams.section = section.name
  }

  if (searchTerm.length > 2) {
    searchParams.searchTerm = searchTerm
  }

  const book = useNotesStore.getState().book

  if (book) {
    searchParams.book = book
  }

  const tags = useNotesStore.getState().tags

  if (tags.length > 0) {
    searchParams.tags = tags
  }

  const inRandomOrder = useNotesStore.getState().inRandomOrder

  if (inRandomOrder) {
    searchParams.inRandomOrder = inRandomOrder
  }

  const withoutBook = useNotesStore.getState().withoutBook

  if (withoutBook) {
    searchParams.withoutBook = withoutBook
  }

  const withoutTags = useNotesStore.getState().withoutTags

  if (withoutTags) {
    searchParams.withoutTags = withoutTags
  }

  const fromDate = useNotesStore.getState().fromDate

  if (fromDate) {
    searchParams.fromDate = fromDate
  }

  const toDate = useNotesStore.getState().toDate

  if (toDate) {
    searchParams.toDate = toDate
  }

  const currentResetId = useNotesStore.getState().resetId

  searchNotesRequest(searchParams, ({ data }) => {
    if (data && currentResetId === useNotesStore.getState().resetId) {
      const notes = data.notes?.map(getNoteFromResponse) ?? []

      if (page === 1) {
        useNotesStore.getState().setNotes(notes)
      } else {
        useNotesStore.getState().appendNotes(notes)
      }

      useNotesStore.getState().setMetaData({
        total: data.count ?? 0,
        canLoadMore: data.canLoadMore ?? false,
        page: data.page ?? 1,
        keywords: data.keywords ?? [],
        foundWholePhrase: data.foundWholePhrase ?? false,
      })
    }
  })
}

function updateFromUrl(urlParams: string) {
  const params = rison.decode(urlParams) as UrlListParams

  if (params.searchTerm) {
    useAutocompleteStore.getState().setSearchTerm(params.searchTerm)
  }

  const notesFilters = {} as NotesFilters

  if (params.book) {
    notesFilters.book = params.book
  }

  if (params.tags) {
    notesFilters.tags = params.tags
  }

  if (params.inRandomOrder) {
    notesFilters.inRandomOrder = params.inRandomOrder
  }

  if (params.withoutBook) {
    notesFilters.withoutBook = params.withoutBook
  }

  if (params.withoutTags) {
    notesFilters.withoutTags = params.withoutTags
  }

  if (params.fromDate) {
    notesFilters.fromDate = params.fromDate
  }

  if (params.toDate) {
    notesFilters.toDate = params.toDate
  }

  useNotesStore.getState().setFilters(notesFilters)
}

function getUrlParams(): UrlListParams {
  const searchTerms = useAutocompleteStore.getState().searchTerm

  const urlListParams = {} as UrlListParams

  if (searchTerms.length > 2) {
    urlListParams.searchTerm = searchTerms
  }

  if (useNotesStore.getState().book) {
    urlListParams.book = useNotesStore.getState().book
  }

  if (useNotesStore.getState().tags.length > 0) {
    urlListParams.tags = useNotesStore.getState().tags
  }

  if (useNotesStore.getState().inRandomOrder) {
    urlListParams.inRandomOrder = useNotesStore.getState().inRandomOrder
  }

  if (useNotesStore.getState().withoutBook) {
    urlListParams.withoutBook = useNotesStore.getState().withoutBook
  }

  if (useNotesStore.getState().withoutTags) {
    urlListParams.withoutTags = useNotesStore.getState().withoutTags
  }

  if (useNotesStore.getState().fromDate) {
    urlListParams.fromDate = useNotesStore.getState().fromDate
  }

  if (useNotesStore.getState().toDate) {
    urlListParams.toDate = useNotesStore.getState().toDate
  }

  return urlListParams
}

function setSearchTerm(searchTerm: string) {
  useAutocompleteStore.getState().reset()
  useAutocompleteStore.getState().setSearchTerm(searchTerm)

  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function loadMore() {
  if (!useNotesStore.getState().canLoadMore || useNotesStore.getState().isLoading) {
    return
  }
  const page = useNotesStore.getState().page + 1
  search(page)
}

function toggleWithoutTags() {
  const withoutTags = !useNotesStore.getState().withoutTags
  useNotesStore.getState().setWithoutTags(withoutTags)
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function toggleWithoutBook() {
  const withoutBook = !useNotesStore.getState().withoutBook
  useNotesStore.getState().setWithoutBook(withoutBook)
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function toggleInRandomOrder() {
  const inRandomOrder = !useNotesStore.getState().inRandomOrder
  useNotesStore.getState().setInRandomOrder(inRandomOrder)
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function setBook(book: string) {
  useNotesStore.getState().setBook(book)
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function setTags(tags: string[]) {
  useNotesStore.getState().setTags(tags)
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function setDate(date: string) {
  useNotesStore.getState().setDateRange({ fromDate: date, toDate: date })
  search()
  dispatchCustomEvent(events.notesList.updateUrl)
}

function createNewNote() {
  const requestParams = {} as components['schemas']['CreateNoteRequest']

  const section = useSectionsStore.getState().currentSection

  if (!section) {
    return
  }

  requestParams.section = section.name

  const book = useNotesStore.getState().book

  if (book) {
    requestParams.book = book
  }

  createNoteRequest(requestParams, ({ data }) => {
    if (data) {
      //TODO:
      /*
            openModal({
              name: 'note',
              data: { id: response.note.id, tab: 'edit' },
            })
      */

      search()
    }
  })
}

export default {
  search,
  loadMore,
  updateFromUrl,
  getUrlParams,
  setSearchTerm,
  toggleWithoutTags,
  toggleWithoutBook,
  toggleInRandomOrder,
  setBook,
  setTags,
  setDate,
  createNewNote,
}
