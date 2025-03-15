import { getClient } from '@/shared/api'

export async function getApplicationData() {
  try {
    const { data, error } = await getClient().GET('/api/application-data', {})
    return { data, error }
  } catch (error) {
    console.error('applicationDataApi.getData', error)
    return { data: null, error: null }
  }
}
