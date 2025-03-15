import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as notesMocks from './notesApiMockData'

export const handlers = [
  http.get('/api/notes', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.searchNotesResponseMock)
  }),

  http.get('/api/notes/autocomplete', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.autocompleteNotesResponseMock)
  }),

  http.get('/api/notes/1', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.getNoteResponseMock)
  }),

  http.post('/api/notes', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.createNoteResponseMock)
  }),

  http.patch('/api/notes/1', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.updateNoteResponseMock)
  }),

  http.delete('/api/notes/1', async () => {
    await mswDelay()
    return new HttpResponse(null, { status: 204 })
  }),

  http.get('/api/notes/calendar', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.getCalendarDaysResponseMock)
  }),
]
