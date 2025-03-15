import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as tagsMocks from './tagsApiMockData'

export const handlers = [
  http.get('/api/tags', async () => {
    await mswDelay()
    return HttpResponse.json(tagsMocks.getTagsListResponseMock)
  }),
]
