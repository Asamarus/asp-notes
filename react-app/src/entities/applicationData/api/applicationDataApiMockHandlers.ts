import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as applicationDataMocks from './applicationDataApiMockData'

export const handlers = [
  http.get('/api/application-data', async () => {
    await mswDelay()
    return HttpResponse.json(applicationDataMocks.getApplicationDataResponseMock)
  }),
]
