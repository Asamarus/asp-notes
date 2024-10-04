import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function searchNotes(request: components['schemas']['SearchNotesRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/search', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.searchNotes', error)
    return { data: null, error: null }
  }
}

export async function autocompleteNotes(
  request: components['schemas']['AutocompleteNotesRequest'],
) {
  try {
    const { data, error } = await getClient().POST('/api/notes/autocomplete', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.autocompleteNotes', error)
    return { data: null, error: null }
  }
}

export async function getNote(request: components['schemas']['GetNoteRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/get', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.getNote', error)
    return { data: null, error: null }
  }
}

export async function createNote(request: components['schemas']['CreateNoteRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/create', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.createNote', error)
    return { data: null, error: null }
  }
}

export async function updateNote(request: components['schemas']['UpdateNoteRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/update', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.updateNote', error)
    return { data: null, error: null }
  }
}

export async function updateNoteBook(request: components['schemas']['UpdateNoteBookRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/updateBook', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.updateNoteBook', error)
    return { data: null, error: null }
  }
}

export async function updateNoteTags(request: components['schemas']['UpdateNoteTagsRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/updateTags', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.updateNoteTags', error)
    return { data: null, error: null }
  }
}

export async function updateNoteSection(
  request: components['schemas']['UpdateNoteSectionRequest'],
) {
  try {
    const { data, error } = await getClient().POST('/api/notes/updateSection', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.updateNoteSection', error)
    return { data: null, error: null }
  }
}

export async function deleteNote(request: components['schemas']['DeleteNoteRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes/delete', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.deleteNote', error)
    return { data: null, error: null }
  }
}

export async function getCalendarDays(
  request: components['schemas']['GetNoteCalendarDaysRequest'],
) {
  try {
    const { data, error } = await getClient().POST('/api/notes/getCalendarDays', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.getCalendarDays', error)
    return { data: null, error: null }
  }
}
