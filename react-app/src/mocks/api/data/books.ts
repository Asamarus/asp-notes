import type { components } from '@/misc/openapi'

export const getBooksListRequestMock: components['schemas']['GetBooksListRequest'] = {
  section: 'section1',
}

export const getBooksListResponseMock: components['schemas']['BookItemResponse'][] = [
  {
    name: 'book1',
    count: 1,
  },
  {
    name: 'book2',
    count: 10,
  },
  {
    name: 'book3',
    count: 15,
  },
]

export const autocompleteBooksRequestMock: components['schemas']['AutocompleteBooksRequest'] = {
  section: 'section1',
  searchTerm: 'book',
}

export const autocompleteBooksResponseMock: string[] = ['book1', 'book2', 'book3']
