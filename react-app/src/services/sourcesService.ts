import { getClient } from './client'
import type { components } from '@/misc/openapi'

export async function addNoteSource(request: components['schemas']['AddNoteSourceRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sources/add', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sourcesService.addNoteSource', error)
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
        console.error('sourcesService.updateNoteSource', error)
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
        console.error('sourcesService.removeNoteSource', error)
        return { data: null, error: null }
    }
}

export async function reorderNoteSources(request: components['schemas']['ReorderNoteSourcesRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sources/reorder', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sourcesService.reorderNoteSources', error)
        return { data: null, error: null }
    }
}