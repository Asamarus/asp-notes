import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { booksMocks } from '../data'

export const handlers = [
    http.post('/api/books/getList', async () => {
        await delay()
        return HttpResponse.json(booksMocks.getBooksListResponseMock)
    }),

    http.post('/api/books/autocomplete', async () => {
        await delay()
        return HttpResponse.json(booksMocks.autocompleteBooksResponseMock)
    }),
]
