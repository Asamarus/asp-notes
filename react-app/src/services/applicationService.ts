import { getClient } from './client'

export async function getInitialData() {
  try {
    const { data, error } = await getClient().POST('/api/application/getInitialData')
    return { data, error }
  } catch (error) {
    console.error('applicationService.getInitialData', error)
    return { data: null, error: null }
  }
}
