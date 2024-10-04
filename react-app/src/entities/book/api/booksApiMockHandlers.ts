import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as booksMocks from './booksApiMockData'

export const handlers = [
  http.post('/api/books/getList', async () => {
    await mswDelay()
    return HttpResponse.json(booksMocks.getBooksListResponseMock)
  }),

  http.post('/api/books/autocomplete', async () => {
    await mswDelay()
    return HttpResponse.json(booksMocks.autocompleteBooksResponseMock)
  }),
]
