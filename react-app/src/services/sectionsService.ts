import { getClient } from './client'
import type { components } from '@/misc/openapi'

export async function getSectionsList() {
    try {
        const { data, error } = await getClient().POST('/api/sections/getList')
        return { data, error }
    } catch (error) {
        console.error('sectionsService.getSectionsList', error)
        return { data: null, error: null }
    }
}

export async function createSection(request: components['schemas']['CreateSectionRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sections/create', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sectionsService.createSection', error)
        return { data: null, error: null }
    }
}

export async function updateSection(request: components['schemas']['UpdateSectionRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sections/update', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sectionsService.updateSection', error)
        return { data: null, error: null }
    }
}

export async function deleteSection(request: components['schemas']['DeleteSectionRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sections/delete', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sectionsService.deleteSection', error)
        return { data: null, error: null }
    }
}

export async function reorderSections(request: components['schemas']['ReorderSectionsRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/sections/reorder', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('sectionsService.reorderSections', error)
        return { data: null, error: null }
    }
}