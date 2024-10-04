import { getBooksList, autocompleteBooks } from './booksApi'
import * as booksMocks from './booksApiMockData'

describe('booksService', () => {
  it('getBooksList request test', async () => {
    const { data } = await getBooksList(booksMocks.getBooksListRequestMock)
    expect(data).toEqual(booksMocks.getBooksListResponseMock)
  })

  it('autocompleteBooks request test', async () => {
    const { data } = await autocompleteBooks(booksMocks.autocompleteBooksRequestMock)
    expect(data).toEqual(booksMocks.autocompleteBooksResponseMock)
  })
})
