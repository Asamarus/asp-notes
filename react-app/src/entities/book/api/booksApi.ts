import { getClient } from '@/shared/api'

export async function getBooksList(section?: string) {
  try {
    const { data, error } = await getClient().GET('/api/books', {
      params: { query: { section } },
    })
    return { data, error }
  } catch (error) {
    console.error('booksApi.getBooksList', error)
    return { data: null, error: null }
  }
}
