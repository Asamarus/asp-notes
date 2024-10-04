import rison from 'rison'
import { dispatchCustomEvent } from '@/shared/lib/useCustomEventListener'
import { events } from '@/shared/config'
import store from '@/shared/lib/store'
import { setFilters } from '@/entities/note'

import type { UrlListParams } from './types'
import type { NotesFilters } from '@/entities/note'

function updateFromUrl(urlParams: string) {
  const params = rison.decode(urlParams) as UrlListParams

  if (params.searchTerm) {
    dispatchCustomEvent(events.notesSearch.setValue, params.searchTerm)
  }

  const notesFilters = {} as NotesFilters
  notesFilters.searchTerm = params.searchTerm ?? ''
  notesFilters.book = params.book ?? ''
  notesFilters.tags = params.tags ?? []
  notesFilters.inRandomOrder = params.inRandomOrder ?? false
  notesFilters.withoutBook = params.withoutBook ?? false
  notesFilters.withoutTags = params.withoutTags ?? false
  notesFilters.fromDate = params.fromDate ?? ''
  notesFilters.toDate = params.toDate ?? ''

  store.dispatch(setFilters(notesFilters))
}

export default updateFromUrl
