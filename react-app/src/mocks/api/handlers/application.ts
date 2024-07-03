import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { applicationMocks } from '../data'

export const handlers = [
  http.post('/api/application/getInitialData', async () => {
    await delay()
    return HttpResponse.json(applicationMocks.getInitialDataResponseMock)
  }),
]
