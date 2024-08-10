import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { sectionsMocks } from '../data'

export const handlers = [
    http.post('/api/sections/getList', async () => {
        await delay()
        return HttpResponse.json(sectionsMocks.getSectionsListResponseMock)
    }),

    http.post('/api/sections/create', async () => {
        await delay()
        return HttpResponse.json(sectionsMocks.createSectionResponseMock)
    }),

    http.post('/api/sections/update', async () => {
        await delay()
        return HttpResponse.json(sectionsMocks.updateSectionResponseMock)
    }),

    http.post('/api/sections/delete', async () => {
        await delay()
        return HttpResponse.json(sectionsMocks.deleteSectionResponseMock)
    }),

    http.post('/api/sections/reorder', async () => {
        await delay()
        return HttpResponse.json(sectionsMocks.reorderSectionsResponseMock)
    }),
]
