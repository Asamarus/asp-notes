import createFetch from '@/utils/createFetch'
import { booksService } from '@/services'
import { useBooksStore, useSectionsStore } from '@/store'

import type { components } from '@/misc/openapi'
import type { Book } from '@/store/books'

function getBookFromResponse(book: components['schemas']['BookItemResponse']): Book {
  return {
    count: book.count ?? 0,
    name: book.name ?? '',
  }
}

function setIsLoading(isLoading: boolean) {
  if (useBooksStore.getState().isMounted) {
    useBooksStore.getState().setIsLoading(isLoading)
  }
}

const getBooksListRequest = createFetch(booksService.getBooksList, setIsLoading, {
  concurrent: true,
})

function getBooks() {
  getBooksListRequest(
    {
      section: useSectionsStore.getState().currentSection?.name,
    },
    ({ data }) => {
      if (data && useBooksStore.getState().isMounted) {
        const books = data?.map(getBookFromResponse) ?? []

        useBooksStore.getState().setBooks(books)
      }
    },
  )
}

export default { getBooks }
