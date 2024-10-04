import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as tagsMocks from './tagsApiMockData'

export const handlers = [
  http.post('/api/tags/getList', async () => {
    await mswDelay()
    return HttpResponse.json(tagsMocks.getTagsListResponseMock)
  }),

  http.post('/api/tags/autocomplete', async () => {
    await mswDelay()
    return HttpResponse.json(tagsMocks.autocompleteTagsResponseMock)
  }),
]
