import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function getBooksList(request: components['schemas']['GetBooksListRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/books/getList', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('booksApi.getBooksList', error)
    return { data: null, error: null }
  }
}

export async function autocompleteBooks(
  request: components['schemas']['AutocompleteBooksRequest'],
) {
  try {
    const { data, error } = await getClient().POST('/api/books/autocomplete', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('booksApi.autocompleteBooks', error)
    return { data: null, error: null }
  }
}
