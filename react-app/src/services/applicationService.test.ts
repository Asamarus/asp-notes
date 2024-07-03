import { getInitialData } from './applicationService'
import { HttpResponse, http } from 'msw'
import { applicationMocks } from '@/mocks/api/data'
import { server } from '@/mocks/api/server'

describe('applicationService', () => {
  it('getInitialData returns data on success', async () => {
    const { data, error } = await getInitialData()

    expect(data).toEqual(applicationMocks.getInitialDataResponseMock)
    expect(error).toBeUndefined()
  })

  it('getInitialData returns null data on error', async () => {
    server.use(
      http.post('/api/application/getInitialData', () => {
        return new HttpResponse('Test error', { status: 500 })
      }),
    )

    const { data, error } = await getInitialData()

    expect(data).toBeUndefined()
    expect(error).toBe('Test error')
  })
})
