import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { sourcesMocks } from '../data'

export const handlers = [
  http.post('/api/sources/add', async () => {
    await delay()
    return HttpResponse.json(sourcesMocks.addNoteSourceResponseMock)
  }),

  http.post('/api/sources/update', async () => {
    await delay()
    return HttpResponse.json(sourcesMocks.updateNoteSourceResponseMock)
  }),

  http.post('/api/sources/remove', async () => {
    await delay()
    return HttpResponse.json(sourcesMocks.removeNoteSourceResponseMock)
  }),

  http.post('/api/sources/reorder', async () => {
    await delay()
    return HttpResponse.json(sourcesMocks.reorderNoteSourcesResponseMock)
  }),
]
