import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as booksMocks from './booksApiMockData'

export const handlers = [
  http.get('/api/books', async () => {
    await mswDelay()
    return HttpResponse.json(booksMocks.getBooksListResponseMock)
  }),
]
