import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as sourcesMocks from './sourcesApiMockData'

export const handlers = [
  http.post('/api/sources/add', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.addNoteSourceResponseMock)
  }),

  http.post('/api/sources/update', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.updateNoteSourceResponseMock)
  }),

  http.post('/api/sources/remove', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.removeNoteSourceResponseMock)
  }),

  http.post('/api/sources/reorder', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.reorderNoteSourcesResponseMock)
  }),
]
