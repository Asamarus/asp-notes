import store from '@/shared/lib/store'

import type { UrlListParams } from './types'

function getUrlParams(): UrlListParams {
  const filters = store.getState().notes.filters

  const urlListParams = {} as UrlListParams

  if (filters.searchTerm.length > 2) {
    urlListParams.searchTerm = filters.searchTerm
  }

  if (filters.book) {
    urlListParams.book = filters.book
  }

  if (filters.tags.length > 0) {
    urlListParams.tags = filters.tags
  }

  if (filters.withoutBook) {
    urlListParams.withoutBook = filters.withoutBook
  }

  if (filters.withoutTags) {
    urlListParams.withoutTags = filters.withoutTags
  }

  if (filters.inRandomOrder) {
    urlListParams.inRandomOrder = filters.inRandomOrder
  }

  if (filters.fromDate) {
    urlListParams.fromDate = filters.fromDate
  }

  if (filters.toDate) {
    urlListParams.toDate = filters.toDate
  }

  return urlListParams
}

export default getUrlParams
