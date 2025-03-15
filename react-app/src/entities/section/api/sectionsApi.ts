import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function getSectionsList() {
  try {
    const { data, error } = await getClient().GET('/api/sections')
    return { data, error }
  } catch (error) {
    console.error('sectionsApi.getSectionsList', error)
    return { data: null, error: null }
  }
}

export async function createSection(request: components['schemas']['SectionsCreateRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/sections', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sectionsApi.createSection', error)
    return { data: null, error: null }
  }
}

export async function updateSection({
  id,
  request,
}: {
  id: number
  request: components['schemas']['SectionsUpdateRequest']
}) {
  try {
    const { data, error } = await getClient().PUT('/api/sections/{id}', {
      params: { path: { id } },
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sectionsApi.updateSection', error)
    return { data: null, error: null }
  }
}

export async function deleteSection(id: number) {
  try {
    const { data, error } = await getClient().DELETE('/api/sections/{id}', {
      params: { path: { id } },
    })
    return { data, error }
  } catch (error) {
    console.error('sectionsApi.deleteSection', error)
    return { data: null, error: null }
  }
}

export async function reorderSections(request: components['schemas']['SectionsReorderRequest']) {
  try {
    const { data, error } = await getClient().PUT('/api/sections/reorder', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('sectionsApi.reorderSections', error)
    return { data: null, error: null }
  }
}
