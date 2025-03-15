import { getBooksList } from './booksApi'
import * as booksMocks from './booksApiMockData'

describe('booksService', () => {
  it('getBooksList request test', async () => {
    const { data } = await getBooksList(booksMocks.getBooksListRequestMock)
    expect(data).toEqual(booksMocks.getBooksListResponseMock)
  })
})
