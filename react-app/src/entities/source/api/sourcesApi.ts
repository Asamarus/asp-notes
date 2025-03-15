import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function addNoteSource({
  noteId,
  request,
}: {
  noteId: number
  request: components['schemas']['NotesSourceCreateRequest']
}) {
  try {
    const { data, error } = await getClient().POST('/api/notes/{noteId}/sources', {
      params: { path: { noteId } },
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.addNoteSource', error)
    return { data: null, error: null }
  }
}

export async function updateNoteSource({
  noteId,
  sourceId,
  request,
}: {
  noteId: number
  sourceId: string
  request: components['schemas']['NotesSourceUpdateRequest']
}) {
  try {
    const { data, error } = await getClient().PUT('/api/notes/{noteId}/sources/{id}', {
      params: { path: { noteId, id: sourceId } },
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.updateNoteSource', error)
    return { data: null, error: null }
  }
}

export async function removeNoteSource({ noteId, sourceId }: { noteId: number; sourceId: string }) {
  try {
    const { data, error } = await getClient().DELETE('/api/notes/{noteId}/sources/{id}', {
      params: { path: { noteId, id: sourceId } },
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.removeNoteSource', error)
    return { data: null, error: null }
  }
}

export async function reorderNoteSources({
  noteId,
  request,
}: {
  noteId: number
  request: components['schemas']['NotesSourcesReorderRequest']
}) {
  try {
    const { data, error } = await getClient().PUT('/api/notes/{noteId}/sources/reorder', {
      params: { path: { noteId } },
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.reorderNoteSources', error)
    return { data: null, error: null }
  }
}
