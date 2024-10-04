import { getClient } from '@/shared/api'
import type { components } from '@/shared/api'

export async function getTagsList(request: components['schemas']['GetTagsListRequest']) {
  try {
    const { data, error } = await getClient().POST('/api/tags/getList', {
      body: request,
    })
    return { data, error }
  } catch (error) {
    console.error('tagsApi.getTagsList', error)
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
    console.error('tagsApi.autocompleteTags', error)
    return { data: null, error: null }
  }
}
