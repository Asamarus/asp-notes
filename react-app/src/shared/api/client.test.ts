// import { HttpResponse, http } from 'msw'
import { getClient } from './client'
// import { events } from '@/shared/config'
// import { server } from '@/app/lib/msw/server'
// import { usersApi, usersApiMockData } from '@/entities/user'
// const mockUser = usersApiMockData.mockUser

vi.mock('@/shared/lib/notifications')
vi.mock('@/shared/lib/useCustomEventListener')

describe('api client', () => {
  afterAll(() => {
    vi.restoreAllMocks()
  })

  it('should return a client', () => {
    const client = getClient()
    expect(client).toBeDefined()
  })

  it('should always return the same client', () => {
    const client1 = getClient()
    const client2 = getClient()
    expect(client1).toBe(client2)
  })

  // it('should call showSuccess', async () => {
  //   const { showSuccess } = await import('@/shared/lib/notifications')
  //   server.use(
  //     http.post('/api/accounts/login', () => {
  //       return HttpResponse.json({ message: 'Test successful', showNotification: true })
  //     }),
  //   )

  //   await usersApi.login({ email: mockUser.email, password: mockUser.password })

  //   expect(showSuccess).toHaveBeenCalledWith('Test successful')
  // })

  // it('should call showError', async () => {
  //   const { showError } = await import('@/shared/lib/notifications')
  //   server.use(
  //     http.post('/api/accounts/login', () => {
  //       return HttpResponse.json(
  //         { message: 'Test successful', showNotification: true },
  //         { status: 400 },
  //       )
  //     }),
  //   )

  //   await usersApi.login({ email: mockUser.email, password: mockUser.password })

  //   expect(showError).toHaveBeenCalledWith('Test successful')
  // })

  // it('should dispatch unAuthorized event', async () => {
  //   const { dispatchCustomEvent } = await import('@/shared/lib/useCustomEventListener')
  //   server.use(
  //     http.post('/api/accounts/login', () => {
  //       return HttpResponse.json(
  //         { message: 'User is not authorized to access this resource' },
  //         { status: 401 },
  //       )
  //     }),
  //   )

  //   await usersApi.login({ email: mockUser.email, password: mockUser.password })

  //   expect(dispatchCustomEvent).toHaveBeenCalledWith(events.user.unAuthorized)
  // })
})
