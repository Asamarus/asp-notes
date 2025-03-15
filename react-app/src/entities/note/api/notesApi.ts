import { getClient } from '@/shared/api'
import type { components, paths } from '@/shared/api'

export async function searchNotes(queryParams: paths['/api/notes']['get']['parameters']['query']) {
  try {
    const { data, error } = await getClient().GET('/api/notes', {
      params: { query: queryParams },
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.searchNotes', error)
    return { data: null, error: null }
  }
}

export async function autocompleteNotes(
  queryParams: paths['/api/notes/autocomplete']['get']['parameters']['query'],
) {
  try {
    const { data, error } = await getClient().GET('/api/notes/autocomplete', {
      params: { query: queryParams },
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.autocompleteNotes', error)
    return { data: null, error: null }
  }
}

export async function getNote(id: number) {
  try {
    const { data, error } = await getClient().GET('/api/notes/{id}', {
      params: { path: { id } },
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.getNote', error)
    return { data: null, error: null }
  }
}

export async function createNote(request: components['schemas']['NotesCreateRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/notes', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.createNote', error)
    return { data: null, error: null }
  }
}

export async function updateNote({
  id,
  request,
}: {
  id: number
  request: components['schemas']['NotesPatchRequest']
}) {
  try {
    const { data, error } = await getClient().PATCH('/api/notes/{id}', {
      params: { path: { id } },
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.updateNote', error)
    return { data: null, error: null }
  }
}

export async function deleteNote(id: number) {
  try {
    const { data, error } = await getClient().DELETE('/api/notes/{id}', {
      params: { path: { id } },
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.deleteNote', error)
    return { data: null, error: null }
  }
}

export async function getCalendarDays(
  queryParams: paths['/api/notes/calendar']['get']['parameters']['query'],
) {
  try {
    const { data, error } = await getClient().GET('/api/notes/calendar', {
      params: { query: queryParams },
    })
    return { data, error }
  } catch (error) {
    console.error('notesApi.getCalendarDays', error)
    return { data: null, error: null }
  }
}
