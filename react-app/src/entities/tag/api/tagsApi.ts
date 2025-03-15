import { getClient } from '@/shared/api'

export async function getTagsList(section?: string) {
  try {
    const { data, error } = await getClient().GET('/api/tags', {
      params: { query: { section } },
    })
    return { data, error }
  } catch (error) {
    console.error('tagsApi.getTagsList', error)
    return { data: null, error: null }
  }
}
