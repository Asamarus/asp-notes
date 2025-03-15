import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as usersMocks from './usersApiMockData'

export const handlers = [
  http.post('/api/users/login', async () => {
    await mswDelay()
    return HttpResponse.json(usersMocks.loginResponseMock)
  }),

  http.get('/api/users', async () => {
    // return new HttpResponse(null, {
    //   status: 401,
    //   statusText: 'unauthorized',
    // })

    await mswDelay()
    return HttpResponse.json(usersMocks.getUserResponseMock)
  }),

  http.post('/api/accounts/logout', async () => {
    await mswDelay()
    return new HttpResponse(null, { status: 204 })
  }),

  http.put('/api/users/password', async () => {
    await mswDelay()
    return new HttpResponse(null, { status: 204 })
  }),
]
