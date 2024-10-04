import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function addNoteSource(request: components['schemas']['AddNoteSourceRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/sources/add', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.addNoteSource', error)
    return { data: null, error: null }
  }
}

export async function updateNoteSource(request: components['schemas']['UpdateNoteSourceRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/sources/update', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.updateNoteSource', error)
    return { data: null, error: null }
  }
}

export async function removeNoteSource(request: components['schemas']['RemoveNoteSourceRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/sources/remove', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.removeNoteSource', error)
    return { data: null, error: null }
  }
}

export async function reorderNoteSources(
  request: components['schemas']['ReorderNoteSourcesRequest'],
) {
  try {
    const { data, error } = await getClient().POST('/api/sources/reorder', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sourcesApi.reorderNoteSources', error)
    return { data: null, error: null }
  }
}
