import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as sourcesMocks from './sourcesApiMockData'

export const handlers = [
  http.post('/api/notes/1/sources', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.addNoteSourceResponseMock)
  }),

  http.put('/api/notes/1/sources/1', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.updateNoteSourceResponseMock)
  }),

  http.delete('/api/notes/1/sources/1', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.removeNoteSourceResponseMock)
  }),

  http.put('/api/notes/1/sources/reorder', async () => {
    await mswDelay()
    return HttpResponse.json(sourcesMocks.reorderNoteSourcesResponseMock)
  }),
]
