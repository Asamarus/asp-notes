import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as notesMocks from './notesApiMockData'

export const handlers = [
  http.post('/api/notes/search', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.searchNotesResponseMock)
  }),

  http.post('/api/notes/autocomplete', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.autocompleteNotesResponseMock)
  }),

  http.post('/api/notes/get', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.getNoteResponseMock)
  }),

  http.post('/api/notes/create', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.createNoteResponseMock)
  }),

  http.post('/api/notes/update', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.updateNoteResponseMock)
  }),

  http.post('/api/notes/updateBook', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.updateNoteBookResponseMock)
  }),

  http.post('/api/notes/updateTags', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.updateNoteTagsResponseMock)
  }),

  http.post('/api/notes/updateSection', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.updateNoteSectionResponseMock)
  }),

  http.post('/api/notes/delete', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.deleteNoteResponseMock)
  }),

  http.post('/api/notes/getCalendarDays', async () => {
    await mswDelay()
    return HttpResponse.json(notesMocks.getCalendarDaysResponseMock)
  }),
]
