import { getClient } from './client'
import type { components } from '@/misc/openapi'

export async function getTagsList(request: components['schemas']['GetTagsListRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/tags/getList', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('tagsService.getTagsList', error)
        return { data: null, error: null }
    }
}

export async function autocompleteTags(request: components['schemas']['AutocompleteTagsRequest']) {
    try {
        const { data, error } = await getClient().POST('/api/tags/autocomplete', {
            body: request,
        })
        return { data, error }
    } catch (error) {
        console.error('tagsService.autocompleteTags', error)
        return { data: null, error: null }
    }
}