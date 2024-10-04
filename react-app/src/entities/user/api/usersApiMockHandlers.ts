import { http, HttpResponse } from 'msw'
import mswDelay from '@/shared/lib/mswDelay'
import * as usersMocks from './usersApiMockData'

export const handlers = [
  http.post('/api/accounts/login', async () => {
    await mswDelay()
    return HttpResponse.json(usersMocks.loginResponseMock)
  }),

  http.post('/api/accounts/getUser', async () => {
    // return new HttpResponse(null, {
    //   status: 401,
    //   statusText: 'unauthorized',
    // })

    await mswDelay()
    return HttpResponse.json(usersMocks.getUserResponseMock)
  }),

  http.post('/api/accounts/logout', async () => {
    await mswDelay()
    return HttpResponse.json(usersMocks.logoutResponseMock)
  }),

  http.post('/api/accounts/changePassword', async () => {
    await mswDelay()
    return HttpResponse.json(usersMocks.changePasswordResponseMock)
  }),
]
