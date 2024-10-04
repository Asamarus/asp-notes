import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as sectionsMocks from './sectionsApiMockData'

export const handlers = [
  http.post('/api/sections/getList', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.getSectionsListResponseMock)
  }),

  http.post('/api/sections/create', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.createSectionResponseMock)
  }),

  http.post('/api/sections/update', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.updateSectionResponseMock)
  }),

  http.post('/api/sections/delete', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.deleteSectionResponseMock)
  }),

  http.post('/api/sections/reorder', async () => {
    await mswDelay()
    return HttpResponse.json(sectionsMocks.reorderSectionsResponseMock)
  }),
]
