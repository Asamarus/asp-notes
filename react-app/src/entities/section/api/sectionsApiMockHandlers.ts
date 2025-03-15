import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as sectionsMocks from './sectionsApiMockData'

export const handlers = [
  http.get('/api/sections', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.getSectionsListResponseMock)
  }),

  http.post('/api/sections', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.createSectionResponseMock)
  }),

  http.put('/api/sections/1', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.updateSectionResponseMock)
  }),

  http.delete('/api/sections/1', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.deleteSectionResponseMock)
  }),

  http.put('/api/sections/reorder', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.reorderSectionsResponseMock)
  }),
]
