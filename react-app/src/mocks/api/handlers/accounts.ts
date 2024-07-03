import { http, HttpResponse } from 'msw'
import { delay } from '../helpers'
import { accountsMocks } from '../data'

export const handlers = [
  http.post('/api/accounts/login', async () => {
    await delay()
    return HttpResponse.json(accountsMocks.loginResponseMock)
  }),

  http.post('/api/accounts/getUser', async () => {
    // return new HttpResponse(null, {
    //   status: 401,
    //   statusText: 'unauthorized',
    // })

    await delay()
    return HttpResponse.json(accountsMocks.getUserResponseMock)
  }),

  http.post('/api/accounts/logout', async () => {
    await delay()
    return HttpResponse.json(accountsMocks.logoutResponseMock)
  }),

  http.post('/api/accounts/changePassword', async () => {
    await delay()
    return HttpResponse.json(accountsMocks.changePasswordResponseMock)
  }),
]
