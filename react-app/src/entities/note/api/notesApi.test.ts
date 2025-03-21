﻿import {
  searchNotes,
  autocompleteNotes,
  getNote,
  createNote,
  updateNote,
  deleteNote,
  getCalendarDays,
} from './notesApi'
import * as notesMocks from './notesApiMockData'

describe('notesApi', () => {
  it('searchNotes request test', async () => {
    const { data } = await searchNotes(notesMocks.searchNotesRequestMock)
    expect(data).toEqual(notesMocks.searchNotesResponseMock)
  })

  it('autocompleteNotes request test', async () => {
    const { data } = await autocompleteNotes(notesMocks.autocompleteNotesRequestMock)
    expect(data).toEqual(notesMocks.autocompleteNotesResponseMock)
  })

  it('getNote request test', async () => {
    const { data } = await getNote(notesMocks.getNoteRequestMock)
    expect(data).toEqual(notesMocks.getNoteResponseMock)
  })

  it('createNote request test', async () => {
    const { data } = await createNote(notesMocks.createNoteRequestMock)
    expect(data).toEqual(notesMocks.createNoteResponseMock)
  })

  it('updateNote request test', async () => {
    const { data } = await updateNote(notesMocks.updateNoteRequestMock)
    expect(data).toEqual(notesMocks.updateNoteResponseMock)
  })

  it('deleteNote request test', async () => {
    const { data } = await deleteNote(notesMocks.deleteNoteRequestMock)
    expect(data).toEqual(undefined)
  })

  it('getCalendarDays request test', async () => {
    const { data } = await getCalendarDays(notesMocks.getCalendarDaysRequestMock)
    expect(data).toEqual(notesMocks.getCalendarDaysResponseMock)
  })
})
