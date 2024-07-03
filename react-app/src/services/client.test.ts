import { login } from './accountsService'
import { HttpResponse, http } from 'msw'
import { getClient } from './client'
import events from '@/events'
import { server } from '@/mocks/api/server'
import { accountsMocks } from '@/mocks/api/data'

const mockUser = accountsMocks.mockUser

vi.mock('@/helpers/notifications')
vi.mock('@/hooks/useCustomEventListener')

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

  it('should call showSuccess', async () => {
    const { showSuccess } = await import('@/helpers/notifications')
    server.use(
      http.post('/api/Accounts/login', () => {
        return HttpResponse.json({ message: 'Test successful', showNotification: true })
      }),
    )

    await login({ email: mockUser.email, password: mockUser.password })

    expect(showSuccess).toHaveBeenCalledWith('Test successful')
  })

  it('should call showError', async () => {
    const { showError } = await import('@/helpers/notifications')
    server.use(
      http.post('/api/Accounts/login', () => {
        return HttpResponse.json(
          { message: 'Test successful', showNotification: true },
          { status: 400 },
        )
      }),
    )

    await login({ email: mockUser.email, password: mockUser.password })

    expect(showError).toHaveBeenCalledWith('Test successful')
  })

  it('should dispatch unAuthorized event', async () => {
    const { dispatch } = await import('@/hooks/useCustomEventListener')
    server.use(
      http.post('/api/Accounts/login', () => {
        return HttpResponse.json(
          { message: 'User is not authorized to access this resource' },
          { status: 401 },
        )
      }),
    )

    await login({ email: mockUser.email, password: mockUser.password })

    expect(dispatch).toHaveBeenCalledWith(events.user.unAuthorized)
  })
})
