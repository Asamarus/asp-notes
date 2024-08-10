import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { tagsMocks } from '../data'

export const handlers = [
    http.post('/api/tags/getList', async () => {
        await delay()
        return HttpResponse.json(tagsMocks.getTagsListResponseMock)
    }),

    http.post('/api/tags/autocomplete', async () => {
        await delay()
        return HttpResponse.json(tagsMocks.autocompleteTagsResponseMock)
    }),
]
